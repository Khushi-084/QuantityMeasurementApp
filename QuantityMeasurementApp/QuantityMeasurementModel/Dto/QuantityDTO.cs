namespace QuantityMeasurementModel
{
    /// <summary>
    /// UC15: Data Transfer Object for passing quantity data between layers.
    /// Self-contained with its own unit enums.
    /// Used for input/output between Controller and Service layers.
    /// </summary>
    public class QuantityDTO
    {
        public double Value    { get; }
        public string UnitName { get; }
        public string Category { get; }

        public QuantityDTO(double value, string unitName, string category)
        {
            Value    = value;
            UnitName = unitName?.ToUpperInvariant()
                       ?? throw new ArgumentNullException(nameof(unitName));
            Category = category?.ToUpperInvariant()
                       ?? throw new ArgumentNullException(nameof(category));
        }

        // ── Internal unit enums ──────────────────────────────────────────

        public enum LengthUnit
        {
            FEET, INCHES, YARDS, CENTIMETERS
        }

        public enum WeightUnit
        {
            KILOGRAM, GRAM, POUND
        }

        public enum VolumeUnit
        {
            LITRE, MILLILITRE, GALLON
        }

        public enum TemperatureUnit
        {
            CELSIUS, FAHRENHEIT, KELVIN
        }

        public override string ToString()
            => $"QuantityDTO({Value}, {UnitName}, {Category})";
    }
}
