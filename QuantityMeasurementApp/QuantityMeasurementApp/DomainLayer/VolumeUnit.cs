using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC11: VolumeUnit enum for volume measurements.
    /// Base unit is Litre. Supports Litre, Millilitre, and Gallon.
    /// Follows the identical pattern as LengthUnit and WeightUnit.
    /// No changes to Quantity&lt;T&gt;, IMeasurable, or QuantityMeasurementApp required.
    /// </summary>
    public enum VolumeUnit
    {
        Litre,
        Millilitre,
        Gallon
    }

    /// <summary>
    /// UC11: Extension methods for VolumeUnit implementing the IMeasurable contract.
    /// Base unit: Litre.
    /// Conversion factors:
    ///   Litre      → 1.0        (base unit)
    ///   Millilitre → 0.001      (1 mL = 0.001 L)
    ///   Gallon     → 3.78541    (1 US gallon ≈ 3.78541 L)
    /// </summary>
    public static class VolumeUnitExtensions
    {
        /// <summary>Conversion factor: litres per 1 unit of this volume.</summary>
        public static double GetConversionFactor(this VolumeUnit unit)
        {
            return unit switch
            {
                VolumeUnit.Litre      => 1.0,
                VolumeUnit.Millilitre => 0.001,
                VolumeUnit.Gallon     => 3.78541,
                _ => throw new ArgumentException("Unsupported volume unit", nameof(unit))
            };
        }

        /// <summary>UC11 / IMeasurable: Converts a value in this unit to litres (base unit).</summary>
        public static double ConvertToBaseUnit(this VolumeUnit unit, double value)
            => value * unit.GetConversionFactor();

        /// <summary>UC11 / IMeasurable: Converts a value from litres (base unit) to this unit.</summary>
        public static double ConvertFromBaseUnit(this VolumeUnit unit, double baseValue)
            => baseValue / unit.GetConversionFactor();

        /// <summary>UC11 / IMeasurable: Returns a readable unit name.</summary>
        public static string GetUnitName(this VolumeUnit unit) => unit.ToString().ToUpper();

        /// <summary>UC11: Wraps this VolumeUnit as an IMeasurable for use with Quantity&lt;T&gt;.</summary>
        public static VolumeUnitMeasurable AsMeasurable(this VolumeUnit unit)
            => new VolumeUnitMeasurable(unit);
    }

    /// <summary>
    /// UC11: Adapter wrapping VolumeUnit as IMeasurable.
    /// Required because C# enums cannot directly implement interfaces (unlike Java).
    /// Mirrors the pattern of LengthUnitMeasurable and WeightUnitMeasurable.
    /// </summary>
    public sealed class VolumeUnitMeasurable : IMeasurable
    {
        private readonly VolumeUnit _unit;
        public VolumeUnit Unit => _unit;

        public VolumeUnitMeasurable(VolumeUnit unit) { _unit = unit; }

        public double GetConversionFactor()         => _unit.GetConversionFactor();
        public double ConvertToBaseUnit(double v)   => _unit.ConvertToBaseUnit(v);
        public double ConvertFromBaseUnit(double b) => _unit.ConvertFromBaseUnit(b);
        public string GetUnitName()                 => _unit.GetUnitName();

        public override bool Equals(object? obj)
            => obj is VolumeUnitMeasurable other && _unit == other._unit;
        public override int GetHashCode() => _unit.GetHashCode();
        public override string ToString()  => _unit.ToString();
    }
}