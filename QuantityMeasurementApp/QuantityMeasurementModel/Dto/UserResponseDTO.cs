namespace QuantityMeasurementModel.Dto
{
    /// <summary>UC17: User profile response DTO.</summary>
    public class UserResponseDTO
    {
        public int      Id        { get; set; }
        public string   Username  { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string   Role      { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
