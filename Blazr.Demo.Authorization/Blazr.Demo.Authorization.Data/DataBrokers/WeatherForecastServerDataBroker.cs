/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Demo.Authorization.Data
{
    /// <summary>
    /// This is the server version of the data broker.
    /// It's used in the Server SPA and in the API web server for the WASM SPA 
    /// </summary>
    public class WeatherForecastServerDataBroker : IWeatherForecastDataBroker
    {
        private readonly WeatherForecastDataStore weatherForecastDataStore;

        public WeatherForecastServerDataBroker(WeatherForecastDataStore weatherForecastDataStore)
            => this.weatherForecastDataStore = weatherForecastDataStore;

        public async ValueTask<bool> AddForecastAsync(WeatherForecast record)
            => await this.weatherForecastDataStore!.AddForecastAsync(record);

        public async ValueTask<bool> DeleteForecastAsync(Guid Id)
            => await this.weatherForecastDataStore!.DeleteForecastAsync(Id);

        public async ValueTask<List<WeatherForecast>> GetWeatherForecastsAsync()
            => await this.weatherForecastDataStore!.GetWeatherForecastsAsync();
    }
}
