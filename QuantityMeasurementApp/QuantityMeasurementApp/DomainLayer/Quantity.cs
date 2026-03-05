using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC10: Generic immutable value object for any measurement category.
    /// Replaces the parallel QuantityLength / QuantityWeight classes.
    /// T must implement IMeasurable (e.g. LengthUnitMeasurable, WeightUnitMeasurable).
    /// </summary>
    public class Quantity<T> where T : IMeasurable
    {
        private readonly double _value;
        private readonly T _unit;

        public double Value => _value;
        public T Unit => _unit;

        /// <summary>
        /// Creates an immutable Quantity.
        /// </summary>
        /// <param name="value">The numeric value. Must be finite (no NaN / infinity).</param>
        /// <param name="unit">The measurement unit. Must be non-null.</param>
        public Quantity(double value, T unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException(
                    "Value must be a finite number; NaN and infinity are not allowed.",
                    nameof(value));

            if (unit is null)
                throw new ArgumentNullException(nameof(unit), "Unit must not be null.");

            _value = value;
            _unit  = unit;
        }

        // ─────────────────────────── Conversion ───────────────────────────

        /// <summary>
        /// UC10: Converts this quantity to the specified target unit.
        /// Returns a new Quantity instance (immutability preserved).
        /// </summary>
        public Quantity<T> ConvertTo(T targetUnit)
        {
            if (targetUnit is null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit must not be null.");

            double baseValue = _unit.ConvertToBaseUnit(_value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            return new Quantity<T>(converted, targetUnit);
        }

        // ─────────────────────────── Addition ─────────────────────────────

        /// <summary>
        /// UC10: Adds another quantity to this one. Result is in this instance's unit.
        /// </summary>
        public Quantity<T> Add(Quantity<T> other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            double sumInBase = _unit.ConvertToBaseUnit(_value)
                             + other._unit.ConvertToBaseUnit(other._value);
            double result    = _unit.ConvertFromBaseUnit(sumInBase);
            return new Quantity<T>(result, _unit);
        }

        /// <summary>
        /// UC10: Static overload — adds two quantities; result is in q1's unit.
        /// </summary>
        public static Quantity<T> Add(Quantity<T> q1, Quantity<T> q2)
        {
            if (q1 is null) throw new ArgumentNullException(nameof(q1));
            if (q2 is null) throw new ArgumentNullException(nameof(q2));
            return q1.Add(q2);
        }

        /// <summary>
        /// UC10: Static overload — adds two quantities and returns result in the specified target unit.
        /// </summary>
        public static Quantity<T> Add(Quantity<T> q1, Quantity<T> q2, T targetUnit)
        {
            if (q1 is null)         throw new ArgumentNullException(nameof(q1));
            if (q2 is null)         throw new ArgumentNullException(nameof(q2));
            if (targetUnit is null) throw new ArgumentNullException(nameof(targetUnit));

            double sumInBase = q1._unit.ConvertToBaseUnit(q1._value)
                             + q2._unit.ConvertToBaseUnit(q2._value);
            double result    = targetUnit.ConvertFromBaseUnit(sumInBase);
            return new Quantity<T>(result, targetUnit);
        }

        // ─────────────────────────── Subtraction ──────────────────────────

        /// <summary>
        /// UC12: Subtracts another quantity from this one. Result is in this instance's unit.
        /// Supports cross-unit subtraction within the same measurement category.
        /// </summary>
        /// <param name="other">The quantity to subtract. Must not be null.</param>
        /// <returns>A new Quantity with the difference, expressed in this instance's unit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public Quantity<T> Subtract(Quantity<T> other)
        {
            ValidateArithmeticOperand(other, nameof(other));

            double diffInBase = _unit.ConvertToBaseUnit(_value)
                              - other._unit.ConvertToBaseUnit(other._value);
            double result = Math.Round(_unit.ConvertFromBaseUnit(diffInBase), 2);
            return new Quantity<T>(result, _unit);
        }

        /// <summary>
        /// UC12: Subtracts another quantity, returning result in the specified target unit.
        /// </summary>
        /// <param name="other">The quantity to subtract. Must not be null.</param>
        /// <param name="targetUnit">The unit for the result. Must not be null.</param>
        /// <returns>A new Quantity with the difference, expressed in targetUnit.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other or targetUnit is null.</exception>
        public Quantity<T> Subtract(Quantity<T> other, T targetUnit)
        {
            ValidateArithmeticOperand(other, nameof(other));
            if (targetUnit is null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit must not be null.");

            double diffInBase = _unit.ConvertToBaseUnit(_value)
                              - other._unit.ConvertToBaseUnit(other._value);
            double result = Math.Round(targetUnit.ConvertFromBaseUnit(diffInBase), 2);
            return new Quantity<T>(result, targetUnit);
        }

        // ─────────────────────────── Division ─────────────────────────────

        /// <summary>
        /// UC12: Divides this quantity by another, returning a dimensionless scalar ratio.
        /// Both quantities are converted to base units before dividing, so cross-unit
        /// division within the same category is fully supported.
        /// </summary>
        /// <param name="other">The divisor. Must not be null and must not be zero.</param>
        /// <returns>A dimensionless double — how many times larger this quantity is.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        /// <exception cref="ArithmeticException">Thrown when the divisor evaluates to zero.</exception>
        /// <example>
        /// new Quantity(10.0, FEET).Divide(new Quantity(2.0, FEET))  → 5.0
        /// new Quantity(12.0, INCHES).Divide(new Quantity(1.0, FEET)) → 1.0
        /// </example>
        public double Divide(Quantity<T> other)
        {
            ValidateArithmeticOperand(other, nameof(other));

            double otherBase = other._unit.ConvertToBaseUnit(other._value);
            if (Math.Abs(otherBase) < 1e-10)
                throw new ArithmeticException(
                    "Division by zero: the divisor quantity evaluates to zero.");

            double thisBase = _unit.ConvertToBaseUnit(_value);
            return thisBase / otherBase;
        }

        // ─────────────────────────── Private Helpers ──────────────────────

        /// <summary>
        /// Validates that an arithmetic operand is non-null and belongs to the same
        /// measurement category as this instance. Shared by Subtract and Divide.
        /// </summary>
        private void ValidateArithmeticOperand(Quantity<T>? other, string paramName)
        {
            if (other is null)
                throw new ArgumentNullException(paramName,
                    "Operand quantity must not be null.");

            if (_unit.GetType() != other._unit.GetType())
                throw new ArgumentException(
                    $"Cross-category arithmetic is not allowed. " +
                    $"This quantity uses {_unit.GetType().Name}, " +
                    $"but the other uses {other._unit.GetType().Name}.",
                    paramName);
        }

        // ─────────────────────────── Equality ─────────────────────────────

        /// <summary>
        /// UC10: Cross-unit equality — compares physical values after converting to base unit.
        /// Cross-category comparisons (e.g. Feet vs Kilogram) always return false.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Quantity<T> other) return false;

            if (_unit.GetType() != other._unit.GetType()) return false;

            double thisBase  = _unit.ConvertToBaseUnit(_value);
            double otherBase = other._unit.ConvertToBaseUnit(other._value);

            return Math.Abs(thisBase - otherBase) < 1e-4;
        }

        public override int GetHashCode()
        {
            double normalized = _unit.ConvertToBaseUnit(_value);
            return Math.Round(normalized, 4).GetHashCode();
        }

        public override string ToString()
            => $"Quantity({_value}, {_unit.GetUnitName()})";
    }
}