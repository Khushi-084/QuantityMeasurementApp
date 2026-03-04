using QuantityMeasurementApp.Domain;

namespace QuantityMeasurementApp.ServiceLayer
{
    /// <summary>
    /// Service providing comparison and conversion operations for length quantities.
    /// UC8: Delegates conversion to LengthUnit.ConvertToBaseUnit/ConvertFromBaseUnit.
    /// </summary>
    public static class QuantityLengthService
    {
        /// <summary>Default epsilon for floating-point equality and precision tolerance.</summary>
        public const double DefaultEpsilon = 1e-6;

        public static bool Compare(double value1, LengthUnit unit1, double value2, LengthUnit unit2)
        {
            double base1 = unit1.ConvertToBaseUnit(value1);
            double base2 = unit2.ConvertToBaseUnit(value2);
            return Math.Abs(base1 - base2) < 0.0001;
        }

        /// <summary>
        /// Converts a numeric value from a source length unit to a target length unit.
        /// UC8: Delegates to unit conversion methods.
        /// </summary>
        public static double Convert(double value, LengthUnit source, LengthUnit target)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number; NaN and infinity are not allowed.", nameof(value));
            if (!System.Enum.IsDefined(typeof(LengthUnit), source))
                throw new ArgumentException("Source unit must be a valid LengthUnit.", nameof(source));
            if (!System.Enum.IsDefined(typeof(LengthUnit), target))
                throw new ArgumentException("Target unit must be a valid LengthUnit.", nameof(target));

            double valueInBase = source.ConvertToBaseUnit(value);
            return target.ConvertFromBaseUnit(valueInBase);
        }
    }
}