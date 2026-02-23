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
        /// <summary>Maximum allowed absolute value for length (in any unit). Values beyond this are rejected.</summary>
        public const double MaxLengthValue = 100000;

        private readonly double value;
        private readonly LengthUnit unit; // using enum from ServiceLayer

        public double Value => value;
        public LengthUnit Unit => unit;

        public QuantityLength(double value, LengthUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number; NaN and infinity are not allowed.", nameof(value));
            if (Math.Abs(value) > MaxLengthValue)
                throw new ArgumentException($"Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.", nameof(value));
            if (!Enum.IsDefined(typeof(LengthUnit), unit))
                throw new ArgumentException("Invalid unit type", nameof(unit));
            this.value = value;
            this.unit = unit;
        }

        /// <summary>
        /// Adds another length to this length. Result is in this instance's unit (first operand).
        /// Converts both to base unit (feet), adds, then converts sum back to this.unit.
        /// </summary>
        /// <param name="other">The length to add (must be non-null).</param>
        /// <returns>A new QuantityLength with the sum in this.unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public QuantityLength Add(QuantityLength other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            double sumInBase = QuantityLengthService.Convert(value, unit, LengthUnit.Feet)
                + QuantityLengthService.Convert(other.value, other.unit, LengthUnit.Feet);
            double resultValue = QuantityLengthService.Convert(sumInBase, LengthUnit.Feet, unit);
            return new QuantityLength(resultValue, unit);
        }

        /// <summary>
        /// Adds two length quantities. Result is in the unit of the first operand (length1).
        /// </summary>
        /// <param name="length1">First length (non-null); result unit = length1.Unit.</param>
        /// <param name="length2">Second length (non-null).</param>
        /// <returns>A new QuantityLength with the sum in length1.Unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when length1 or length2 is null.</exception>
        public static QuantityLength Add(QuantityLength length1, QuantityLength length2)
        {
            if (length1 is null)
                throw new ArgumentNullException(nameof(length1));
            if (length2 is null)
                throw new ArgumentNullException(nameof(length2));
            return length1.Add(length2);
        }

        /// <summary>
        /// UC7: Adds two length quantities and returns the result in the explicitly specified target unit.
        /// Converts both operands to base unit (feet), adds, then converts sum to targetUnit.
        /// </summary>
        /// <param name="length1">First length (non-null).</param>
        /// <param name="length2">Second length (non-null).</param>
        /// <param name="targetUnit">Target unit for the result.</param>
        /// <returns>A new QuantityLength with the sum in targetUnit.</returns>
        public static QuantityLength Add(QuantityLength length1, QuantityLength length2, LengthUnit targetUnit)
        {
            return AddInternal(length1, length2, targetUnit);
        }

        /// <summary>
        /// UC7 convenience overload to allow null validation for target unit.
        /// </summary>
        public static QuantityLength Add(QuantityLength length1, QuantityLength length2, LengthUnit? targetUnit)
        {
            if (targetUnit is null)
                throw new ArgumentException("Target unit must be provided.", nameof(targetUnit));
            return AddInternal(length1, length2, targetUnit.Value);
        }

        private static QuantityLength AddInternal(QuantityLength length1, QuantityLength length2, LengthUnit targetUnit)
        {
            if (length1 is null)
                throw new ArgumentNullException(nameof(length1));
            if (length2 is null)
                throw new ArgumentNullException(nameof(length2));
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Target unit must be a valid LengthUnit.", nameof(targetUnit));

            double sumInBase = QuantityLengthService.Convert(length1.value, length1.unit, LengthUnit.Feet)
                + QuantityLengthService.Convert(length2.value, length2.unit, LengthUnit.Feet);
            double resultValue = QuantityLengthService.Convert(sumInBase, LengthUnit.Feet, targetUnit);
            return new QuantityLength(resultValue, targetUnit);
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