using CMS.src.Domain.Entities;
using System.Text.Json.Serialization;

namespace CMS.src.Application.DTOs.Auth
{
    public class LoginResult
    {
        public int UserId { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; } 
        public string Message { get; set; }
        public bool MustChangePassword { get; set; }
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }


}
