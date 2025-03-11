using Castle.DynamicProxy;

namespace AOP.Interception.Logging.Interceptors;

public class LogInterceptor : ILogInterceptor
{
    private readonly ILogger<LogInterceptor> _logger;

    public LogInterceptor(ILogger<LogInterceptor> logger)
    {
        _logger = logger;
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        LogInformation($"Intercepted method call :{invocation.MethodInvocationTarget?.Name}");
        
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

    public void InterceptAsynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue!;
            await task;
        }
        catch (Exception exception)
        {
            LogError(exception);
            throw;
        }
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue!;
            var result = await task;

            LogInformation(result?.ToString());

            return result;
        }
        catch (Exception exception)
        {
            LogError(exception);
            throw;
        }
    }
    
    private void LogInformation(object? message)
    {
        _logger.LogInformation($"Intercepted call with result: {message?.ToString() ?? "LogInterceptor Info}"}");
    }

    private void LogError(Exception exception)
    {
        _logger.LogError(exception, $"Intercepted exception with message: {exception.Message ?? "LogInterceptor Error"}");
    }
}