using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security;

namespace CMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<AccessRole> AccessRoles => Set<AccessRole>();

        public object PostTranslations { get; internal set; }
        public object Sites { get; internal set; }
        public DbSet<SiteContent> SiteContents { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<MediaContent> Media { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuración de la relación entre User y AccessRole
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.AccessRole)
                      .WithMany(p => p.Users)
                      .HasForeignKey(d => d.RolId)
                      .HasConstraintName("fk_user_role");
                entity.ToTable("users");
                entity.Property(e => e.RolId).HasColumnName("rol_id");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.ValidationToken).HasColumnName("validation_token");
            });
            modelBuilder.Entity<AccessRole>(entity =>
            {
                entity.ToTable("access_role");
                entity.Property(e => e.NameRol).HasColumnName("name_rol");
                entity.Property(e => e.CanView).HasColumnName("can_view");
                entity.Property(e => e.CanCreate).HasColumnName("can_create");
                entity.Property(e => e.CanEdit).HasColumnName("can_edit");
                entity.Property(e => e.CanDelete).HasColumnName("can_delete");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            });

            modelBuilder.Entity<UserSite>(entity =>
            {
                // 1. Definir la Llave Primaria Compuesta
                entity.HasKey(us => new { us.UserId, us.SiteId });

                // 2. Mapeo de columnas EXACTO para Postgres (Esto quita el error 42703)
                entity.Property(us => us.UserId)
                      .HasColumnName("user_id"); // O "user_id" según tu DB

                entity.Property(us => us.SiteId)
                      .HasColumnName("site_id"); // O "site_id" según tu DB

                // 3. Relación con Usuario
                entity.HasOne(us => us.User)
                      .WithMany(u => u.UserSites)
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 4. Relación con Sitio
                entity.HasOne(us => us.SiteNavigate)
                      .WithMany()
                      .HasForeignKey(us => us.SiteId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("user_sites");
            });

            modelBuilder.Entity<RolePermissions>(entity =>
            {
                // PK compuesta
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                // Relaciones definidas arriba con HasMany/WithOne
            });

            modelBuilder.Entity<Permissions>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PermissionKey)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(p => p.Description)
                      .HasMaxLength(250);

                // Relación muchos a muchos con Role a través de RolePermission
                entity.HasMany(p => p.RolePermissions)
                      .WithOne(rp => rp.Permissions)
                      .HasForeignKey(rp => rp.PermissionId);
            });

        }

        internal async Task<int> SaveChangeAsync()
        {
            throw new NotImplementedException();
        }

        internal async Task<int> saveChangeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
