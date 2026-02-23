using System;

namespace QuantityMeasurementApp.DomainLayer
{
    /// <summary>
    /// Represents a Feet measurement.
    /// Contains validation and equality logic (UC-1).
    /// </summary>
    public class Feet
    {
        public double Value { get; }

        /// <summary>
        /// Constructor with validation rules.
        /// </summary>
        /// <param name="value">Feet value</param>
        public Feet(double value)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Feet value cannot be NaN.");

            if (double.IsInfinity(value))
                throw new ArgumentException("Feet value cannot be Infinity.");

            if (value < 0)
                throw new ArgumentException("Feet value cannot be negative.");

            if (value > 1000000) // Upper limit constraint
                throw new ArgumentException("Feet value is too large.");

            Value = value;
        }

        /// <summary>
        /// Equality comparison for Feet (UC-1 functionality).
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Feet))
                return false;

            Feet other = (Feet)obj;

            // Direct comparison since values are controlled
            return this.Value == other.Value;
        }

        /// <summary>
        /// Required when overriding Equals
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}