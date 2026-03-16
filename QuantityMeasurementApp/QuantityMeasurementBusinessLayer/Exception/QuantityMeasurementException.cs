namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC15: Custom unchecked exception for all quantity measurement domain errors.
    /// Wraps underlying exceptions with domain-meaningful messages.
    /// </summary>
    public class QuantityMeasurementException : Exception
    {
        public QuantityMeasurementException(string message) : base(message) { }
        public QuantityMeasurementException(string message, Exception inner) : base(message, inner) { }
    }
}
