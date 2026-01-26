namespace CMS.src.Application.DTOs.Post
{
    public class CreatePostDto
    {
        public Guid SiteId { get; set; }
        public List<PostTranslationDto> Translations { get; set; } = new();
    }
}
