using QuantityMeasurementApp.DomainLayer;

namespace QuantityMeasurementApp.ServiceLayer
{
    public static class QuantityMeasurementService
    {
        // ---------------- UC1 ----------------
        public static bool CompareFeet(double value1, double value2)
        {
            Feet feetNumber1 = new Feet(value1);
            Feet feetNUmber2 = new Feet(value2);

            return feetNumber1.Equals(feetNUmber2);
        }

        // ---------------- UC2 (Inches Only) ----------------
        public static bool CompareInches(double value1, double value2)
        {
            Inches inchesNumber1 = new Inches(value1);
            Inches inchesNumber2 = new Inches(value2);

            return inchesNumber1.Equals(inchesNumber2);
        }

        // ---------------- UC2 (Feet vs Inches) ----------------
        public static bool CompareFeetAndInches(double feetValue, double inchValue)
        {
            Feet feet = new Feet(feetValue);
            Inches inches = new Inches(inchValue);

            return feet.Equals(inches);
        }
    }
}