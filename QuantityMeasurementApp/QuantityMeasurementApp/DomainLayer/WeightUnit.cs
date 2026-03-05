using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC10: WeightUnit refactored to support IMeasurable via adapter.
    /// Base unit is Kilogram. All UC9 conversion factors are unchanged.
    /// </summary>
    public enum WeightUnit
    {
        Kilogram,
        Gram,
        Pound
    }

    /// <summary>
    /// UC10: Extension methods for WeightUnit implementing the IMeasurable contract.
    /// Fully backward compatible — all existing method signatures preserved.
    /// </summary>
    public static class WeightUnitExtensions
    {
        /// <summary>Conversion factor: kilograms per 1 unit.</summary>
        public static double GetConversionFactor(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.Kilogram => 1.0,
                WeightUnit.Gram     => 0.001,       // 1 g = 0.001 kg
                WeightUnit.Pound    => 0.453592,    // 1 lb ≈ 0.453592 kg
                _ => throw new ArgumentException("Unsupported weight unit", nameof(unit))
            };
        }

        /// <summary>UC10 / IMeasurable: Converts a value in this unit to kilograms (base unit).</summary>
        public static double ConvertToBaseUnit(this WeightUnit unit, double value)
            => value * unit.GetConversionFactor();

        /// <summary>UC10 / IMeasurable: Converts a value from kilograms (base unit) to this unit.</summary>
        public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue)
            => baseValue / unit.GetConversionFactor();

        /// <summary>UC10 / IMeasurable: Returns a readable unit name.</summary>
        public static string GetUnitName(this WeightUnit unit) => unit.ToString().ToUpper();

        /// <summary>UC10: Wraps this WeightUnit as an IMeasurable for use with Quantity&lt;T&gt;.</summary>
        public static IMeasurable AsMeasurable(this WeightUnit unit)
            => new WeightUnitMeasurable(unit);
    }

    /// <summary>
    /// UC10: Adapter wrapping WeightUnit as IMeasurable.
    /// Required because C# enums cannot directly implement interfaces (unlike Java).
    /// </summary>
    public sealed class WeightUnitMeasurable : IMeasurable
    {
        private readonly WeightUnit _unit;
        public WeightUnit Unit => _unit;

        public WeightUnitMeasurable(WeightUnit unit) { _unit = unit; }

        public double GetConversionFactor()         => _unit.GetConversionFactor();
        public double ConvertToBaseUnit(double v)   => _unit.ConvertToBaseUnit(v);
        public double ConvertFromBaseUnit(double b) => _unit.ConvertFromBaseUnit(b);
        public string GetUnitName()                 => _unit.GetUnitName();

        public override bool Equals(object? obj)
            => obj is WeightUnitMeasurable other && _unit == other._unit;
        public override int GetHashCode() => _unit.GetHashCode();
        public override string ToString()  => _unit.ToString();
    }
}