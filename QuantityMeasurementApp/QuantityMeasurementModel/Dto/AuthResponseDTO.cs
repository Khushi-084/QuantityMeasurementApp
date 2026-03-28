namespace QuantityMeasurementModel.Dto
{
    /// <summary>UC17: Auth response carrying JWT token and user info.</summary>
    public class AuthResponseDTO
    {
        public string   Token     { get; set; } = string.Empty;
        public string   Username  { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string   Role      { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
