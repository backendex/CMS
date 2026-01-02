using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyCMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Es vital mantener esta línea para que Identity configure sus tablas internas
            base.OnModelCreating(builder);

            // Ejemplo: Personalizar nombres de tablas (opcional)
            // Por defecto son AspNetUsers, AspNetRoles, etc.
            builder.Entity<User>(entity => { entity.ToTable(name: "Users"); });

            // Aquí puedes configurar relaciones adicionales para tu CMS
            // Ejemplo: Un usuario tiene muchas actividades
            // builder.Entity<Activity>()
            //    .HasOne(a => a.User)
            //    .WithMany(u => u.Activities)
            //    .HasForeignKey(a => a.UserId);
        }

        // Tus tablas del CMS irán apareciendo aquí
        // public DbSet<Page> Pages { get; set; }
        // public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}
