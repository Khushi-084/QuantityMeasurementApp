using QuantityMeasurementApp.Abstractions;
using QuantityMeasurementApp.Domain;

namespace QuantityMeasurementApp.Service
{
    public class QuantityMeasurementService : IQuantityMeasurementService
    {

    /// <summary>
    /// Implementation of <see cref="IQuantityMeasurementService"/>.
    /// Provides methods to compare quantities like Feet, Inch, etc.
    /// </summary>
        public bool AreEqual(Feet value1, Feet value2)
        {
             // Check if either of the inputs is null to avoid NullReferenceException
            if (value1 == null || value2 == null)
                return false;

             // Use the overridden Equals method in Feet class to compare values
            return value1.Equals(value2);
        }
    }
}