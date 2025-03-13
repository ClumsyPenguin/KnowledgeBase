using AOP.Interception.Caching;

namespace AOP.Interception.WeatherForecast.Repositories;

    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        [Cache(5)]
        public Models.WeatherForecast GetWeatherForecast() 
            => new(DateOnly.FromDateTime(DateTime.UtcNow), 21, "Breezy light workshop weather");
    }