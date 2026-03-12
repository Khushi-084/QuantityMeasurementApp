using System;
using QuantityMeasurementModel.Interfaces;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC14: Temperature unit implemented as a class-based enum implementing IMeasurable.
    /// Follows the same pattern as LengthUnitM, WeightUnitM, and VolumeUnitM.
    ///
    /// Key UC14 design decisions:
    ///   - Base unit is KELVIN (absolute scale; all conversions normalize through Kelvin).
    ///   - Conversions are NON-LINEAR (offset formulas, not simple multiplication).
    ///   - Arithmetic (add/subtract/divide) is NOT supported for absolute temperatures.
    ///   - SupportsArithmetic() overrides IMeasurable default to return false.
    ///   - ValidateOperationSupport() overrides to throw NotSupportedException.
    ///   - GetConversionFactor() returns 1.0 (not meaningful for non-linear; present for contract).
    ///
    /// Lambda-style functional interface equivalent (C# Func&lt;double, double&gt;):
    ///   Each constant holds a ToKelvin and FromKelvin converter — the C# equivalent of
    ///   Java's Function&lt;Double, Double&gt; lambda expressions per enum constant.
    /// </summary>
    public class TemperatureUnit : IMeasurable
    {
        // ===== Constants =====

        /// <summary>Celsius. Base conversion: K = C + 273.15</summary>
        public static readonly TemperatureUnit CELSIUS    = new TemperatureUnit(
            "CELSIUS",
            toKelvin:   celsius    => celsius + 273.15,
            fromKelvin: kelvin     => kelvin  - 273.15
        );

        /// <summary>Fahrenheit. Base conversion: K = (F - 32) * 5/9 + 273.15</summary>
        public static readonly TemperatureUnit FAHRENHEIT = new TemperatureUnit(
            "FAHRENHEIT",
            toKelvin:   fahrenheit => (fahrenheit - 32.0) * 5.0 / 9.0 + 273.15,
            fromKelvin: kelvin     => (kelvin - 273.15) * 9.0 / 5.0 + 32.0
        );

        /// <summary>Kelvin (base unit). Identity conversion.</summary>
        public static readonly TemperatureUnit KELVIN     = new TemperatureUnit(
            "KELVIN",
            toKelvin:   kelvin => kelvin,
            fromKelvin: kelvin => kelvin
        );

        // ===== Fields =====

        private readonly string              name;
        private readonly Func<double, double> toKelvinConverter;
        private readonly Func<double, double> fromKelvinConverter;

        /// <summary>
        /// UC14: Lambda-style SupportsArithmetic flag — equivalent to Java's:
        ///   SupportsArithmetic supportsArithmetic = () -> false;
        /// Temperature does not support arithmetic on absolute values.
        /// </summary>
        private readonly bool arithmeticSupported = false;

        // ===== Constructor =====

        private TemperatureUnit(
            string name,
            Func<double, double> toKelvin,
            Func<double, double> fromKelvin)
        {
            this.name                = name;
            this.toKelvinConverter   = toKelvin;
            this.fromKelvinConverter = fromKelvin;
        }

        // ===== IMeasurable — Mandatory Methods =====

        /// <summary>
        /// Returns 1.0 — conversion factor is not meaningful for non-linear temperature.
        /// Present to satisfy IMeasurable contract.
        /// </summary>
        public double GetConversionFactor() => 1.0;

        /// <summary>
        /// Converts a temperature value in this unit to the base unit (Kelvin).
        /// Uses the lambda converter defined per constant.
        /// </summary>
        public double ConvertToBaseUnit(double value) => toKelvinConverter(value);

        /// <summary>
        /// Converts a Kelvin value back to this unit.
        /// Uses the lambda converter defined per constant.
        /// </summary>
        public double ConvertFromBaseUnit(double baseValue) => fromKelvinConverter(baseValue);

        /// <summary>Returns the unit name (CELSIUS / FAHRENHEIT / KELVIN).</summary>
        public string GetUnitName() => name;

        // ===== IMeasurable — UC14 Overrides =====

        /// <summary>
        /// UC14: Overrides default IMeasurable.SupportsArithmetic().
        /// Returns false — temperature does not support add/subtract/divide on absolute values.
        /// C# equivalent of Java lambda: SupportsArithmetic supportsArithmetic = () -> false;
        /// </summary>
        public bool SupportsArithmetic() => arithmeticSupported;

        /// <summary>
        /// UC14: Overrides default IMeasurable.ValidateOperationSupport().
        /// Throws NotSupportedException with a clear, informative message.
        /// Called by Quantity&lt;U&gt;.ValidateArithmeticOperands() before any arithmetic attempt.
        /// </summary>
        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException(
                $"Temperature does not support '{operation}' on absolute temperature values. " +
                $"Temperature arithmetic is physically meaningless (e.g. 100°C + 50°C ≠ 150°C). " +
                $"Only equality comparison and unit conversion are supported for TemperatureUnit.");
        }

        // ===== Display =====

        public override string ToString() => name;
    }
}
