using BCCIAwbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;

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


    }
}
