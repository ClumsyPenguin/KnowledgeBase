namespace AOP.Interception.WeatherForecast.Services;

public interface IWeatherForecastService
{
    public Task<Models.WeatherForecast> GetWeatherForecast();
}