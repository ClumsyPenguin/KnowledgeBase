using AOP.Interception.WeatherForecast.Repositories;

namespace AOP.Interception.WeatherForecast.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IWeatherForecastRepository _forecastRepository;

    public WeatherForecastService(IWeatherForecastRepository forecastRepository)
    {
        _forecastRepository = forecastRepository;
    }
    
    public Task<Models.WeatherForecast> GetWeatherForecast()
    {
        var forecast = _forecastRepository.GetWeatherForecast();
        
        return Task.FromResult(forecast);
    }
}