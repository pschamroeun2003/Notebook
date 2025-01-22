using System.ComponentModel.DataAnnotations; // Add this line

namespace NoteTakingApp.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } // Use Email for login

        [Required]
        [MinLength(6)] // You can set a minimum length for the password
        public string Password { get; set; } // Use Password instead of PasswordHash for login
    }

    public class UserRegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)] // You can set a minimum length for the password
        public string Password { get; set; } // Use Password instead of PasswordHash for registration
    }

    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}