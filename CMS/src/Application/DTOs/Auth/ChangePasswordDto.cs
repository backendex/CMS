using System.ComponentModel.DataAnnotations;

namespace CMS.src.Application.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }


}
