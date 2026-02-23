using System;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// Immutable value object representing a length measurement in a given unit.
    /// Supports equality by normalized length and conversion to other length units.
    /// </summary>
    public class QuantityLength
    {
        private readonly double value;
        private readonly LengthUnit unit; // using enum from ServiceLayer

        public QuantityLength(double value, LengthUnit unit)
        {
            if (value < 0)
                throw new ArgumentException("Length cannot be negative.");
             if (!Enum.IsDefined(typeof(LengthUnit), unit))
                throw new ArgumentException("Invalid unit type");
            this.value = value;
            this.unit = unit;
        }

        /// <summary>
        /// Converts this length to the specified target unit. Returns a new instance (value object semantics).
        /// </summary>
        /// <param name="targetUnit">The unit to convert to.</param>
        /// <returns>A new QuantityLength with the same physical length expressed in the target unit.</returns>
        public QuantityLength ConvertTo(LengthUnit targetUnit)
        {
            double converted = QuantityLengthService.Convert(value, unit, targetUnit);
            return new QuantityLength(converted, targetUnit);
        }

        // Convert any unit to inches for comparison
        private double ToInches()
        {
            return unit switch
            {
                LengthUnit.Feet => value * 12,
                LengthUnit.Inch => value,
                LengthUnit.Yard => value * 36,
                LengthUnit.Centimeter => value / 2.54,
                _ => throw new ArgumentException("Invalid unit.")
            };
        }

        public override bool Equals(object? obj) // <-- use object? to remove warning
        {
            if (obj is not QuantityLength other) return false;
            return Math.Abs(this.ToInches() - other.ToInches()) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ToInches().GetHashCode();
        }

        /// <summary>Returns a human-readable representation of this length (e.g. "3.5 Feet").</summary>
        public override string ToString()
        {
            return $"{value} {unit}";
        }
    }
}