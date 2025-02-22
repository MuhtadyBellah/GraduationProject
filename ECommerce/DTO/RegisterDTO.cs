using ECommerce.Core.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&;*()_+]).{8,}$",
            ErrorMessage = "Pa$$w0rd must Contains 1 LowerCase, 1 UpperCase, 1 Digit, 1 SpecialCharacter, At Least 8 Characters")]
        public string Password { get; set; }
    }
}
