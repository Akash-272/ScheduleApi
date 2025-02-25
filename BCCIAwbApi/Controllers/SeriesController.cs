using BCCIAwbApi.Models;
using BCCIAwbApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BCCIAwbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesRepository _repository;

        public SeriesController(ISeriesRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpPost]
        public IActionResult CreateSeries([FromBody] Series series)
        {
            _repository.AddSeries(series);
            return CreatedAtAction(nameof(GetSeriesById), new { id = series.SeriesID }, series);
        }

        [HttpPut("{id}")]
        public IActionResult ModifySeries(int id, [FromBody] Series updatedSeries)
        {
            _repository.UpdateSeries(updatedSeries);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSeries(int id)
        {
            _repository.DeleteSeries(id);
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetAllSeries()
        {
            return Ok(_repository.GetAllSeries());
        }

        [HttpGet("{id}")]
        public IActionResult GetSeriesById(int id)
        {
            var series = _repository.GetSeriesById(id);
            if (series == null) return NotFound();
            return Ok(series);
        }
    }
}
