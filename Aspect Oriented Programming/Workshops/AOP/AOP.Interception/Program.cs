using AOP.Interception.WeatherForecast.Config;
using AOP.Interception.WeatherForecast.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddFusionCache().WithRegisteredMemoryCache();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(GeneralDiConfigLoader.Load);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (IWeatherForecastService weatherForecastService) => await weatherForecastService.GetWeatherForecast())
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

