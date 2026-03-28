namespace QuantityMeasurementModel.Dto
{
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
