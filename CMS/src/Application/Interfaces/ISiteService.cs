
using CMS.src.Application.DTOs.Post;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces
{
    public interface ISiteService
    {
        Task<Site> GetByDomainAsync(string domain);

    }
}
