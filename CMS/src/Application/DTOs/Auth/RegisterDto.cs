namespace CMS.src.Application.DTOs.Auth
{
    public record RegisterDto
    {
          public string Name { get; init; }
          public string LastName { get; init; }
          public string Email { get; init; }
          public int RolId { get; init; }
          public bool EmailConfirmed { get; set; }

    }
}

