using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;

namespace AOP.Interception.Caching.Interceptors;

public class MemoryCacheInterceptor : ICacheInterceptor
{
    private readonly IMemoryCache _memoryCache;


    public MemoryCacheInterceptor(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        if (IsCacheable(invocation, out var itemLifeSpan))
        {
            var cacheKey = CacheKeyGenerator.Generate(invocation.Method, invocation.Arguments);
            invocation.ReturnValue = _memoryCache.Set(GetNonCachedValue(invocation)!, itemLifeSpan);
        }
        else
        {
            invocation.Proceed();
        }
    }


    public void InterceptAsynchronous(IInvocation invocation)
    {
        //This method is one which doesn't return a value, hence no caching desired
        invocation.Proceed();
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        if (IsCacheable(invocation, out var itemLifeSpan))
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation, itemLifeSpan);
        }
        else
        {
            invocation.Proceed();
        }
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation, TimeSpan itemLifeSpan)
    {
        var proceed = invocation.CaptureProceedInfo();
        var cacheKey = CacheKeyGenerator.Generate(invocation.Method, invocation.Arguments);
        
        if (_memoryCache.TryGetValue(cacheKey, out TResult? cachedValue))
            return cachedValue!;
        
        
        return _memoryCache.Set(cacheKey,
            _ => GetNonCachedValueAsync<TResult>(proceed, invocation),
            itemLifeSpan);
    }

    private static async Task<TResult> GetNonCachedValueAsync<TResult>(IInvocationProceedInfo proceed, IInvocation invocation)
    {
        proceed.Invoke();

        var task = (Task<TResult>)invocation.ReturnValue!;
        var result = await task;

        return result;
    }

    private static object? GetNonCachedValue(IInvocation invocation)
    {
        invocation.Proceed();

        return invocation.ReturnValue;
    }

    private static bool IsCacheable(IInvocation invocation, out TimeSpan itemLifeSpan)
    {
        if (Attribute.IsDefined(invocation.MethodInvocationTarget!, typeof(CacheAttribute)))
        {
            var cacheAttribute = Attribute.GetCustomAttribute(invocation.MethodInvocationTarget, typeof(CacheAttribute)) as CacheAttribute;
            itemLifeSpan = cacheAttribute!.ItemLifeSpan;

            return true;
        }

        itemLifeSpan = TimeSpan.Zero;
        return false;
    }
    
    public async Task<TItem?> GetOrSetAsync<TItem>(
        IMemoryCache cache,
        object key,
        Func<Task<TItem>> factory,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        if (cache.TryGetValue(key, out TItem? cacheEntry))
        {
            return cacheEntry;
        }

        var value = await factory();

        cache.Set(key, value, absoluteExpirationRelativeToNow);

        return value;
    }
}