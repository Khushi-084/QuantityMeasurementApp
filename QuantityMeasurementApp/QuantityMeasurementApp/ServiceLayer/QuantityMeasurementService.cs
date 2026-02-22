using QuantityMeasurementApp.DomainLayer;

namespace QuantityMeasurementApp.ServiceLayer
{
    public static class QuantityMeasurementService
    {
        // UC-1 → Compare Feet
        public static bool CompareFeet(double value1, double value2)
        {
            Feet feet1 = new Feet(value1);
            Feet feet2 = new Feet(value2);

            return feet1.Equals(feet2);
        }

        // UC-2 → Compare Inches
        public static bool CompareInches(double value1, double value2)
        {
            Inches inches1 = new Inches(value1);
            Inches inches2 = new Inches(value2);

            return inches1.Equals(inches2);
        }
    }
}