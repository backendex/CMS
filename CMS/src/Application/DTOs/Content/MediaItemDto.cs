namespace CMS.src.Application.DTOs.Content
{
    public class SaveMediaDto
    {
        public string Url { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string SiteId { get; set; }
    }
    public class ImageKitAuthDto
    {
        public string Token { get; set; }
        public string Expire { get; set; }
        public string Signature { get; set; }
    }
}
