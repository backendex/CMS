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
        public DbSet<BlogPost> BlogPost { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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
                entity.HasKey(us => new { us.UserId, us.SiteId });
                entity.Property(us => us.UserId)
                      .HasColumnName("user_id");
                entity.Property(us => us.SiteId)
                      .HasColumnName("site_id");
                entity.HasOne(us => us.User)
                      .WithMany(u => u.UserSites)
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(us => us.SiteNavigate)
                      .WithMany()
                      .HasForeignKey(us => us.SiteId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("user_sites");
            });

            modelBuilder.Entity<RolePermissions>(entity =>
            {

                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            });

            modelBuilder.Entity<Permissions>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PermissionKey)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(p => p.Description)
                      .HasMaxLength(250);
                entity.HasMany(p => p.RolePermissions)
                      .WithOne(rp => rp.Permissions)
                      .HasForeignKey(rp => rp.PermissionId);
            });
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.ToTable("blog_post");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Slug).HasColumnName("slug");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.FeaturedImage).HasColumnName("featured_image");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.IsPublished).HasColumnName("is_published");
                entity.Property(e => e.SiteId).HasColumnName("site_id");
                entity.Property(e => e.SeoData)
                  .HasColumnName("seo_data")
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                      v => System.Text.Json.JsonSerializer.Deserialize<SeoMetadata>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new SeoMetadata()
                  );
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Slug).HasColumnName("slug");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");
                entity.Property(e => e.SiteId).HasColumnName("site_id");

                entity.HasOne(d => d.ParentCategory)
                      .WithMany()
                      .HasForeignKey(d => d.ParentCategoryId)
                      .IsRequired(false) 
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}