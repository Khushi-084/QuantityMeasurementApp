using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC10: LengthUnit refactored to support IMeasurable via adapter.
    /// Base unit is Feet. All UC1–UC8 conversion factors are unchanged.
    /// </summary>
    public enum LengthUnit
    {
        Feet,
        Inch,
        Yard,
        Centimeter
    }

    /// <summary>
    /// UC10: Extension methods for LengthUnit implementing the IMeasurable contract.
    /// Fully backward compatible — all existing method signatures preserved.
    /// </summary>
    public static class LengthUnitExtensions
    {
        /// <summary>Conversion factor: feet per 1 unit.</summary>
        public static double GetConversionFactor(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.Feet       => 1.0,
                LengthUnit.Inch       => 1.0 / 12.0,
                LengthUnit.Yard       => 3.0,
                LengthUnit.Centimeter => 1.0 / 30.48,
                _ => throw new System.ArgumentException("Unsupported unit", nameof(unit))
            };
        }

        /// <summary>Legacy alias — preserved for backward compatibility.</summary>
        public static double ToFeet(this LengthUnit unit) => unit.GetConversionFactor();

        /// <summary>UC10 / IMeasurable: Converts a value in this unit to feet (base unit).</summary>
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
            => value * unit.GetConversionFactor();

        /// <summary>UC10 / IMeasurable: Converts a value from feet (base unit) to this unit.</summary>
        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
            => baseValue / unit.GetConversionFactor();

        /// <summary>UC10 / IMeasurable: Returns a readable unit name.</summary>
        public static string GetUnitName(this LengthUnit unit) => unit.ToString().ToUpper();

        /// <summary>UC10: Wraps this LengthUnit as an IMeasurable for use with Quantity&lt;T&gt;.</summary>
        public static IMeasurable AsMeasurable(this LengthUnit unit)
            => new LengthUnitMeasurable(unit);
    }

    /// <summary>
    /// UC10: Adapter wrapping LengthUnit as IMeasurable.
    /// Required because C# enums cannot directly implement interfaces (unlike Java).
    /// </summary>
    public sealed class LengthUnitMeasurable : IMeasurable
    {
        private readonly LengthUnit _unit;
        public LengthUnit Unit => _unit;

        public LengthUnitMeasurable(LengthUnit unit) { _unit = unit; }

        public double GetConversionFactor()         => _unit.GetConversionFactor();
        public double ConvertToBaseUnit(double v)   => _unit.ConvertToBaseUnit(v);
        public double ConvertFromBaseUnit(double b) => _unit.ConvertFromBaseUnit(b);
        public string GetUnitName()                 => _unit.GetUnitName();

        public override bool Equals(object? obj)
            => obj is LengthUnitMeasurable other && _unit == other._unit;
        public override int GetHashCode() => _unit.GetHashCode();
        public override string ToString()  => _unit.ToString();
    }
}