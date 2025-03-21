namespace CampusConnect.Models;

public class WeatherInfo
{
    public double Temperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
}