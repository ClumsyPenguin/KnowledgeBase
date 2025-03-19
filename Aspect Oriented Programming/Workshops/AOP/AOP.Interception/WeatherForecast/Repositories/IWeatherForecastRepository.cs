namespace AOP.Interception.WeatherForecast.Repositories;

public interface IWeatherForecastRepository
{
    public Task<Models.WeatherForecast> GetWeatherForecast();
}