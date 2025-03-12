using AOP.Interception.Extensions;
using AOP.Interception.Logging.Interceptors;
using AOP.Interception.WeatherForecast.Repositories;
using AOP.Interception.WeatherForecast.Services;
using Autofac;
using Castle.DynamicProxy;

namespace AOP.Interception.WeatherForecast.Config;

public class DiConfig : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ProxyGenerator>().AsSelf();
        
        //interceptors
        builder.RegisterType<LogInterceptor>().As<ILogInterceptor>();
        
        //dependencies
        builder.RegisterTypeWithInterception<WeatherForecastService, IWeatherForecastService>(typeof(ILogInterceptor));
        builder.RegisterTypeWithInterception<WeatherForecastRepository, IWeatherForecastRepository>(typeof(ILogInterceptor));
    }
}