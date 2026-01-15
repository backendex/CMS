using CMS.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.src.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<AccessRole> AccessRoles { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
