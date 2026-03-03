namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC8: Standalone length unit enum with conversion responsibility.
    /// Base unit is Feet. Manages conversions to and from base unit.
    /// </summary>
    public enum LengthUnit
    {
        Feet,
        Inch,
        Yard,
        Centimeter
    }

    /// <summary>
    /// UC8: Extension methods for LengthUnit providing conversion responsibility.
    /// convertToBaseUnit: value in this unit → feet.
    /// convertFromBaseUnit: feet → value in this unit.
    /// </summary>
    public static class LengthUnitExtensions
    {
        /// <summary>Conversion factor: feet per 1 unit. Used for convertToBaseUnit.</summary>
        public static double GetConversionFactor(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.Feet => 1.0,
                LengthUnit.Inch => 1.0 / 12.0,           // 12 inches = 1 foot
                LengthUnit.Yard => 3.0,                  // 1 yard = 3 feet
                LengthUnit.Centimeter => 1.0 / 30.48,    // 1 foot = 30.48 cm
                _ => throw new System.ArgumentException("Unsupported unit", nameof(unit))
            };
        }

        /// <summary>Legacy alias for GetConversionFactor. Base unit = Feet.</summary>
        public static double ToFeet(this LengthUnit unit) => unit.GetConversionFactor();

        /// <summary>UC8: Converts a value in this unit to feet (base unit).</summary>
        /// <param name="unit">The source unit.</param>
        /// <param name="value">Value expressed in this unit.</param>
        /// <returns>Value in feet.</returns>
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            return value * unit.GetConversionFactor();
        }

        /// <summary>UC8: Converts a value from feet (base unit) to this unit.</summary>
        /// <param name="unit">The target unit.</param>
        /// <param name="baseValue">Value in feet.</param>
        /// <returns>Value expressed in this unit.</returns>
        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
        {
            return baseValue / unit.GetConversionFactor();
        }
    }
}