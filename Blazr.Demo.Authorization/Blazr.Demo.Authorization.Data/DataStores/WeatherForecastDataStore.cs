/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Demo.Authorization.Data;

public class WeatherForecastDataStore
{
    private int _recordsToGet = 5;
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private List<DboWeatherForecast> _records;

    public WeatherForecastDataStore()
        =>
        _records = GetForecasts();

    private List<DboWeatherForecast> GetForecasts()
    {
        var list = new List<DboWeatherForecast>();
        var rng = new Random();
        for (var i = 1; i <= _recordsToGet; i++)
        {
            var c = rng.Next(1, 3);
            list.Add(new DboWeatherForecast
            {
                Id = Guid.NewGuid(),
                OwnerId = new Guid($"10000000-0000-0000-0000-20000000000{c}"),
                Date = DateTime.Now.AddDays(i),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
        return list;
    }

    public ValueTask<bool> AddForecastAsync(WeatherForecast weatherForecast)
    {
        var record = DboWeatherForecast.FromDto(weatherForecast);
        _records.Add(record);
        return ValueTask.FromResult(true);
    }

    public ValueTask<bool> DeleteForecastAsync(Guid Id)
    {
        var deleted = false;
        var record = _records.FirstOrDefault(item => item.Id == Id);
        if (record != null)
        {
            _records.Remove(record);
            deleted = true;
        }
        return ValueTask.FromResult(deleted);
    }

    public ValueTask<bool> UpdateForecastAsync(WeatherForecast weatherForecast)
    {
        var record = _records.FirstOrDefault(item => item.Id == weatherForecast.Id);
        if (record != null)
            _records.Remove(record);
        record = DboWeatherForecast.FromDto(weatherForecast);
        _records.Add(record);
        return ValueTask.FromResult(true);
    }

    public ValueTask<List<WeatherForecast>> GetWeatherForecastsAsync()
    {
        var list = new List<WeatherForecast>();
        _records
            .ForEach(item => list.Add(item.ToDto()));
        return ValueTask.FromResult(list.OrderBy(item => item.Date).ToList());
    }

    public void OverrideWeatherForecastDateSet(List<WeatherForecast> list)
    {
        _records.Clear();
        list.ForEach(item => _records.Add(DboWeatherForecast.FromDto(item)));
    }

    public static List<WeatherForecast> CreateTestForecasts(int count)
    {
        var list = new List<WeatherForecast>();
        var rng = new Random();
        for (var i = 1; i <= count; i++)
        {
            var c = rng.Next(1, 3);
            list.Add(new WeatherForecast
            {
                Id = Guid.NewGuid(),
                OwnerId = new Guid($"10000000-0000-0000-0000-20000000000{c}"),
                Date = DateTime.Now.AddDays(i),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
        return list;
    }
}

