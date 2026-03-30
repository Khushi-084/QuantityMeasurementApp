using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC17: EF Core entity for the QuantityMeasurements table.
    /// Used by the Web API repository (EF Core) — separate from UC16's
    /// ADO.NET QuantityMeasurementEntity which is preserved unchanged.
    /// </summary>
    [Table("QuantityMeasurements")]
    public class QuantityMeasurementApiEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OperationType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MeasurementCategory { get; set; }

        public double? Operand1Value    { get; set; }
        [MaxLength(50)]
        public string? Operand1Unit     { get; set; }

        public double? Operand2Value    { get; set; }
        [MaxLength(50)]
        public string? Operand2Unit     { get; set; }

        public double? ResultValue      { get; set; }
        [MaxLength(50)]
        public string? ResultUnit       { get; set; }
        [MaxLength(50)]
        public string? ResultCategory   { get; set; }

        public bool    HasError         { get; set; }
        [MaxLength(500)]
        public string? ErrorMessage     { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
