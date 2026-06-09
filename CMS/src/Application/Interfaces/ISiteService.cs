
using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.src.Application.Interfaces
{
    public interface ISiteService
    {
        Task<Site> GetByDomainAsync(string domain);
        Task<SiteAccessDto> GetUserAccessAsync(int userId);
        Task<IEnumerable<Site>> GetAllSitesAsync();
        Task<Site?> GetByIdAsync(Guid id);
        Task<Site> CreateSiteAsync(SiteDto dto, int userId);
        Task<Site> UpdateSiteAsync(Guid id, SiteDto dto);
        Task DeleteSiteAsync(Guid id);
    }
}
