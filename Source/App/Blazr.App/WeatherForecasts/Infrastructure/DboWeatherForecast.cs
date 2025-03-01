/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.App.Infrastructure;

internal record DboWeatherForecast
{
    public Guid Id { get; init; }

    public Guid OwnerId { get; init; }

    public DateTime Date { get; init; }

    public int TemperatureC { get; init; }

    public string? Summary { get; init; }

    public WeatherForecast ToDto()
        => new WeatherForecast
        {
            Id = this.Id,
            OwnerId = this.OwnerId,
            Date = this.Date,
            TemperatureC = this.TemperatureC,
            Summary = this.Summary
        };

    public static DboWeatherForecast FromDto(WeatherForecast record)
        => new DboWeatherForecast
        {
            Id = record.Id,
            OwnerId = record.OwnerId,
            Date = record.Date,
            TemperatureC = record.TemperatureC,
            Summary = record.Summary
        };
}

