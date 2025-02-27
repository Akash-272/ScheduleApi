using Frontend.DTO;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Frontend.Controllers
{
    public class SeriesController1 : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7064/api");
        private readonly HttpClient _client;
        public SeriesController1()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<SeriesView> seriesList = new List<SeriesView>();
            HttpResponseMessage response=_client.GetAsync(_client.BaseAddress + "/Series/GetAllSeries").Result;

            if(response.IsSuccessStatusCode)
            {
                string data=response.Content.ReadAsStringAsync().Result;
                seriesList = JsonConvert.DeserializeObject<List<SeriesView>>(data);
            }
            return View(seriesList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(SeriesDto series)
        {
            try
            {
                string data = JsonConvert.SerializeObject(series);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Series/AddSeries", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Series Added Successfully";
                    //return Content("<script>alert('Series Added Successfully'); window.location.href='/Series/Index';</script>", "text/html");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                //return Content($"<script>alert('Error: {ex.Message}');</script>", "text/html");
                throw;
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id) 
        {
            SeriesDto series = new SeriesDto();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Series/GetSeriesById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                series = JsonConvert.DeserializeObject<SeriesDto>(data);
            }

            return View(series);
        }

        [HttpPost]
        public IActionResult Edit(SeriesDto series) 
        {
            try
            {
                string data = JsonConvert.SerializeObject(series);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Series/UpdateSeries", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Series Updated Successfully";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                throw;
            }
            return View();
        }

        [HttpGet]
        [Route("SeriesController1/Details/{id}")]
        public IActionResult Details(int id)
        {
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Series/GetSeriesById/" + id).Result;
                    
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var series = JsonConvert.DeserializeObject<SeriesView>(data);
                return View(series);
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Series/DeleteSeries/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Series Deleted Successfully";
            }
            else
            {
                TempData["errorMessage"] = "Failed to Delete Series or Series Not Found";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
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

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
