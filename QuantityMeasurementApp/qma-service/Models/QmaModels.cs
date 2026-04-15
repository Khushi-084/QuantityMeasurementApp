// ─────────────────────────────────────────────────────────────────────────────
// QMA — DTOs
// ─────────────────────────────────────────────────────────────────────────────

namespace ModelService.Qma.Dto
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

    public class QuantityDTO
    {
        public double Value    { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class QuantityInputDTO
    {
        public QuantityDTO ThisQuantityDTO { get; set; } = null!;
        public QuantityDTO ThatQuantityDTO { get; set; } = null!;
    }

    public class ConvertRequestDTO
    {
        public QuantityDTO ThisQuantityDTO { get; set; } = null!;
        public string      TargetUnit      { get; set; } = string.Empty;
    }

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

        public static QuantityMeasurementDTO FromEntity(ModelService.Qma.Entities.QmaMeasurementEntity e) => new()
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

        public static List<QuantityMeasurementDTO> FromEntityList(
            IEnumerable<ModelService.Qma.Entities.QmaMeasurementEntity> entities)
            => entities.Select(FromEntity).ToList();
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
// QMA — Entities
// ─────────────────────────────────────────────────────────────────────────────

namespace ModelService.Qma.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QuantityMeasurements")]
    public class QmaMeasurementEntity
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required][MaxLength(50)]
        public string OperationType { get; set; } = string.Empty;

        [MaxLength(50)] public string? MeasurementCategory { get; set; }
        public double? Operand1Value { get; set; }
        [MaxLength(50)] public string? Operand1Unit { get; set; }
        public double? Operand2Value { get; set; }
        [MaxLength(50)] public string? Operand2Unit { get; set; }
        public double? ResultValue   { get; set; }
        [MaxLength(50)] public string? ResultUnit    { get; set; }
        [MaxLength(50)] public string? ResultCategory { get; set; }
        public bool    HasError      { get; set; }
        [MaxLength(500)] public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
