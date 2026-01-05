namespace CMS.src.Application.DTOs.Auth
{
    public record RegisterDto
    {
      public string Email { get; init; }
      public string Password { get; init; }
      public int RolId { get; init; }

    }
}

