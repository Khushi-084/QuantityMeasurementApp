using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>UC17: Input DTO for unit conversion.</summary>
    public class ConvertRequestDTO
    {
        [Required(ErrorMessage = "ThisQuantityDTO is required.")]
        public QuantityDTO ThisQuantityDTO { get; set; } = null!;

        [Required(ErrorMessage = "TargetUnit is required.")]
        public string TargetUnit { get; set; } = string.Empty;
    }
}
