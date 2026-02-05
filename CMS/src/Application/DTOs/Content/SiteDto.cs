namespace CMS.src.Application.DTOs.Content
{
    public class SiteDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }
        public bool IsMaintenance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
