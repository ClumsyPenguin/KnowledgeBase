using AOP.Interception.WeatherForecast.Config;
using AOP.Interception.WeatherForecast.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NBomber.Contracts;
using NBomber.CSharp;
using ZiggyCreatures.Caching.Fusion;

public class Program
{
    public static async Task Main(string[] args)
    {
        var interceptedApp = CreateWebApp(interceptorsEnabled: true);
        await interceptedApp.StartAsync();
        var interceptedClient = interceptedApp.GetTestClient();

        var plainApp = CreateWebApp(interceptorsEnabled: false);
        await plainApp.StartAsync();
        var plainClient = plainApp.GetTestClient();

        var interceptedScenario = CreateHttpScenario("Intercepted_API", interceptedClient);
        var plainScenario = CreateHttpScenario("Plain_API", plainClient);

        NBomberRunner.RegisterScenarios(interceptedScenario, plainScenario)
                     .Run();

        await interceptedApp.StopAsync();
        await plainApp.StopAsync();
    }
    
    private static WebApplication CreateWebApp(bool interceptorsEnabled)
    {
        var builder = WebApplication.CreateBuilder();
        
        builder.WebHost.UseTestServer();
        
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
        
        app.UseHttpsRedirection();
        app.MapGet("/weatherforecast",
            async (IWeatherForecastService weatherForecastService) =>
                await weatherForecastService.GetWeatherForecast());
        
        return app;
    }
    
    private static ScenarioProps CreateHttpScenario(string scenarioName, HttpClient client)
    {
        return Scenario.Create(scenarioName, async context =>
        {
            var response = await client.GetAsync("/weatherforecast");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 5, during: TimeSpan.FromSeconds(10)),
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(15)),
            Simulation.RampingConstant(copies: 50, during: TimeSpan.FromSeconds(10)));
    }
}
