using System;

namespace QuantityMeasurementApp.DomainLayer
{
    public class Inches
    {
        public double Value { get; }

        public Inches(double value)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Inches value cannot be NaN.");

            if (double.IsInfinity(value))
                throw new ArgumentException("Inches value cannot be Infinity.");

            if (value < 0)
                throw new ArgumentException("Inches value cannot be negative.");

            if (value > 1000000)
                throw new ArgumentException("Inches value is too large.");

            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Inches))
                return false;

            Inches other = (Inches)obj;
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
