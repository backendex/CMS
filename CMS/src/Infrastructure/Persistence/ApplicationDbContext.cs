using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuramos los nombres exactos para evitar el error 42P01
            modelBuilder.Entity<User>().ToTable("user");

            // Si deseas mapear la tabla access_role de tu imagen:
            modelBuilder.Entity<User>()
                .Property(u => u.RolId)
                .HasColumnName("rol_id");
        }
    }
}
