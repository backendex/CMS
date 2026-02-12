namespace CMS.src.Application.DTOs.Content
{
    public class TourDto
    {
        public Guid? Id { get; set; } 
        public Guid SiteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string Slug { get; set; }
        public Dictionary<string, object> DynamicData { get; set; } = new(); 


    }
}
