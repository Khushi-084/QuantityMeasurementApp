using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Represents a measurement value in Inches.
    /// This class is a Value Object used for equality comparison.
    /// </summary>
    public class Inches
    {
        // Stores measurement value
        private readonly double value;

        /// <summary>
        /// Initializes a new Inches measurement.
        /// </summary>
        public Inches(double value)
        {
            this.value = value;
        }

        /// <summary>
        /// Returns stored value.
        /// </summary>
        public double GetValue()
        {
            return value;
        }

        /// <summary>
        /// Equality comparison.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (this == obj)
                return true;

            if (obj == null || GetType() != obj.GetType())
                return false;

            Inches other = (Inches)obj;

            return Math.Abs(value - other.value) < 0.0001;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{value} in";
        }
    }
}
