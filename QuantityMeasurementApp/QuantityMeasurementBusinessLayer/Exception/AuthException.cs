namespace QuantityMeasurementBusinessLayer.Exception
{
    /// <summary>UC17: Authentication failure. Carries HTTP status code (401/403/409).</summary>
    public class AuthException : System.Exception
    {
        public int StatusCode { get; }
        public AuthException(string message, int statusCode = 401) : base(message)
            => StatusCode = statusCode;
    }

    /// <summary>UC17: Resource not found — maps to HTTP 404.</summary>
    public class NotFoundException : System.Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
