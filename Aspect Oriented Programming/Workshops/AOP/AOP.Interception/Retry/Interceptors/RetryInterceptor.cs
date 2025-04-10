using Castle.DynamicProxy;

namespace AOP.Interception.Retry.Interceptors;

public class RetryInterceptor : IRetryInterceptor
{
    public void InterceptSynchronous(IInvocation invocation)
    {
        throw new NotImplementedException();
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        throw new NotImplementedException();
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        throw new NotImplementedException();
    }
}