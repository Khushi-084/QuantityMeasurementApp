using QuantityMeasurementApp.DomainLayer;

namespace QuantityMeasurementApp.ServiceLayer
{
    /// <summary>
    /// Handles UC-1 logic between Presentation and Domain.
    /// </summary>
    public class QuantityMeasurementService
    {
        public bool CompareFeet(double value1, double value2)
        {
            Feet feet1 = new Feet(value1);
            Feet feet2 = new Feet(value2);

            return feet1.Equals(feet2);
        }
    }
}