namespace AOP.Interception.WeatherForecast.Repositories;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    public Models.WeatherForecast GetWeatherForecast() 
        => new(DateOnly.FromDateTime(DateTime.UtcNow), 21, "Breezy light workshop weather");
}