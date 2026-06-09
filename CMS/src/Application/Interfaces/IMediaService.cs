using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces
{
    public interface IMediaService
    {
        ImageKitAuthDto GetImageKitAuth();

        Task<MediaItem> SaveMediaAsync(SaveMediaDto dto);

        Task<IEnumerable<MediaItem>> GetMediaBySiteAsync(string siteId);

        Task DeleteMediaAsync(int id);
    }
}
