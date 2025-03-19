using Castle.DynamicProxy;
using ZiggyCreatures.Caching.Fusion;

namespace AOP.Interception.Caching.Interceptors;

public class MemoryCacheInterceptor : ICacheInterceptor
{
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<MemoryCacheInterceptor> _logger;

    public MemoryCacheInterceptor(IFusionCache fusionCache, ILogger<MemoryCacheInterceptor> logger)
    {
        _fusionCache = fusionCache;
        _logger = logger;
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        if (IsCacheable(invocation, out var itemLifeSpan))
        {
            var cacheKey = CacheKeyGenerator.Generate(invocation.Method, invocation.Arguments);
            var factoryCalled = false;

            invocation.ReturnValue = _fusionCache.GetOrSet(
                cacheKey,
                _ =>
                {
                    factoryCalled = true;
                    _logger.LogInformation("Cache MISS for key '{CacheKey}'", cacheKey);
                    return GetNonCachedValue(invocation);
                },
                options => options.SetDuration(itemLifeSpan)
            );

            if (!factoryCalled)
            {
                _logger.LogInformation("Cache HIT for key '{CacheKey}'", cacheKey);
            }
        }
        else
        {
            invocation.Proceed();
        }
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        // The method call is void, so no caching needed
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
        var factoryCalled = false;

        var result = await _fusionCache.GetOrSetAsync(
            cacheKey,
            async _ =>
            {
                factoryCalled = true;
                _logger.LogInformation("Cache MISS for key '{CacheKey}'", cacheKey);
                return await GetNonCachedValueAsync<TResult>(proceed, invocation);
            },
            options => options.SetDuration(itemLifeSpan)
        );

        if (!factoryCalled)
        {
            _logger.LogInformation("Cache HIT for key '{CacheKey}'", cacheKey);
        }

        return result!;
    }

    private static async Task<TResult> GetNonCachedValueAsync<TResult>(
        IInvocationProceedInfo proceed,
        IInvocation invocation)
    {
        proceed.Invoke();
        var task = (Task<TResult>)invocation.ReturnValue!;
        return await task;
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
            itemLifeSpan = (Attribute.GetCustomAttribute(invocation.MethodInvocationTarget!, typeof(CacheAttribute)) as CacheAttribute)!.ItemLifeSpan;
            return true;
        }

        itemLifeSpan = TimeSpan.Zero;
        return false;
    }
}
