using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>
    /// UC17: Input DTO for binary operations (compare, add, subtract, divide).
    /// Wraps two QuantityDTOs — ThisQuantityDTO and ThatQuantityDTO.
    /// </summary>
    public class QuantityInputDTO
    {
        [Required(ErrorMessage = "ThisQuantityDTO is required.")]
        public QuantityDTO ThisQuantityDTO { get; set; } = null!;

        [Required(ErrorMessage = "ThatQuantityDTO is required.")]
        public QuantityDTO ThatQuantityDTO { get; set; } = null!;
    }
}
