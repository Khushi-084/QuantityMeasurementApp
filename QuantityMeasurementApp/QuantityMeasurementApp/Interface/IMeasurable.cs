namespace QuantityMeasurementApp.Interface
{
    /// <summary>
    /// UC10: Common interface for all measurement unit enums.
    /// Provides a standard contract for unit conversion across all categories
    /// (length, weight, volume, temperature, etc.).
    /// </summary>
    public interface IMeasurable
    {
        /// <summary>Returns the conversion factor relative to the category's base unit.</summary>
        double GetConversionFactor();

        /// <summary>Converts a value expressed in this unit to the base unit.</summary>
        double ConvertToBaseUnit(double value);

        /// <summary>Converts a value from the base unit to this unit.</summary>
        double ConvertFromBaseUnit(double baseValue);

        /// <summary>Returns a human-readable name for this unit (e.g. "FEET", "KILOGRAM").</summary>
        string GetUnitName();
    }
}