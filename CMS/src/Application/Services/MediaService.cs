using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using System.Security.Cryptography;
using System.Text;

namespace CMS.src.Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _config;

        public MediaService(IApplicationDbContext context, IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public ImageKitAuthDto GetImageKitAuth()
        {
            var privateKey = _config["ImageKit:PrivateKey"]
                ?? throw new InvalidOperationException("La clave privada de ImageKit no está configurada.");

            var token = Guid.NewGuid().ToString();
            var expire = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString();

            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(privateKey));
            var signature = BitConverter.ToString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(token + expire))
            ).Replace("-", "").ToLower();

            return new ImageKitAuthDto
            {
                Token = token,
                Expire = expire,
                Signature = signature
            };
        }

        public async Task<MediaItem> SaveMediaAsync(SaveMediaDto dto)
        {
            var media = new MediaItem
            {
                Url = dto.Url,
                FileId = dto.FileId,
                FileName = dto.FileName,
                FileType = dto.FileType,
                SiteId = dto.SiteId,
                CreatedAt = DateTime.UtcNow
            };

            _context.MediaItems.Add(media);
            await _context.SaveChangesAsync();
            return media;
        }

        public async Task<IEnumerable<MediaItem>> GetMediaBySiteAsync(string siteId)
        {
            return await _context.MediaItems
                .Where(m => m.SiteId == siteId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteMediaAsync(int id)
        {
            var media = await _context.MediaItems.FindAsync(id);
            if (media == null) throw new KeyNotFoundException("Media no encontrado.");

            _context.MediaItems.Remove(media);
            await _context.SaveChangesAsync();
        }
    }
}