using AOP.Interception.WeatherForecast.Repositories;
using AOP.Interception.WeatherForecast.Services;
using Autofac;

namespace AOP.Interception.WeatherForecast.Config;

public static class GeneralDiConfigLoader
{
    public static void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<DiConfig>();
    }

    public static void LoadWithoutInterceptors(ContainerBuilder builder)
    {
        builder.RegisterType<WeatherForecastService>().As<IWeatherForecastService>();
        builder.RegisterType<WeatherForecastRepository>().As<IWeatherForecastRepository>();
    }

    public static void LoadWithInterceptors(ContainerBuilder builder)
    {
        builder.RegisterModule<DiConfig>();
    }
}
