namespace QuantityMeasurementApp.ServiceLayer
{
    /// <summary>
    /// Service providing comparison and conversion operations for length quantities.
    /// Uses a base unit (feet) for normalization. UC5 adds explicit unit-to-unit conversion.
    /// </summary>
    public static class QuantityLengthService
    {
        /// <summary>Default epsilon for floating-point equality and precision tolerance.</summary>
        public const double DefaultEpsilon = 1e-6;

        public static bool Compare(double value1, LengthUnit unit1, double value2, LengthUnit unit2)
        {
            double inches1 = ConvertToInches(value1, unit1);
            double inches2 = ConvertToInches(value2, unit2);

            return Math.Abs(inches1 - inches2) < 0.0001;
        }

        /// <summary>
        /// Converts a numeric value from a source length unit to a target length unit.
        /// Normalizes to the common base unit (feet) then to the target unit.
        /// </summary>
        /// <param name="value">The numeric value to convert (must be finite).</param>
        /// <param name="source">The source length unit (must be a defined enum member).</param>
        /// <param name="target">The target length unit (must be a defined enum member).</param>
        /// <returns>The converted value in the target unit. Preserves sign and zero.</returns>
        /// <exception cref="ArgumentException">Thrown when value is NaN or infinite, or when source or target is not a valid LengthUnit.</exception>
        public static double Convert(double value, LengthUnit source, LengthUnit target)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number; NaN and infinity are not allowed.", nameof(value));
            if (!System.Enum.IsDefined(typeof(LengthUnit), source))
                throw new ArgumentException("Source unit must be a valid LengthUnit.", nameof(source));
            if (!System.Enum.IsDefined(typeof(LengthUnit), target))
                throw new ArgumentException("Target unit must be a valid LengthUnit.", nameof(target));

            double valueInBase = value * source.ToFeet();
            return valueInBase / target.ToFeet();
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