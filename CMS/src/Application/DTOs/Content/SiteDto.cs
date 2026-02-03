namespace CMS.src.Application.DTOs.Content
{
    public class SiteDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
