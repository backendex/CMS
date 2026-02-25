using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces
{
    public interface ITourService
    {
        Task<IEnumerable<Tour>> GetTourAsync(Guid siteId);
        Task<Tour?> GetTourByIdAsync(Guid id, Guid siteId);
        Task<Tour> CreateTourAsync(TourDto tourDto);
        Task<bool> UpdateTourAsync(Guid id, TourDto tourDto);
        Task<bool> DeleteTourAsync(Guid id);
        Task<IEnumerable<ContentTypeDto>> GetContentTypesBySiteAsync(Guid siteId);
    }
}
