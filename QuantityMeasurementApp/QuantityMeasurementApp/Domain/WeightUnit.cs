using System;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC9: Standalone weight unit enum with conversion responsibility.
    /// Base unit is Kilogram. Manages conversions to and from base unit.
    /// </summary>
    public enum WeightUnit
    {
        Kilogram,
        Gram,
        Pound
    }

    /// <summary>
    /// UC9: Extension methods for WeightUnit providing conversion responsibility.
    /// convertToBaseUnit: value in this unit → kilograms.
    /// convertFromBaseUnit: kilograms → value in this unit.
    /// </summary>
    public static class WeightUnitExtensions
    {
        /// <summary>Conversion factor: kilograms per 1 unit. Used for ConvertToBaseUnit.</summary>
        public static double GetConversionFactor(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.Kilogram => 1.0,
                WeightUnit.Gram => 0.001,           // 1 g = 0.001 kg
                WeightUnit.Pound => 0.453592,       // 1 lb ≈ 0.453592 kg
                _ => throw new ArgumentException("Unsupported weight unit", nameof(unit))
            };
        }

        /// <summary>UC9: Converts a value in this unit to kilograms (base unit).</summary>
        /// <param name="unit">The source unit.</param>
        /// <param name="value">Value expressed in this unit.</param>
        /// <returns>Value in kilograms.</returns>
        public static double ConvertToBaseUnit(this WeightUnit unit, double value)
        {
            return value * unit.GetConversionFactor();
        }

        /// <summary>UC9: Converts a value from kilograms (base unit) to this unit.</summary>
        /// <param name="unit">The target unit.</param>
        /// <param name="baseValue">Value in kilograms.</param>
        /// <returns>Value expressed in this unit.</returns>
        public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue)
        {
            return baseValue / unit.GetConversionFactor();
        }
    }
}

