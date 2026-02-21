using System;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// Represents a measurement in Feet.
    /// This class validates input and supports equality comparison.
    /// </summary>
    public class Feet
    {
        /// <summary>
        /// Gets the value of feet.
        /// Property is read-only to maintain immutability.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Constructor to initialize Feet object.
        /// Validates business rules before assigning value.
        /// </summary>
        /// <param name="value">Feet measurement value</param>
        /// <exception cref="ArgumentException">
        /// Thrown when value is negative or exceeds allowed limit.
        /// </exception>
        public Feet(double value)
        {
            // 🔹 Rule 1: Negative values are not allowed
            if (value < 0)
                throw new ArgumentException("Feet value cannot be negative.");

            // 🔹 Rule 2: Extremely large values are not allowed
            if (value > 10000)
                throw new ArgumentException("Feet value cannot exceed 10000.");

            // Assign validated value
            Value = value;
        }

        /// <summary>
        /// Overrides default Equals method to compare two Feet objects.
        /// Two Feet objects are considered equal if their values 
        /// differ by less than 0.0001 (floating-point tolerance).
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if equal, otherwise false</returns>
        public override bool Equals(object obj)
        {
            // 🔹 If both references point to same object → true
            if (this == obj)
                return true;

            // 🔹 If object is null OR not of type Feet → false
            if (obj == null || GetType() != obj.GetType())
                return false;

            // 🔹 Type cast object to Feet
            Feet other = (Feet)obj;

            // 🔹 Compare values with tolerance (to handle floating-point precision issues)
            return Math.Abs(Value - other.Value) < 0.0001;
        }

        /// <summary>
        /// Overrides GetHashCode.
        /// Equal objects must return the same hash code.
        /// </summary>
        /// <returns>Hash code based on Value</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}