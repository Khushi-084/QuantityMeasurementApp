using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC11: Standalone volume unit enum with conversion responsibility.
    /// Base unit is LITRE. All conversions normalize through litres.
    /// Supports: LITRE, MILLILITRE, GALLON.
    /// </summary>
   

    public static class VolumeUnitExtensions
    {
        /// <summary>
        /// Conversion factor: valueInUnit * factor = valueInLitres.
        /// Base unit: LITRE.
        /// </summary>
        public static double GetConversionFactor(this VolumeUnit unit)
        {
            switch (unit)
            {
                case VolumeUnit.LITRE:
                    return 1.0;
                case VolumeUnit.MILLILITRE:
                    return 0.001;        // 1 mL = 0.001 L
                case VolumeUnit.GALLON:
                    return 3.78541;      // 1 US gallon = 3.78541 L
                default:
                    throw new ArgumentException("Unsupported volume unit.");
            }
        }

        /// <summary>Converts a value in this unit to base unit (litres).</summary>
        public static double ConvertToBaseUnit(this VolumeUnit unit, double value)
        {
            return value * unit.GetConversionFactor();
        }

        /// <summary>Converts a value from base unit (litres) to this unit.</summary>
        public static double ConvertFromBaseUnit(this VolumeUnit unit, double baseValue)
        {
            double factor = unit.GetConversionFactor();
            if (Math.Abs(factor) < 1e-15)
                throw new ArgumentException("Unsupported volume unit.");
            return baseValue / factor;
        }
    }
}
