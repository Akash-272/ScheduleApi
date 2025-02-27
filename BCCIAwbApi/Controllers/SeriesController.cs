using BCCIAwbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using BCCIAwbApi.DTO;

namespace BCCIAwbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SeriesController(IConfiguration configuration)
        {
            _configuration = configuration;
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
        public IActionResult GetSeries(SeriesDto series) {
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
