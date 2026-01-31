using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces
{
    public interface ITourService
    {
        Task<List<Tour>> GetToursBySiteIdAsync(Guid siteId);
        Task<Tour?> GetTourByIdAsync(Guid id);
        Task<Tour> CreateTourAsync(TourDto tourDto);
        Task<bool> UpdateTourAsync(Guid id, TourDto tourDto);
        Task<bool> DeleteTourAsync(Guid id);
    }
}
