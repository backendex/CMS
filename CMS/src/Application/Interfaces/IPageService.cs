using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.src.Application.Interfaces
{
    public interface IPageService
    {
        Task<IEnumerable<Page>> GetPagesBySiteAsync(Guid siteId);
        Task<Page?> GetPageByIdAsync(Guid id);
        Task<Page> CreatePageAsync(Guid siteId, string slug, string defaultTitle);
        Task<Page> UpdatePageAsync(Guid id, PageSaveDto dto);
        Task<Page> DeletePageAsync(Guid id);
        Task<Page?> GetPageByDomainAndSlugAsync(string domain, string slug);
        Task<IEnumerable<Page>> GetPublicPagesByDomainAsync(string domain);
    }
}
