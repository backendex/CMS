using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMS.src.API.Controller
{
    [ApiController]
    [Route("api/tour")]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet("/getTour")]
        public async Task<IActionResult> GetTours([FromQuery] Guid siteId)
        {
            try
            {
                if (siteId == Guid.Empty) return BadRequest("SiteId es requerido.");
                var tours = await _tourService.GetTourAsync(siteId);
                return Ok(tours);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la lista", error = ex.Message });
            }

        }

        [HttpGet("/getTourById")]
        public async Task<IActionResult> GetById(Guid id, Guid siteId)
        {
            try
            {
                var tour = await _tourService.GetTourByIdAsync(id, siteId);
                if (tour == null || tour.SiteId != siteId)
                {
                    return NotFound(new { message = "Tour no encontrado para el SiteId proporcionado" });
                }
                return Ok(tour);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el tour", error = ex.Message });
            }
        }

        [HttpPost("/createTour")]
        public async Task<IActionResult> Create([FromBody] TourDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                if (dto.SiteId == Guid.Empty)
                    return BadRequest("El siteId es obligatorio para vincular el tour a un sitio.");

                var newTour = await _tourService.CreateTourAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tour añadido exitosamente",
                    data = newTour
                });
            }
            catch (Exception ex)
            {
                string friendlyMessage = ex.Message.Contains("23505")
            ? "El 'slug' ya está en uso. Por favor, elige otro nombre o slug."
            : "Error al guardar el tour. Verifica que el SiteId sea correcto.";

                return BadRequest(new { success = false, message = friendlyMessage, details = ex.Message });
            }
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

