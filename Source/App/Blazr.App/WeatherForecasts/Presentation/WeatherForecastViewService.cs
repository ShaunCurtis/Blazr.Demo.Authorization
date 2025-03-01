/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Presentation;

public class WeatherForecastViewService
{
    private readonly IWeatherForecastDataBroker? weatherForecastDataBroker;

    public List<WeatherForecast>? Records { get; private set; }

    private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    private IAuthorizationService AuthorizationService { get; set; }

    public string Message { get; set; } = string.Empty;

    public WeatherForecastViewService(IWeatherForecastDataBroker weatherForecastDataBroker, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
    { 
        this.weatherForecastDataBroker = weatherForecastDataBroker;
        this.AuthenticationStateProvider = authenticationState;
        this.AuthorizationService = authorizationService;
    }

    public async ValueTask GetForecastsAsync()
    {
        this.Records = null;
        this.NotifyListChanged(this.Records, EventArgs.Empty);
        this.Records = await weatherForecastDataBroker!.GetWeatherForecastsAsync();
        this.NotifyListChanged(this.Records, EventArgs.Empty);
    }

    public async ValueTask AddRecord(WeatherForecast record)
    {
        this.Message = string.Empty;
        var authstate = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
        var result = await this.AuthorizationService.AuthorizeAsync(authstate.User, null, AppPolicies.IsUserPolicy);
        if (result.Succeeded)
        {
            await weatherForecastDataBroker!.AddForecastAsync(record);
            await GetForecastsAsync();
        }
        else
            this.Message = "That Ain't Allowed!";
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
