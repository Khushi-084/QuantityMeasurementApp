using System;

namespace QuantityMeasurementModel
{
    /// <summary>
    /// UC15: Internal model class used within the Service Layer.
    /// Different from QuantityDTO — this is used INSIDE the service,
    /// not for external communication.
    /// Generic value object wrapping a value and its unit.
    /// No IMeasurable constraint here — Model layer has no dependency on BusinessLayer.
    /// The BusinessLayer enforces IMeasurable when constructing QuantityModel instances.
    /// </summary>
    public class QuantityModel<U> where U : class
    {
        public double Value { get; }
        public U Unit { get; }

        public QuantityModel(double value, U unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit), "Unit cannot be null.");
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite.");

            Value = value;
            Unit  = unit;
        }

        public override string ToString()
            => $"QuantityModel({Value}, {Unit})";
    }
}