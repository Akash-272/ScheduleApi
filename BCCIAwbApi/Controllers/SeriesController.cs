using BCCIAwbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using BCCIAwbApi.DTO;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace BCCIAwbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client = new HttpClient();
        public SeriesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _client.BaseAddress = new Uri("https://localhost:7064/api");
        }
        [HttpGet]
        [Route("GetAllSeries")]
        public IActionResult GetSeries() {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM Series";
                var seriesList = con.Query<Series>(query).ToList();

                string query1= "SELECT * FROM Matches";
                var matchList=con.Query<Match>(query1).ToList();

                foreach(var s in seriesList)
                {
                    s.Matches=matchList.Where(m=>m.SeriesID==s.SeriesID).ToList();
                }
                if (seriesList.Any())
                {
                    return Ok(seriesList);
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "No Data Found" });
            }
        }

        [HttpGet]
        [Route("GetSeriesById/{id}")]
        public IActionResult GetSeriesById(int id)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM Series WHERE SeriesID=@id";
                var series = con.QueryFirstOrDefault<Series>(query, new { id });

                if (series != null)
                {
                    string matchQuery = "SELECT * FROM Matches WHERE SeriesID=@id";
                    var matchList = con.Query<Match>(matchQuery, new { id }).ToList();

                    series.Matches = matchList;

                    return Ok(series);
                }

                return NotFound(new { statusCode = 100, ErrorMessage = "No Data Found" });
            }
        }





        [HttpPost]
        [Route("AddSeries")]
        public IActionResult GetSeries(SeriesDto series)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "INSERT INTO Series (Name,StartDate,EndDate) VALUES (@Name,@StartDate,@EndDate)";
                var result = con.Execute(query, series);
                if (result > 0)
                {
                    return Ok(new { statusCode = 200, Message = "Series Added Successfully" });
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "Failed to Add Series" });
            }
        }

        [HttpPost]
        [Route("AddSeriesForCurrentWeek")]
        public IActionResult CreateSeriesForCurrentWeek(SeriesDto series)
        {
            DateTime today = DateTime.Now;
            DateTime weekStart = today.AddDays(-(int)today.DayOfWeek + 1);
            DateTime weekEnd = weekStart.AddDays(6);
            DateTime startDate = DateTime.ParseExact(series.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(series.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (startDate < weekStart || endDate > weekEnd)
            {
                return BadRequest(new { Message = "Series must be within the current week!" });
            }

            string data = JsonConvert.SerializeObject(series);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Series/AddSeries", content).Result;

            return Ok(new { Message = "Series added successfully!" });
        }

        [HttpGet]
        [Route("GetWeatherForecast/{date}")]
        public async Task<IActionResult> GetWeatherForecast(string date)
        {
            string apiKey = "2392b26f6dcc83d61f13bcce5faaccff";
            string city = "Pune";
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}";

            HttpResponseMessage response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to fetch weather data");
            }

            string responseData = await response.Content.ReadAsStringAsync();
            dynamic weatherData = JsonConvert.DeserializeObject(responseData);

            foreach (var forecast in weatherData.list)
            {
                string forecastDate = forecast.dt_txt;
                if (forecastDate.Contains(date))
                {
                    double rainChance = forecast.pop * 100;
                    return Ok(new { Date = date, RainProbability = rainChance });
                }
            }

            return NotFound("No forecast data available");
        }

        [HttpPut]
        [Route("UpdateSeries")]
        public IActionResult UpdateSeries(SeriesDto series)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "UPDATE Series SET Name=@Name,StartDate=@StartDate,EndDate=@EndDate WHERE SeriesID=@SeriesID";
                var result = con.Execute(query, series);
                if (result > 0)
                {
                    return Ok(new { statusCode = 200, Message = "Series Updated Successfully" });
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "Failed to Update Series" });
            }
        }

        [HttpDelete]
        [Route("DeleteSeries/{id}")]
        public IActionResult DeleteSeries(int id)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "DELETE FROM Series WHERE SeriesID=@id";
                var result = con.Execute(query, new { id });
                if (result > 0)
                {
                    return Ok(new { statusCode = 200, Message = "Series Deleted Successfully" });
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "Failed to Delete Series" });
            }
        }
    }
}
