using Autofac;
using Autofac.Builder;
using Castle.DynamicProxy;

namespace AOP.Interception.Extensions;

public static class ContainerBuilderExtensions
{
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
}