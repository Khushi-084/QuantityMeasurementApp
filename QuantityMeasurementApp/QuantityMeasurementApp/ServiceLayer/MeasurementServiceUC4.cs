using QuantityMeasurementApp.DomainLayer;

namespace QuantityMeasurementApp.ServiceLayer
{
    // Service for UC4: Adding Feet and Inches
    public class MeasurementServiceUC4
    {
        // Method to add Feet and Inches
        public Inches Add(Feet feet, Inches inches)
        {
            // Convert Feet to Inches and add
            double totalInches = (feet.Value * 12) + inches.Value;
            return new Inches(totalInches);
        }
    }
}