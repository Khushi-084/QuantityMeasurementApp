using System.ComponentModel.DataAnnotations;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementModel.Dto
{
    // ═══════════════════════════════════════════════════════════════════════
    // UC17 REQUEST DTOs
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// UC17: Input DTO for binary operations (compare, add, subtract, divide).
    /// Wraps two QuantityDTOs — ThisQuantityDTO and ThatQuantityDTO.
    /// Preserves the existing QuantityDTO class unchanged from UC16.
    /// </summary>
    public class QuantityInputDTO
    {
        [Required(ErrorMessage = "ThisQuantityDTO is required.")]
        public QuantityDTO ThisQuantityDTO { get; set; } = null!;

        [Required(ErrorMessage = "ThatQuantityDTO is required.")]
        public QuantityDTO ThatQuantityDTO { get; set; } = null!;
    }

    /// <summary>UC17: Input DTO for unit conversion.</summary>
    public class ConvertRequestDTO
    {
        [Required(ErrorMessage = "ThisQuantityDTO is required.")]
        public QuantityDTO ThisQuantityDTO { get; set; } = null!;

        [Required(ErrorMessage = "TargetUnit is required.")]
        public string TargetUnit { get; set; } = string.Empty;
    }

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

    /// <summary>UC17: Login request DTO.</summary>
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // ═══════════════════════════════════════════════════════════════════════
    // UC17 RESPONSE DTOs
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// UC17: Response DTO for quantity measurement operations.
    /// Contains static factory methods for clean Entity → DTO conversion.
    /// </summary>
    public class QuantityMeasurementDTO
    {
        public int     Id                  { get; set; }
        public string  OperationType       { get; set; } = string.Empty;
        public string? MeasurementCategory { get; set; }
        public double? Operand1Value       { get; set; }
        public string? Operand1Unit        { get; set; }
        public double? Operand2Value       { get; set; }
        public string? Operand2Unit        { get; set; }
        public double? ResultValue         { get; set; }
        public string? ResultUnit          { get; set; }
        public string? ResultCategory      { get; set; }
        public bool    HasError            { get; set; }
        public string? ErrorMessage        { get; set; }
        public DateTime CreatedAt          { get; set; }

        // ── Static factory: Entity → DTO ──────────────────────────────────
        public static QuantityMeasurementDTO FromEntity(QuantityMeasurementApiEntity e) => new()
        {
            Id                  = e.Id,
            OperationType       = e.OperationType,
            MeasurementCategory = e.MeasurementCategory,
            Operand1Value       = e.Operand1Value,
            Operand1Unit        = e.Operand1Unit,
            Operand2Value       = e.Operand2Value,
            Operand2Unit        = e.Operand2Unit,
            ResultValue         = e.ResultValue,
            ResultUnit          = e.ResultUnit,
            ResultCategory      = e.ResultCategory,
            HasError            = e.HasError,
            ErrorMessage        = e.ErrorMessage,
            CreatedAt           = e.CreatedAt
        };

        // ── Static factory: List<Entity> → List<DTO> ──────────────────────
        public static List<QuantityMeasurementDTO> FromEntityList(
            IEnumerable<QuantityMeasurementApiEntity> entities)
            => entities.Select(FromEntity).ToList();

        // ── DTO → Entity ──────────────────────────────────────────────────
        public QuantityMeasurementApiEntity ToEntity() => new()
        {
            OperationType       = OperationType,
            MeasurementCategory = MeasurementCategory,
            Operand1Value       = Operand1Value,
            Operand1Unit        = Operand1Unit,
            Operand2Value       = Operand2Value,
            Operand2Unit        = Operand2Unit,
            ResultValue         = ResultValue,
            ResultUnit          = ResultUnit,
            ResultCategory      = ResultCategory,
            HasError            = HasError,
            ErrorMessage        = ErrorMessage
        };
    }

    /// <summary>UC17: Standard API response envelope.</summary>
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

    /// <summary>UC17: Auth response carrying JWT token and user info.</summary>
    public class AuthResponseDTO
    {
        public string   Token     { get; set; } = string.Empty;
        public string   Username  { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string   Role      { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>UC17: User profile response DTO.</summary>
    public class UserResponseDTO
    {
        public int      Id        { get; set; }
        public string   Username  { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string   Role      { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>UC17: Structured error response from GlobalExceptionMiddleware.</summary>
    public class ErrorResponseDTO
    {
        public int      Status    { get; set; }
        public string   Error     { get; set; } = string.Empty;
        public string   Message   { get; set; } = string.Empty;
        public string   Path      { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
