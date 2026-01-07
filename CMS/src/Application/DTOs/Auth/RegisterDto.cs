namespace CMS.src.Application.DTOs.Auth
{
    public record RegisterDto
    {
      public string Email { get; init; }
      public int RolId { get; init; }
      public string FullName { get; init; }
      public bool EmailConfirmed { get; set; } = false;

    }
}

