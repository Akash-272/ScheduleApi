using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using BCCIAwbApi.Models;
using BCCIAwbApi.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;

namespace BCCIAwbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ScheduleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        [Route("GetAllSchedules")]
        public IActionResult GetMatches()
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM Schedule";
                var scheduleList = con.Query<Schedule>(query).ToList();

                string query1 = "SELECT * FROM Series";
                var seriesList = con.Query<Series>(query1).ToList();

                string query2 = "SELECT * FROM Matches";
                var matchList = con.Query<Match>(query2).ToList();

                foreach (var schedule in scheduleList)
                {
                    schedule.Series = seriesList.FirstOrDefault(s => s.SeriesID == schedule.SeriesID);
                    schedule.Match = matchList.FirstOrDefault(m => m.MatchID == schedule.MatchID);
                }

                return scheduleList.Any() ? Ok(scheduleList) : NotFound(new { statusCode = 100, ErrorMessage = "No Data Found" });
            }
        }


        [HttpPost]
        [Route("CreateSchedule")]
        public IActionResult CreateSchdule([FromBody] ScheduleDto schedule)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "INSERT INTO Schedule (SeriesID, MatchID, ScheduledDate) VALUES (@SeriesID, @MatchID, @ScheduledDate)";
                var res=con.Execute(query, schedule);

                if(res> 0)
                {
                    return Ok(new { statusCode = 200, Message = "Schedule Created Successfully" });
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "Failed to Create Schedule" });
            }
        }


        [HttpPut]
        [Route("UpdateSchedule")]
        public IActionResult UpdateSchedule([FromBody] UpdateScheduleDto schedule)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = @"

        UPDATE Schedule
        SET ScheduledDate = @ScheduledDate
        WHERE ScheduleID = @ScheduleID";
                var res = con.Execute(query, schedule);

                if (res > 0)
                {
                    return Ok(new { statusCode = 200, Message = "Schedule Updated Successfully" });
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "Failed to Update Schedule" });
            }
        }

        [HttpDelete]
        [Route("DeleteSchedule/{id}")]
        public IActionResult DeleteSchedule(int id)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "DELETE FROM Schedule WHERE ScheduleID = @ScheduleID";
                var res = con.Execute(query, new { ScheduleID = id });

                if (res > 0)
                {
                    return Ok(new { statusCode = 200, Message = "Schedule Deleted Successfully" });
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "Failed to Delete Schedule" });
            }
        }

        [HttpGet]
        [Route("GetScheduleBySeriesId/{id}")]
        public IActionResult GetScheduleForSeries(int id) {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM Schedule WHERE SeriesID = @SeriesID";
                var scheduleList = con.Query<Schedule>(query, new { SeriesID = id }).ToList();

                string query1 = "SELECT * FROM Series";
                var seriesList = con.Query<Series>(query1).ToList();

                string query2 = "SELECT * FROM Matches";
                var matchList = con.Query<Match>(query2).ToList();

                foreach (var schedule in scheduleList)
                {
                    schedule.Series = seriesList.FirstOrDefault(s => s.SeriesID == schedule.SeriesID);
                    schedule.Match = matchList.FirstOrDefault(m => m.MatchID == schedule.MatchID);
                }

                if(scheduleList.Any())
                {
                    return Ok(scheduleList);
                }
                return NotFound(new { statusCode = 100, ErrorMessage = "No Data Found" });
            }
        }

    }
}
