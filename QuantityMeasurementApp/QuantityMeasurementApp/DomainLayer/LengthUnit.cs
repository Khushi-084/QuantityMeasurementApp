namespace QuantityMeasurementApp.ServiceLayer
{
    public enum LengthUnit
    {
        Feet,
        Inch,
        Yard,
        Centimeter
    }

    public static class LengthUnitExtensions
    {
        // Base unit = Feet
        public static double ToFeet(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.Feet => 1.0,
                LengthUnit.Inch => 1.0 / 12.0,           // 12 inches = 1 foot
                LengthUnit.Yard => 3.0,                 // 1 yard = 3 feet
                LengthUnit.Centimeter => (0.393701 / 12.0), 
                // 1 cm = 0.393701 inches
                // convert inches to feet => divide by 12

                _ => throw new System.ArgumentException("Unsupported unit")
            };
        }
    }
}