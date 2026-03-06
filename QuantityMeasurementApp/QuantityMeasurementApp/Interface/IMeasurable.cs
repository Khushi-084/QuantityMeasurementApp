namespace QuantityMeasurementApp.Interface
{
    // ─────────────────────────────────────────────────────────────────────────
    // UC14: Functional delegate — C# equivalent of Java's @FunctionalInterface.
    // Indicates whether a measurement unit supports arithmetic operations.
    // Lambda usage:  SupportsArithmetic supported = () => true;
    //                SupportsArithmetic notSupported = () => false;
    // ─────────────────────────────────────────────────────────────────────────
    public delegate bool SupportsArithmetic();

    /// <summary>
    /// UC10–UC14: Common interface for all measurement unit adapters.
    ///
    /// UC14 additions (non-breaking — all existing adapters inherit defaults):
    ///   • SupportsArithmeticOp()       — default returns true; TemperatureUnitMeasurable overrides to false.
    ///   • ValidateOperationSupport()   — default no-op; TemperatureUnitMeasurable overrides to throw.
    ///
    /// Existing implementations (LengthUnitMeasurable, WeightUnitMeasurable, VolumeUnitMeasurable)
    /// require NO changes — they automatically inherit the default true / no-op behaviour.
    /// </summary>
    public interface IMeasurable
    {
        // ── Original UC10 contract ────────────────────────────────────────────

        /// <summary>Returns the conversion factor relative to the category's base unit.</summary>
        double GetConversionFactor();

        /// <summary>Converts a value expressed in this unit to the base unit.</summary>
        double ConvertToBaseUnit(double value);

        /// <summary>Converts a value from the base unit to this unit.</summary>
        double ConvertFromBaseUnit(double baseValue);

        /// <summary>Returns a human-readable name for this unit (e.g. "FEET", "KILOGRAM").</summary>
        string GetUnitName();

        // ── UC14 additions — optional operation support (default implementations) ──

        /// <summary>
        /// UC14: Returns true when this unit category supports arithmetic operations
        /// (Add, Subtract, Divide). Default: true — all existing units are unaffected.
        /// TemperatureUnitMeasurable overrides this to return false.
        /// </summary>
        bool SupportsArithmeticOp() => true;

        /// <summary>
        /// UC14: Validates that the named operation is supported for this unit.
        /// Default implementation: no-op (all operations allowed).
        /// TemperatureUnitMeasurable overrides to throw NotSupportedException with a clear message.
        /// </summary>
        /// <param name="operation">Human-readable operation name, e.g. "Add", "Subtract", "Divide".</param>
        void ValidateOperationSupport(string operation) { /* no-op by default */ }
    }
}