namespace ElTocardo.API.Endpoints;

/// <summary>
///     Weather forecast record
/// </summary>
/// <param name="Date">The date of the forecast</param>
/// <param name="TemperatureC">Temperature in Celsius</param>
/// <param name="Summary">Weather summary</param>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    ///     Temperature in Fahrenheit
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}