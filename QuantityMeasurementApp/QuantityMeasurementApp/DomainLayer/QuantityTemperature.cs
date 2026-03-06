using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC14: QuantityTemperature wrapper — mirrors QuantityLength, QuantityWeight, QuantityVolume.
    ///
    /// Adds two layers of validation on top of Quantity&lt;TemperatureUnitMeasurable&gt;:
    ///   1. Range validation — value must be ≥ absolute zero in the given unit,
    ///      and ≤ MaxTemperatureValue (1,000,000 °C equivalent).
    ///   2. Enum validation — unit must be a defined TemperatureUnit constant.
    ///
    /// Supported operations : Equals, ConvertTo.
    /// Unsupported operations: Add, Subtract, Divide — these are intentionally
    ///   NOT exposed on this class because temperature arithmetic is physically
    ///   meaningless. Attempting them via the inner Quantity&lt;T&gt; throws
    ///   NotSupportedException (enforced by TemperatureUnitMeasurable.ValidateOperationSupport).
    ///
    /// Absolute zero constants:
    ///   Celsius    : −273.15 °C
    ///   Fahrenheit : −459.67 °F
    ///   Kelvin     :    0.00  K
    /// </summary>
    public class QuantityTemperature
    {
        // ── Range constants ───────────────────────────────────────────────────

        /// <summary>Maximum allowed temperature in Celsius (practical upper bound).</summary>
        public const double MaxTemperatureCelsius    =  1_000_000.0;

        /// <summary>Minimum allowed temperature in Celsius (absolute zero).</summary>
        public const double MinTemperatureCelsius    = -273.15;

        /// <summary>Minimum allowed temperature in Fahrenheit (absolute zero).</summary>
        public const double MinTemperatureFahrenheit = -459.67;

        /// <summary>Minimum allowed temperature in Kelvin (absolute zero).</summary>
        public const double MinTemperatureKelvin     = 0.0;

        /// <summary>Maximum allowed temperature in Fahrenheit (equivalent of MaxTemperatureCelsius).</summary>
        public const double MaxTemperatureFahrenheit = MaxTemperatureCelsius * 9.0 / 5.0 + 32.0;

        /// <summary>Maximum allowed temperature in Kelvin (equivalent of MaxTemperatureCelsius).</summary>
        public const double MaxTemperatureKelvin     = MaxTemperatureCelsius + 273.15;

        // ── Internal state ────────────────────────────────────────────────────

        private readonly Quantity<TemperatureUnitMeasurable> _inner;

        public double          Value => _inner.Value;
        public TemperatureUnit Unit  => _inner.Unit.Unit;

        // ── Constructor ───────────────────────────────────────────────────────

        /// <summary>
        /// Creates a validated temperature quantity.
        /// </summary>
        /// <param name="value">Temperature value in the given unit.</param>
        /// <param name="unit">Temperature unit (Celsius, Fahrenheit, or Kelvin).</param>
        /// <exception cref="ArgumentException">
        ///   Thrown when value is NaN/infinity, below absolute zero, above max, or unit is invalid.
        /// </exception>
        public QuantityTemperature(double value, TemperatureUnit unit)
        {
            // 1. Finite check
            if (!double.IsFinite(value))
                throw new ArgumentException(
                    "Value must be a finite number; NaN and infinity are not allowed.",
                    nameof(value));

            // 2. Valid enum check
            if (!Enum.IsDefined(typeof(TemperatureUnit), unit))
                throw new ArgumentException("Invalid temperature unit.", nameof(unit));

            // 3. Range check — min (absolute zero) and max per unit
            double min = GetMinValue(unit);
            double max = GetMaxValue(unit);

            if (value < min)
                throw new ArgumentException(
                    $"Temperature value {value} {unit} is below absolute zero. " +
                    $"Minimum allowed value for {unit} is {min}.",
                    nameof(value));

            if (value > max)
                throw new ArgumentException(
                    $"Temperature value {value} {unit} exceeds the maximum allowed value of {max} {unit}.",
                    nameof(value));

            _inner = new Quantity<TemperatureUnitMeasurable>(value, new TemperatureUnitMeasurable(unit));
        }

        // Private constructor used internally by ConvertTo
        private QuantityTemperature(Quantity<TemperatureUnitMeasurable> inner)
        {
            _inner = inner;
        }

        // ── Operations ────────────────────────────────────────────────────────

        /// <summary>
        /// UC14: Converts this temperature to the specified target unit.
        /// Returns a new QuantityTemperature in the target unit.
        /// </summary>
        public QuantityTemperature ConvertTo(TemperatureUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(TemperatureUnit), targetUnit))
                throw new ArgumentException("Invalid temperature unit.", nameof(targetUnit));

            return new QuantityTemperature(
                _inner.ConvertTo(new TemperatureUnitMeasurable(targetUnit)));
        }

        // ── Equality ──────────────────────────────────────────────────────────

        public override bool Equals(object? obj)
        {
            if (obj is not QuantityTemperature other) return false;
            return _inner.Equals(other._inner);
        }

        public override int GetHashCode() => _inner.GetHashCode();

        public override string ToString() => $"{Value} {Unit}";

        // ── Static range helpers ──────────────────────────────────────────────

        /// <summary>Returns the minimum valid temperature value for the given unit (absolute zero).</summary>
        public static double GetMinValue(TemperatureUnit unit) => unit switch
        {
            TemperatureUnit.Celsius    => MinTemperatureCelsius,
            TemperatureUnit.Fahrenheit => MinTemperatureFahrenheit,
            TemperatureUnit.Kelvin     => MinTemperatureKelvin,
            _ => throw new ArgumentException($"Unsupported unit: {unit}", nameof(unit))
        };

        /// <summary>Returns the maximum valid temperature value for the given unit.</summary>
        public static double GetMaxValue(TemperatureUnit unit) => unit switch
        {
            TemperatureUnit.Celsius    => MaxTemperatureCelsius,
            TemperatureUnit.Fahrenheit => MaxTemperatureFahrenheit,
            TemperatureUnit.Kelvin     => MaxTemperatureKelvin,
            _ => throw new ArgumentException($"Unsupported unit: {unit}", nameof(unit))
        };
    }
}