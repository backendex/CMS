using CMS.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.src.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<AccessRole> AccessRoles { get; }
        DbSet<Site> Sites { get; }
        DbSet<Page> Pages { get; }
        DbSet<PageTranslation> PageTranslations { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<MediaItem> MediaItems { get; }
    }
}
