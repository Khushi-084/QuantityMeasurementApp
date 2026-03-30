using System;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// UC8: Extension methods giving LengthEnum conversion responsibility.
    /// ConvertToBaseUnit: value in this unit → feet.
    /// ConvertFromBaseUnit: value in feet → this unit.
    /// The LengthEnum itself lives in QuantityMeasurementModel.Entities.
    /// </summary>
    public static class LengthUnitExtensions
    {
        /// <summary>
        /// Conversion factor: valueInUnit * factor = valueInFeet.
        /// Example: INCH factor = 1/12 so 12 inches * (1/12) = 1 foot.
        /// </summary>
        public static double GetConversionFactor(this LengthEnum unit)
        {
            return unit.ToFeetFactor();
        }

        // Convert unit to FEET (base unit)
        public static double ToFeetFactor(this LengthEnum unit)
        {
            switch (unit)
            {
                case LengthEnum.FEET:
                    return 1.0;

                case LengthEnum.INCH:
                    return 1.0 / 12.0; // 12 inch = 1 foot

                case LengthEnum.YARD:
                    return 3.0; // 1 yard = 3 feet

                case LengthEnum.CENTIMETER:
                    return 0.0328084; // 1 cm = 0.0328084 feet

                default:
                    throw new ArgumentException("Unsupported unit");
            }
        }

        /// <summary>
        /// Converts a value in this unit to base unit (feet).
        /// Example: LengthEnum.INCH.ConvertToBaseUnit(12.0) → 1.0
        /// </summary>
        public static double ConvertToBaseUnit(this LengthEnum unit, double value)
        {
            return value * unit.ToFeetFactor();
        }

        /// <summary>
        /// Converts a value from base unit (feet) to this unit.
        /// Example: LengthEnum.INCH.ConvertFromBaseUnit(1.0) → 12.0
        /// </summary>
        public static double ConvertFromBaseUnit(this LengthEnum unit, double baseValue)
        {
            double factor = unit.ToFeetFactor();

            if (Math.Abs(factor) < 1e-15)
                throw new ArgumentException("Unsupported unit");

            return baseValue / factor;
        }

        // Convert unit to INCHES
        public static double ToInchesFactor(this LengthEnum unit)
        {
            switch (unit)
            {
                case LengthEnum.FEET:
                    return 12.0;

                case LengthEnum.INCH:
                    return 1.0;

                case LengthEnum.YARD:
                    return 36.0; // 1 yard = 36 inches

                case LengthEnum.CENTIMETER:
                    return 0.393701; // 1 cm = 0.393701 inch

                default:
                    throw new ArgumentException("Unsupported unit");
            }
        }
    }
}