using AOP.Interception.WeatherForecast.Repositories;

namespace AOP.Interception.WeatherForecast.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IWeatherForecastRepository _forecastRepository;

    public WeatherForecastService(IWeatherForecastRepository forecastRepository)
    {
        _forecastRepository = forecastRepository;
    }
    
    public async Task<Models.WeatherForecast> GetWeatherForecast()
    {
        return await _forecastRepository.GetWeatherForecast();
    }
}