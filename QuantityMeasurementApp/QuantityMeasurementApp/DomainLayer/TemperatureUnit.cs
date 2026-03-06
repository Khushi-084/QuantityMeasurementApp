using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    // ─────────────────────────────────────────────────────────────────────────
    // UC14: TemperatureUnit enum.
    //
    // Base unit: KELVIN (absolute scale — handles offset conversions cleanly).
    //
    // Supported   : Equality comparison, unit conversion (ConvertTo).
    // Unsupported : Add, Subtract, Divide  → throw NotSupportedException.
    //
    // Conversion formulas:
    //   Celsius    → Kelvin :  K = C + 273.15
    //   Kelvin     → Celsius:  C = K − 273.15
    //   Fahrenheit → Kelvin :  K = (F − 32) × 5/9 + 273.15
    //   Kelvin     → Fahrenheit: F = (K − 273.15) × 9/5 + 32
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>UC14: Supported temperature scales.</summary>
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }

    /// <summary>
    /// UC14: Extension methods for TemperatureUnit.
    /// Temperature conversions are non-linear (involve additive offsets),
    /// so they use Func&lt;double,double&gt; lambdas rather than simple
    /// multiplication/division factors used by Length/Weight/Volume.
    /// </summary>
    public static class TemperatureUnitExtensions
    {
        // ── UC14: Lambda — temperature does NOT support arithmetic ────────────
        // Mirrors Java:  SupportsArithmetic supportsArithmetic = () -> false;
        public static readonly SupportsArithmetic SupportsArithmeticLambda = () => false;

        // ── Conversion lambdas ────────────────────────────────────────────────

        // Celsius ↔ Kelvin lambdas
        private static readonly Func<double, double> CelsiusToKelvin    = (c) => c + 273.15;
        private static readonly Func<double, double> KelvinToCelsius    = (k) => k - 273.15;

        // Fahrenheit ↔ Kelvin lambdas
        private static readonly Func<double, double> FahrenheitToKelvin = (f) => (f - 32.0) * 5.0 / 9.0 + 273.15;
        private static readonly Func<double, double> KelvinToFahrenheit = (k) => (k - 273.15) * 9.0 / 5.0 + 32.0;

        // Identity lambda — Celsius to Celsius (mirrors Java CELSIUS_TO_CELSIUS)
        private static readonly Func<double, double> Identity            = (k) => k;

        /// <summary>
        /// UC14: Converts a value in the given unit to Kelvin (base unit).
        /// Uses the unit-specific lambda defined above.
        /// </summary>
        public static double ConvertToBaseUnit(this TemperatureUnit unit, double value)
        {
            Func<double, double> toKelvin = unit switch
            {
                TemperatureUnit.Celsius    => CelsiusToKelvin,
                TemperatureUnit.Fahrenheit => FahrenheitToKelvin,
                TemperatureUnit.Kelvin     => Identity,
                _ => throw new ArgumentException($"Unsupported temperature unit: {unit}", nameof(unit))
            };
            return toKelvin(value);
        }

        /// <summary>
        /// UC14: Converts a value from Kelvin (base unit) to the given unit.
        /// Uses the unit-specific lambda defined above.
        /// </summary>
        public static double ConvertFromBaseUnit(this TemperatureUnit unit, double kelvin)
        {
            Func<double, double> fromKelvin = unit switch
            {
                TemperatureUnit.Celsius    => KelvinToCelsius,
                TemperatureUnit.Fahrenheit => KelvinToFahrenheit,
                TemperatureUnit.Kelvin     => Identity,
                _ => throw new ArgumentException($"Unsupported temperature unit: {unit}", nameof(unit))
            };
            return fromKelvin(kelvin);
        }

        /// <summary>
        /// UC14: GetConversionFactor is not meaningful for temperature (non-linear).
        /// Returns 1.0 as a neutral sentinel; always use ConvertToBaseUnit instead.
        /// </summary>
        public static double GetConversionFactor(this TemperatureUnit unit) => 1.0;

        /// <summary>UC14: Returns a human-readable unit name.</summary>
        public static string GetUnitName(this TemperatureUnit unit) => unit.ToString().ToUpper();

        /// <summary>UC14: Wraps this TemperatureUnit as an IMeasurable for use with Quantity&lt;T&gt;.</summary>
        public static TemperatureUnitMeasurable AsMeasurable(this TemperatureUnit unit)
            => new TemperatureUnitMeasurable(unit);
    }

    /// <summary>
    /// UC14: Adapter wrapping TemperatureUnit as IMeasurable.
    /// Mirrors the pattern of LengthUnitMeasurable / WeightUnitMeasurable / VolumeUnitMeasurable.
    ///
    /// Key UC14 overrides:
    ///   • SupportsArithmeticOp()      → false  (overrides interface default of true)
    ///   • ValidateOperationSupport()  → always throws NotSupportedException
    /// </summary>
    public sealed class TemperatureUnitMeasurable : IMeasurable
    {
        private readonly TemperatureUnit _unit;
        public TemperatureUnit Unit => _unit;

        // UC14: Lambda indicates temperature does NOT support arithmetic
        // Mirrors Java:  SupportsArithmetic supportsArithmetic = () -> false;
        private static readonly SupportsArithmetic _supportsArithmetic = () => false;

        public TemperatureUnitMeasurable(TemperatureUnit unit) { _unit = unit; }

        // ── IMeasurable core contract ─────────────────────────────────────────

        public double GetConversionFactor()         => _unit.GetConversionFactor();
        public double ConvertToBaseUnit(double v)   => _unit.ConvertToBaseUnit(v);
        public double ConvertFromBaseUnit(double k) => _unit.ConvertFromBaseUnit(k);
        public string GetUnitName()                 => _unit.GetUnitName();

        // ── UC14: Override optional operation-support methods ─────────────────

        /// <summary>
        /// UC14: Temperature does not support arithmetic — returns false.
        /// Uses the SupportsArithmetic lambda delegate.
        /// </summary>
        public bool SupportsArithmeticOp() => _supportsArithmetic();

        /// <summary>
        /// UC14: Throws NotSupportedException for any arithmetic operation on temperature.
        /// Called by Quantity&lt;T&gt;.Add / Subtract / Divide before any other logic.
        /// Error message clearly explains why the operation is physically meaningless.
        /// </summary>
        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException(
                $"Temperature does not support {operation}. " +
                $"Adding or subtracting absolute temperatures is physically meaningless " +
                $"(e.g. 100°C + 50°C ≠ 150°C in a useful sense), and dividing temperature " +
                $"values yields a dimensionless number with no physical meaning. " +
                $"Only equality comparison and unit conversion are supported for {_unit.GetUnitName()}.");
        }

        // ── Equality / hashing ────────────────────────────────────────────────

        public override bool Equals(object? obj)
            => obj is TemperatureUnitMeasurable other && _unit == other._unit;

        public override int GetHashCode() => _unit.GetHashCode();
        public override string ToString()  => _unit.ToString();
    }
}