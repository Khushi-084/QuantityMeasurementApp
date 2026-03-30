namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC15: Internal model class used within the Service Layer.
    /// Different from QuantityDTO — this is used INSIDE the service,
    /// not for external communication.
    /// Wraps the existing Quantity<U> concept with IMeasurable units.
    /// </summary>
    public class QuantityModel<U> where U : class, IMeasurable
    {
        public double Value { get; }
        public U Unit       { get; }

        public QuantityModel(double value, U unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit), "Unit cannot be null.");
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.");

            Value = value;
            Unit  = unit;
        }

        /// <summary>Convert this model's value to base unit.</summary>
        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);

        /// <summary>Convert to a target unit — returns new QuantityModel.</summary>
        public QuantityModel<U> ConvertTo(U targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit));

            double baseValue  = Unit.ConvertToBaseUnit(Value);
            double converted  = targetUnit.ConvertFromBaseUnit(baseValue);
            double rounded    = Math.Round(converted, 2, MidpointRounding.AwayFromZero);
            return new QuantityModel<U>(rounded, targetUnit);
        }

        public override string ToString()=> $"QuantityModel({Value}, {Unit.GetUnitName()})";
    }
}
