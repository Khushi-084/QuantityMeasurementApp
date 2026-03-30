using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementModel.Dto
{
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
}
