namespace CMS.src.Application.DTOs.Auth
{
    public class UserMethods
    {
        public class UpdateUserDto
        {
            public string FullName { get; set; } = null!;
            public int RoleId { get; set; }
        }

        public class UserResponseDto
        {
            public int Id { get; set; }
            public string Email { get; set; } = null!;
            public string FullName { get; set; } = null!;
            public int RoleId { get; set; }
        }
    }
}
