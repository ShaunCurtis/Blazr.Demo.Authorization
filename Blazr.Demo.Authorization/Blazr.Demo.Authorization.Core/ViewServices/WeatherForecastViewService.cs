/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Authorization.Core
{
    public class WeatherForecastViewService
    {
        private readonly IWeatherForecastDataBroker? weatherForecastDataBroker;

        public List<WeatherForecast>? Records { get; private set; }

        public WeatherForecastViewService(IWeatherForecastDataBroker weatherForecastDataBroker)
            => this.weatherForecastDataBroker = weatherForecastDataBroker!;

        public async ValueTask GetForecastsAsync()
        {
            this.Records = null;
            this.NotifyListChanged(this.Records, EventArgs.Empty);
            this.Records = await weatherForecastDataBroker!.GetWeatherForecastsAsync();
            this.NotifyListChanged(this.Records, EventArgs.Empty);
        }

        public async ValueTask AddRecord(WeatherForecast record)
        {
            await weatherForecastDataBroker!.AddForecastAsync(record);
            await GetForecastsAsync();
        }

        public async ValueTask DeleteRecord(Guid Id)
        {
            _ = await weatherForecastDataBroker!.DeleteForecastAsync(Id);
            await GetForecastsAsync();
        }

        public event EventHandler<EventArgs>? ListChanged;

        public void NotifyListChanged(object? sender, EventArgs e)
            => ListChanged?.Invoke(sender, e);
    }
}
