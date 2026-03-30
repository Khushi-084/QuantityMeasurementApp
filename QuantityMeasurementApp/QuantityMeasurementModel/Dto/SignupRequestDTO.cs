using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>UC17: Signup request DTO.</summary>
    public class SignupRequestDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit and special character.")]
        public string Password { get; set; } = string.Empty;
    }
}
