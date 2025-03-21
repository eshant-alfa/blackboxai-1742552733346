using Microsoft.AspNetCore.Mvc;
using CampusConnect.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CampusConnect.Controllers;

public class WeatherController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey = "YOUR_API_KEY"; // You would typically store this in configuration
    private readonly string _city = "Your City"; // Default city, could be configurable

    public WeatherController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var weatherInfo = await GetWeatherInfo();
        return View(weatherInfo);
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentWeather()
    {
        var weatherInfo = await GetWeatherInfo();
        return Json(weatherInfo);
    }

    private async Task<WeatherInfo> GetWeatherInfo()
    {
        // For demo purposes, return mock data
        // In production, you would make an API call to a weather service
        return new WeatherInfo
        {
            Temperature = 22.5,
            Description = "Partly Cloudy",
            Icon = "cloud-sun",
            Humidity = 65,
            WindSpeed = 5.2
        };
    }
}