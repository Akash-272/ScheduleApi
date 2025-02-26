using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using BCCIAwbApi.Models;

namespace BCCIAwbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MatchesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        [Route("GetAllMatches")]
        public IActionResult GetMatches()
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query= "SELECT MatchID,SeriesID,MatchDate,Venue,Team1,Team2 FROM Matches";
                var matchList=con.Query<Match>(query).ToList();
                if (matchList.Any())
                {
                    return Ok(matchList);
                }
                return NotFound(new {statusCode=100,ErrorMessage= "No Data Found" });
            }
        }
        
    }
}
