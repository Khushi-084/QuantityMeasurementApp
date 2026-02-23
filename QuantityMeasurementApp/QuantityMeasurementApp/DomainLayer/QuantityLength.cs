using System;

namespace QuantityMeasurementApp.DomainLayer
{
    public class QuantityLength      //one genric class that works with multiple units
    {
        private const double MAX_VALUE = 10000;

        public double Value { get; }
        public LengthUnit Unit { get; }

        public QuantityLength(double value, LengthUnit unit)
        {
            if (value < 0)
                throw new ArgumentException("Length cannot be negative");

            if (value > MAX_VALUE)
                throw new ArgumentException("Length value too large");

            Value = value;
            Unit = unit;
        }

        private double ConvertToFeet()      // converts any unit into FEET
        {
            return Value * Unit.ToFeet();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))   // If both objects are actually same object in memory → return true immediately.
                return true;

            if (obj is not QuantityLength other)   // If object is not QuantityLength → cannot compare → false.
                return false;

            return Math.Abs(this.ConvertToFeet() - other.ConvertToFeet()) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ConvertToFeet().GetHashCode();
        }
    }
}
