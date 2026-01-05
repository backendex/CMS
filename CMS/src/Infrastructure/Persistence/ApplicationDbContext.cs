using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<AccessRole> AccessRoles => Set<AccessRole>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuración de la relación entre User y AccessRole
            modelBuilder.Entity<User>(entity => {
                entity.HasOne(d => d.Role)
                      .WithMany(p => p.Users)
                      .HasForeignKey(d => d.RolId)
                      .HasConstraintName("fk_user_role"); 
                entity.ToTable("users"); 
                entity.Property(e => e.RolId).HasColumnName("rol_id");
                // MAPEO DE LOS NUEVOS CAMPOS (Obligatorio para Postgres)
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.ValidationToken).HasColumnName("validation_token");
            });
            modelBuilder.Entity<AccessRole>(entity => {
                entity.ToTable("access_role");
                entity.Property(e => e.NameRol).HasColumnName("name_rol");
                entity.Property(e => e.CanView).HasColumnName("can_view");
                entity.Property(e => e.CanCreate).HasColumnName("can_create");
                entity.Property(e => e.CanEdit).HasColumnName("can_edit");
                entity.Property(e => e.CanDelete).HasColumnName("can_delete");
            });


        }
    }
}
