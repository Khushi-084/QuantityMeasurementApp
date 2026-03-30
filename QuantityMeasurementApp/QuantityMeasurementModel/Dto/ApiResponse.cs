namespace QuantityMeasurementModel.Dto
{
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
}
