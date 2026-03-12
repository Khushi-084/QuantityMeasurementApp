using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC8: Standalone length unit enum with conversion responsibility.
    /// Base unit is FEET. All conversions normalize through feet.
    /// </summary>
    public enum LengthUnit
    {
        FEET,
        INCH,
        YARD,
        CENTIMETER
    }

    /// <summary>
    /// UC8: Extension methods giving LengthUnit responsibility for unit conversions.
    /// convertToBaseUnit: value in this unit → feet.
    /// convertFromBaseUnit: value in feet → this unit.
    /// </summary>
    public static class LengthUnitExtensions
    {
        /// <summary>
        /// Conversion factor: valueInUnit * factor = valueInFeet.
        /// E.g. INCH factor = 1/12 so 12 inches * (1/12) = 1 foot.
        /// </summary>
        public static double GetConversionFactor(this LengthUnit unit)
        {
            return unit.ToFeetFactor();
        }

        // Convert unit to FEET (base unit) - factor such that value * factor = feet
        public static double ToFeetFactor(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return 1.0;

                case LengthUnit.INCH:
                    return 1.0 / 12.0; // 12 inch = 1 foot

                case LengthUnit.YARD:
                    return 3.0; // 1 yard = 3 feet

                case LengthUnit.CENTIMETER:
                    return 0.0328084; // 1 cm = 0.0328084 feet

                default:
                    throw new ArgumentException("Unsupported unit");
            }
        }

        /// <summary>
        /// UC8: Converts a value in this unit to base unit (feet).
        /// Example: LengthUnit.INCH.ConvertToBaseUnit(12.0) → 1.0
        /// </summary>
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            return value * unit.ToFeetFactor();
        }

        /// <summary>
        /// UC8: Converts a value from base unit (feet) to this unit.
        /// Example: LengthUnit.INCH.ConvertFromBaseUnit(1.0) → 12.0
        /// </summary>
        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
        {
            double factor = unit.ToFeetFactor();
            if (Math.Abs(factor) < 1e-15)
                throw new ArgumentException("Unsupported unit");
            return baseValue / factor;
        }

        // Convert unit to INCHES (needed by QuantityLength / legacy)
        public static double ToInchesFactor(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return 12.0;

                case LengthUnit.INCH:
                    return 1.0;

                case LengthUnit.YARD:
                    return 36.0; // 1 yard = 36 inches

                case LengthUnit.CENTIMETER:
                    return 0.393701; // 1 cm = 0.393701 inch

                default:
                    throw new ArgumentException("Unsupported unit");
            }
        }
    }
}
