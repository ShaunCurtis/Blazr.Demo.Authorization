

using System.Net.Http.Json;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

/// <summary>
/// This is the client version of the data broker.
/// It's used in the Wasm SPA and gets its data from the API 
/// </summary>
public class WeatherForecastAPIDataBroker : IWeatherForecastDataBroker
{
    private readonly HttpClient? httpClient;

    public WeatherForecastAPIDataBroker(HttpClient httpClient)
        => this.httpClient = httpClient!;

    public async ValueTask<bool> AddForecastAsync(WeatherForecast record)
    {
        var response = await this.httpClient!.PostAsJsonAsync<WeatherForecast>($"/api/weatherforecast/add", record);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        return result;
    }

    public async ValueTask<bool> DeleteForecastAsync(Guid Id)
    {
        var response = await this.httpClient!.PostAsJsonAsync<Guid>($"/api/weatherforecast/delete", Id);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        return result;
    }

    public async ValueTask<List<WeatherForecast>> GetWeatherForecastsAsync()
    {
        var list = await this.httpClient!.GetFromJsonAsync<List<WeatherForecast>>($"/api/weatherforecast/list");
        return list!;
    }
}
