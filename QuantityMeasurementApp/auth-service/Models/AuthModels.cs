// ─────────────────────────────────────────────────────────────────────────────
// AUTH — DTOs
// ─────────────────────────────────────────────────────────────────────────────

namespace ModelService.Auth.Dto
{
    public class ApiResponse<T>
    {
        public bool     Success   { get; set; }
        public string?  Message   { get; set; }
        public T?       Data      { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message) =>
            new() { Success = false, Message = message };
    }

    public class AuthResponseDTO
    {
        public string   Token     { get; set; } = string.Empty;
        public string   Username  { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string   Role      { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class SignupRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email    { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequestDTO
    {
        public string Email    { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class GoogleLoginRequestDTO
    {
        public string IdToken { get; set; } = string.Empty;
    }

    public class UserResponseDTO
    {
        public int      Id        { get; set; }
        public string   Username  { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string   Role      { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ErrorResponseDTO
    {
        public int      Status    { get; set; }
        public string   Error     { get; set; } = string.Empty;
        public string   Message   { get; set; } = string.Empty;
        public string   Path      { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// AUTH — Entities
// ─────────────────────────────────────────────────────────────────────────────

namespace ModelService.Auth.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Users")]
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required][MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required][MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required][MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Role { get; set; } = "User";

        public DateTime  CreatedAt   { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool      IsActive    { get; set; } = true;
    }
}
