using AOP.Interception.WeatherForecast.Config;
using AOP.Interception.WeatherForecast.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZiggyCreatures.Caching.Fusion;

namespace AOP.Benchmark;

public class Config : ManualConfig
{
    public Config()
    {
        AddLogger(ConsoleLogger.Default);
        AddColumnProvider(DefaultColumnProviders.Instance);

        AddExporter(HtmlExporter.Default);
        AddExporter(CsvExporter.Default);
        AddExporter(RPlotExporter.Default);

        AddDiagnoser(MemoryDiagnoser.Default);
        AddAnalyser(EnvironmentAnalyser.Default);
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<WeatherForecastBenchmark>(new Config());
    }
}

[SimpleJob(invocationCount: 1)]
public class WeatherForecastBenchmark
{
    private IWeatherForecastService _interceptedService = null!;
    private IWeatherForecastService _plainService = null!;

    [GlobalSetup]
    public void Setup()
    {
        var interceptedApp = CreateWebApp(interceptorsEnabled: true);
        _interceptedService = interceptedApp.Services.GetRequiredService<IWeatherForecastService>();
            
        var plainApp = CreateWebApp(interceptorsEnabled: false);
        _plainService = plainApp.Services.GetRequiredService<IWeatherForecastService>();
    }

    [Benchmark]
    public async Task InterceptedCall()
    {
        for (var i = 0; i < 1000; i++)
        {
            await _interceptedService.GetWeatherForecast();
        }
    }

    [Benchmark]
    public async Task PlainCall()
    {
        for (var i = 0; i < 1000; i++)
        {
            await _plainService.GetWeatherForecast();
        }
    }

    private static WebApplication CreateWebApp(bool interceptorsEnabled)
    {
        var builder = WebApplication.CreateBuilder();
            
        builder.Services.AddMemoryCache();
        builder.Services.AddFusionCache().WithRegisteredMemoryCache();
        
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            if (interceptorsEnabled) 
                GeneralDiConfigLoader.LoadWithInterceptors(containerBuilder);
            else 
                GeneralDiConfigLoader.LoadWithoutInterceptors(containerBuilder);
        });

        var app = builder.Build();

        return app;
    }
}
