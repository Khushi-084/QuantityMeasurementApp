namespace QuantityMeasurementBusinessLayer{
    /// <summary>
    /// UC10: Common contract for all measurement unit enums.
    /// Enables generic Quantity&lt;U&gt; to work with any measurement category.
    ///
    /// UC14: Refactored to support selective arithmetic via default interface methods.
    ///       - SupportsArithmetic(): default returns true; TemperatureUnit overrides to false.
    ///       - ValidateOperationSupport(): default no-op; TemperatureUnit overrides to throw.
    ///       - Existing units (Length, Weight, Volume) require NO changes — defaults apply.
    ///       - Adheres to Interface Segregation Principle: categories opt in to arithmetic.
    /// </summary>
    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();

        bool SupportsArithmetic() => true;

        void ValidateOperationSupport(string operation)
        {
            // Default: no-op.
        }
    }
}
