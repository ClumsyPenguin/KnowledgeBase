using Autofac;

namespace AOP.Interception.WeatherForecast.Config;

public static class GeneralDiConfigLoader
{
    public static void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<DiConfig>();
    }
}
