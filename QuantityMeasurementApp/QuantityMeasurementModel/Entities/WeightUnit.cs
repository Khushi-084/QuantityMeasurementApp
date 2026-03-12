using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC9: Standalone weight unit enum with conversion responsibility.
    /// Base unit is KILOGRAM. All conversions normalize through kilograms.
    /// </summary>
    public enum WeightUnit
    {
        KILOGRAM,
        GRAM,
        POUND
    }

    public static class WeightUnitExtensions
    {
        /// <summary>
        /// Conversion factor: valueInUnit * factor = valueInKilograms.
        /// </summary>
        public static double GetConversionFactor(this WeightUnit unit)
        {
            switch (unit)
            {
                case WeightUnit.KILOGRAM:
                    return 1.0;
                case WeightUnit.GRAM:
                    return 0.001;      // 1 g = 0.001 kg
                case WeightUnit.POUND:
                    return 0.453592;   // 1 lb ≈ 0.453592 kg
                default:
                    throw new ArgumentException("Unsupported weight unit.");
            }
        }

        /// <summary>
        /// Converts a value in this unit to base unit (kilograms).
        /// </summary>
        public static double ConvertToBaseUnit(this WeightUnit unit, double value)
        {
            return value * unit.GetConversionFactor();
        }

        /// <summary>
        /// Converts a value from base unit (kilograms) to this unit.
        /// </summary>
        public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue)
        {
            double factor = unit.GetConversionFactor();
            if (Math.Abs(factor) < 1e-15)
                throw new ArgumentException("Unsupported weight unit.");
            return baseValue / factor;
        }
    }
}

