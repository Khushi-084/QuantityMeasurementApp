namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC15: Custom unchecked exception for all quantity measurement domain errors.
    /// Wraps underlying exceptions with domain-meaningful messages.
    /// NOTE: Uses System.Exception explicitly because the folder name "Exception"
    /// creates a child namespace that shadows the System.Exception type.
    /// </summary>
    public class QuantityMeasurementException : System.Exception
    {
        public QuantityMeasurementException(string message) : base(message) { }
        public QuantityMeasurementException(string message, System.Exception inner) : base(message, inner) { }
    }
}