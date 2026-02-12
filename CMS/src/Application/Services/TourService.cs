using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CMS.src.Application.Services
{
    public class TourService : ITourService
    {
        private readonly ApplicationDbContext _context;

        public TourService(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("site/{siteId}")]
        public async Task<List<Tour>> GetToursBySiteIdAsync(Guid siteId)
        {
            var tour = await _context.Tours
                .Where(t => t.SiteId == siteId)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return Ok(tour);

        }
        private List<Tour> Ok(List<Tour> tour)
        {
            throw new NotImplementedException();
        }

        public async Task<Tour?> GetTourByIdAsync(Guid Id)
        {
            return await _context.Tours.FindAsync(Id);
        }

        public async Task<Tour> CreateTourAsync(TourDto tourDto)
        {

            var tour = new Tour
            {
                SiteId = tourDto.SiteId,
                Name = tourDto.Name,
                Description = tourDto.Description,
                Price = tourDto.Price,
                Category = tourDto.Category,
                IsActive = tourDto.IsActive,
                SeoTitle = tourDto.SeoTitle,
                SeoDescription = tourDto.SeoDescription,
                Slug = tourDto.Slug,               

            };

            tour.DynamicData = JsonDocument.Parse(JsonSerializer.Serialize(tourDto.DynamicData ?? new Dictionary<string, object>()));

            _context.Tours.Add(tour);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Error de base de datos: {message}");
            }
            return tour;
        }

        public async Task<bool> UpdateTourAsync(Guid id, TourDto tourDto)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return false;

            tour.Name = tourDto.Name;
            tour.Description = tourDto.Description;
            tour.Price = tourDto.Price;
            tour.Category = tourDto.Category;
            tour.IsActive = tourDto.IsActive;
            tour.SeoTitle = tourDto.SeoTitle;
            tour.SeoDescription = tourDto.SeoDescription;
            tour.Slug = tourDto.Slug;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTourAsync(Guid id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return false;

            _context.Tours.Remove(tour);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
