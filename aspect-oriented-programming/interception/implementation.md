# Implementation

### Interceptor

```csharp
public class LogInterceptor : ILogInterceptor
{
    private readonly ILogger<LogInterceptor> _logger;

    public LogInterceptor(ILogger<LogInterceptor> logger)
    {
        _logger = logger;
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        _logger.LogInformation($"Intercepted method call: {invocation.MethodInvocationTarget?.Name} from {invocation.InvocationTarget!.ToString()!.Split('.').Last()}");
        
        try
        {
            invocation.Proceed();
        }
        catch (Exception exception)
        {
            LogError(exception);
            throw;
        }
    }
    
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
    {
        _logger.LogInformation($"Intercepted method call: {invocation.MethodInvocationTarget?.Name} from {invocation.InvocationTarget!.ToString()!.Split('.').Last()}");

        try
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue!;
            var result = await task;
            
            _logger.LogInformation($"Intercepted call with result: {result?.ToString() ?? "LogInterceptor Info}"}");

            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Intercepted exception with message: {exception.Message ?? "LogInterceptor Error"}");
            throw;
        }
    }
}
```

### Registration

```csharp
builder.RegisterType<ProxyGenerator>().AsSelf();
builder.RegisterType<LogInterceptor>().As<ILogInterceptor>();

builder.RegisterTypeWithInterception<WeatherForecastService, IWeatherForecastService>(typeof(ILogInterceptor));
```

### The magic behind

```csharp
public static IRegistrationBuilder<TU, SimpleActivatorData, SingleRegistrationStyle> RegisterTypeWithInterception<T, TU>(this ContainerBuilder builder, params Type[] interceptorTypes) where T : class, TU where TU : class
{
    builder.RegisterType<T>().AsSelf();
    
    return builder.Register(c =>
    {
        var store = c.Resolve<T>();
        var generator = c.Resolve<ProxyGenerator>();
        var interceptors = ResolveInterceptors(interceptorTypes, c);

        return generator.CreateInterfaceProxyWithTargetInterface<TU>(store, interceptors);
    });
}

private static IAsyncInterceptor[] ResolveInterceptors(Type[] interceptorTypes, IComponentContext context)
{
    var interceptors = new List<IAsyncInterceptor>();

    foreach (var interceptorType in interceptorTypes)
    {
        interceptors.Add((context.Resolve(interceptorType) as IAsyncInterceptor)!);
    }

    return interceptors.ToArray();
}
```







