using AOP.Interception.Caching;

namespace AOP.Interception.WeatherForecast.Repositories;

    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private static readonly Random Random = new();
        
        [Cache(5)]
        public async Task<Models.WeatherForecast> GetWeatherForecast()
        { 
            await Task.Delay(Random.Next(30, 150)); 
            
            return new Models.WeatherForecast(DateOnly.FromDateTime(DateTime.UtcNow), 21, "Breezy light workshop weather"); 
        }
    }