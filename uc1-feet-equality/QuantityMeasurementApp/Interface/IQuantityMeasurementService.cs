using QuantityMeasurementApp.Domain;

namespace QuantityMeasurementApp.Abstractions
{
    /// <summary>
    /// Interface for Quantity Measurement Service.
    /// Defines operations for comparing quantities like Feet, Inch, etc.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        bool AreEqual(Feet value1, Feet value2);
    }
}