namespace CMS.src.Application.DTOs.Content
{
    public class SiteAccessDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public List<SiteDto> AllowedSites { get; set; }
    }
}
