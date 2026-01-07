namespace CMS.src.Application.DTOs.Auth
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } 
        public bool EmailConfirmed { get; set; }  
        public DateTime CreatedAt { get; set; }
    }
}
