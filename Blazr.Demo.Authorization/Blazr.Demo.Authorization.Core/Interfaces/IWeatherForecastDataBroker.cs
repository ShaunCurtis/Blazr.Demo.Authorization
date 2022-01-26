/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Authorization.Core
{
    /// <summary>
    /// The data broker interface abstracts the interface between the logic layer and the data layer.
    /// </summary>
    public interface IWeatherForecastDataBroker
    {
        public ValueTask<bool> AddForecastAsync(WeatherForecast record);

        public ValueTask<bool> DeleteForecastAsync(Guid Id);

        public ValueTask<List<WeatherForecast>> GetWeatherForecastsAsync();
    }
}
