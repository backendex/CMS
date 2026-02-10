using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMS.src.API.Controller
{
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet("site/{siteId}")]
        public async Task<IActionResult> GetTours(Guid siteId)
        {
            var tours = await _tourService.GetToursBySiteIdAsync(siteId);
            return Ok(tours);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null) return NotFound();
            return Ok(tour);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TourDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newTour = await _tourService.CreateTourAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newTour.Id }, newTour);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TourDto dto)
        {
            var success = await _tourService.UpdateTourAsync(id, dto);
            if (!success) return NotFound();
            return Ok(new { message = "Tour actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _tourService.DeleteTourAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Tour eliminado" });
        }
    }
}

