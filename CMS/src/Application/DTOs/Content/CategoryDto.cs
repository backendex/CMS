using CMS.src.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Application.DTOs.Content
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public Guid SiteId { get; set; }
        public Category? ParentCategory { get; set; }
    }
}
