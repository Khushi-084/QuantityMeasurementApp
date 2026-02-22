namespace QuantityMeasurementApp.ServiceLayer
{
    

    public static class QuantityLengthService
    {
        public static bool Compare(double value1, LengthUnit unit1, double value2, LengthUnit unit2)
        {
            double inches1 = ConvertToInches(value1, unit1);
            double inches2 = ConvertToInches(value2, unit2);

            return Math.Abs(inches1 - inches2) < 0.0001;
        }

        private static double ConvertToInches(double value, LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.Feet => value * 12,
                LengthUnit.Inch => value,
                LengthUnit.Yard => value * 36,
                LengthUnit.Centimeter => value / 2.54,
                _ => throw new ArgumentException("Invalid unit")
            };
        }
    }
}