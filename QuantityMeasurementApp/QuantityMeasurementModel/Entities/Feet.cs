using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Represents a measurement value in Feet.
    /// This class is a Value Object used for equality comparison.
    /// Validation is intentionally NOT done here.
    /// </summary>
    public class Feet
    {
        // Stores measurement value
        private readonly double value;

        /// <summary>
        /// Initializes a new Feet measurement.
        /// </summary>
        /// <param name="value">Feet value</param>
        public Feet(double value)
        {
            // Direct assignment
            this.value = value;
        }

        /// <summary>
        /// Returns stored measurement value.
        /// </summary>
        public double GetValue()
        {
            return value;
        }

        /// <summary>
        /// Overrides equality comparison using value-based equality.
        /// </summary>
        public override bool Equals(object? obj)
        {
            // Same reference check
            if (this == obj){
                return true;
            }
            // Null and type safety
            if (obj == null || GetType() != obj.GetType()){
                return false;
            }

            Feet other = (Feet)obj;

            // Floating precision comparison
            return Math.Abs(value - other.value) < 0.0001;
        }

        /// <summary>
        /// Required override when Equals is implemented.
        /// </summary>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Returns formatted string representation.
        /// </summary>
        public override string ToString()
        {
            return $"{value} ft";
        }
    }
}
