using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BCCIAwbApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly HttpClient _client = new HttpClient();
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly string _apiKey = "2392b26f6dcc83d61f13bcce5faaccff";

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {

            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet]
        [Route("GetWeatherForecast/{city}/{date}")]
        public async Task<IActionResult> GetWeatherForecast(string city, string date)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";

                HttpResponseMessage response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { Message = "Failed to fetch weather data" });
                }

                string responseData = await response.Content.ReadAsStringAsync();
                JObject weatherData = JObject.Parse(responseData);

                foreach (var forecast in weatherData["list"])
                {
                    string forecastDate = forecast["dt_txt"].ToString().Split(' ')[0]; // Extract date part
                    if (forecastDate == date)
                    {
                        string condition = forecast["weather"][0]["main"].ToString();
                        double rainVolume = forecast["rain"]?["3h"]?.ToObject<double>() ?? 0; // Rain in last 3 hours
                        double rainChance = forecast["pop"]?.ToObject<double>() * 100 ?? 0; // Probability of precipitation

                        return Ok(new
                        {
                            City = city,
                            Date = date,
                            WeatherCondition = condition,
                            RainProbability = rainChance,
                            RainVolume = rainVolume
                        });
                    }
                }

                return NotFound(new { Message = "No forecast data available for the given date" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetWeeklyWeather/{city}")]
        public async Task<IActionResult> GetWeeklyWeather(string city)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";

                HttpResponseMessage response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { Message = "Failed to fetch weather data" });
                }

                string responseData = await response.Content.ReadAsStringAsync();
                JObject weatherData = JObject.Parse(responseData);

                List<object> weeklyForecast = new List<object>();

                DateTime today = DateTime.Now.Date;
                DateTime weekEnd = today.AddDays(7);

                foreach (var forecast in weatherData["list"])
                {
                    string forecastDate = forecast["dt_txt"].ToString().Split(' ')[0]; 
                    DateTime forecastDateTime = DateTime.Parse(forecastDate);

                    if (forecastDateTime >= today && forecastDateTime <= weekEnd)
                    {
                        string condition = forecast["weather"][0]["main"].ToString();
                        double rainVolume = forecast["rain"]?["3h"]?.ToObject<double>() ?? 0; 
                        double rainChance = forecast["pop"]?.ToObject<double>() * 100 ?? 0; 

                        weeklyForecast.Add(new
                        {
                            Date = forecastDate,
                            WeatherCondition = condition,
                            RainProbability = rainChance,
                            RainVolume = rainVolume
                        });
                    }
                }

                if (weeklyForecast.Count == 0)
                {
                    return NotFound(new { Message = "No weather data available for the upcoming week" });
                }

                return Ok(new { City = city, Forecast = weeklyForecast });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [HttpGet]
        [Route("PredictMatchStatus/{city}/{date}")]
        public async Task<IActionResult> PredictMatchStatus(string city, string date)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";

                HttpResponseMessage response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { Message = "Failed to fetch weather data" });
                }

                string responseData = await response.Content.ReadAsStringAsync();
                JObject weatherData = JObject.Parse(responseData);

                foreach (var forecast in weatherData["list"])
                {
                    string forecastDate = forecast["dt_txt"].ToString().Split(' ')[0]; // Extract only date
                    if (forecastDate == date)
                    {
                        string condition = forecast["weather"][0]["main"].ToString();
                        double rainProbability = forecast["pop"]?.ToObject<double>() * 100 ?? 0; // Probability of precipitation
                        double rainVolume = forecast["rain"]?["3h"]?.ToObject<double>() ?? 0; // Rain volume in last 3 hours

                        string matchStatus = "Match will happen as scheduled.";
                        if (condition.Contains("Rain") || rainProbability > 70 || rainVolume > 1)
                        {
                            matchStatus = "Match may be interrupted due to rain.";
                        }

                        return Ok(new
                        {
                            City = city,
                            Date = date,
                            WeatherCondition = condition,
                            RainProbability = rainProbability,
                            RainVolume = rainVolume,
                            MatchStatus = matchStatus
                        });
                    }
                }

                return NotFound(new { Message = "No weather data available for the given date" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
    }
}
