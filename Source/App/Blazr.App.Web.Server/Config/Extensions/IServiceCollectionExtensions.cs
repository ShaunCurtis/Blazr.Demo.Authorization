/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazr.App.Web.Server;

public static class IServiceCollectionExtensions
{
    public static void AddAppBlazorServerServices(this IServiceCollection services)
    {
        services.AddSingleton<WeatherForecastDataStore>();
        services.AddSingleton<IWeatherForecastDataBroker, WeatherForecastServerDataBroker>();
        services.AddScoped<WeatherForecastViewService>();

        services.AddScoped<AuthenticationStateProvider, VerySimpleAuthenticationStateProvider>();
        services.AddAppPolicyServices();
        services.AddAuthorization(config =>
        {
            foreach (var policy in AppPolicies.Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });
    }

    public static void AddAppBlazorWASMServices(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastDataBroker, WeatherForecastAPIDataBroker>();
        services.AddScoped<WeatherForecastViewService>();
    }

    public static void AddAppWASMServerServices(this IServiceCollection services)
    {
        services.AddSingleton<WeatherForecastDataStore>();
        services.AddSingleton<IWeatherForecastDataBroker, WeatherForecastServerDataBroker>();
    }
}

