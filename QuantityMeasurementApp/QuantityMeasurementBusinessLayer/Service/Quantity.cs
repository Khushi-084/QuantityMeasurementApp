using System;

namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC10: Generic, type-safe Quantity class that works with any IMeasurable unit.
    /// UC12: Extended with Subtract and Divide arithmetic operations.
    /// UC13: Refactored to eliminate code duplication via centralized helper methods.
    ///       - ArithmeticOperation enum dispatches ADD/SUBTRACT/DIVIDE.
    ///       - ValidateArithmeticOperands: single validation point for all operations.
    ///       - PerformBaseArithmetic: single conversion + computation point.
    ///       Public API and all behaviors remain identical to UC12.
    /// </summary>
    public class Quantity<U> where U : class, IMeasurable
    {
        private readonly double value;
        private readonly U unit;

        public double Value => value;
        public U Unit => unit;

        // ===== UC13: ArithmeticOperation Enum =====

        /// <summary>
        /// UC13: Enum-based arithmetic operation dispatch (Lambda Expression approach).
        /// Each constant represents one arithmetic operation.
        /// Compute() acts as the DoubleBinaryOperator functional interface equivalent.
        /// Scalable: adding MULTIPLY only requires one new case in Compute().
        /// </summary>
        private enum ArithmeticOperation
        {
            ADD,
            SUBTRACT,
            DIVIDE
        }

        /// <summary>
        /// UC13: Executes the arithmetic operation on two base-unit values.
        /// Centralizes all operation logic — single source of truth.
        /// </summary>
        private static double Compute(ArithmeticOperation operation, double a, double b)
        {
            return operation switch
            {
                ArithmeticOperation.ADD      => a + b,
                ArithmeticOperation.SUBTRACT => a - b,
                ArithmeticOperation.DIVIDE   =>
                    (Math.Abs(b) < 1e-15)
                        ? throw new ArithmeticException(
                            "Division by zero: divisor quantity resolves to zero in base unit.")
                        : a / b,
                _ => throw new InvalidOperationException($"Unsupported operation: {operation}")
            };
        }

        public Quantity(double value, U unit)
        {
            if (unit == null)
                throw new ArgumentException("Unit cannot be null.");
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite (not NaN or Infinity).");

            this.value = value;
            this.unit  = unit;
        }

        // ===== Conversion =====

        public double ConvertToBaseUnit()
            => unit.ConvertToBaseUnit(value);

        public Quantity<U> ConvertTo(U targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentException("Target unit cannot be null.");

            double baseValue = unit.ConvertToBaseUnit(value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);

            // UC14: Temperature conversions are non-linear and use more decimal places (e.g. 273.15).
            // Round to 2 decimal places consistently for all categories.
            return new Quantity<U>(RoundToTwoDecimals(converted), targetUnit);
        }

        // ===== UC13: Centralized Private Helpers =====

        /// <summary>
        /// UC13: Single validation point for ALL arithmetic operations.
        /// UC14: Now also calls unit.ValidateOperationSupport() to reject unsupported categories.
        ///       TemperatureUnit overrides ValidateOperationSupport() to throw NotSupportedException.
        ///       All other units inherit the default no-op and pass through silently.
        /// Validates: operation support, null operand, category compatibility, finiteness,
        /// and optional explicit target unit.
        /// </summary>
        private void ValidateArithmeticOperands(Quantity<U> other, U? targetUnit, bool targetUnitRequired)
        {
            // UC14: Check if this unit supports arithmetic at all (e.g. TemperatureUnit throws here).
            // The string "arithmetic" is passed so TemperatureUnit can include it in the error message.
            unit.ValidateOperationSupport("arithmetic");

            if (other == null)
                throw new ArgumentNullException(nameof(other), "Operand quantity cannot be null.");

            if (unit.GetType() != other.unit.GetType())
                throw new ArgumentException(
                    $"Cannot perform arithmetic on different measurement categories: " +
                    $"{unit.GetType().Name} vs {other.unit.GetType().Name}.");

            if (!double.IsFinite(this.value))
                throw new ArgumentException("This quantity's value must be finite (not NaN or Infinity).");

            if (!double.IsFinite(other.value))
                throw new ArgumentException("Operand quantity's value must be finite (not NaN or Infinity).");

            if (targetUnitRequired && targetUnit == null)
                throw new ArgumentException("Target unit cannot be null.");
        }

        /// <summary>
        /// UC13: Core arithmetic helper.
        /// Converts both operands to base unit, executes ArithmeticOperation,
        /// returns raw base-unit result. Conversion back to target unit is caller's job.
        /// All three public operations delegate here — DRY principle enforced.
        /// </summary>
        private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
        {
            double thisBase  = unit.ConvertToBaseUnit(value);
            double otherBase = other.unit.ConvertToBaseUnit(other.value);
            return Compute(operation, thisBase, otherBase);
        }

        /// <summary>
        /// UC13: Rounds to two decimal places using AwayFromZero midpoint rounding.
        /// Used consistently by Add and Subtract. Division does NOT round.
        /// </summary>
        private static double RoundToTwoDecimals(double value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        // ===== Addition (UC10, refactored UC13) =====

        /// <summary>UC10: Adds two quantities. Result unit = this unit.</summary>
        public Quantity<U> Add(Quantity<U> other)
        {
            ValidateArithmeticOperands(other, null, false);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.ADD);
            double converted  = unit.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(RoundToTwoDecimals(converted), unit);
        }

        /// <summary>UC10: Adds two quantities. Result unit = specified target unit.</summary>
        public Quantity<U> Add(Quantity<U> other, U targetUnit)
        {
            ValidateArithmeticOperands(other, targetUnit, true);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.ADD);
            double converted  = targetUnit!.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(RoundToTwoDecimals(converted), targetUnit);
        }

        // ===== Subtraction (UC12, refactored UC13) =====

        /// <summary>
        /// UC12: Subtracts another quantity from this one. Result unit = this unit.
        /// Negative results valid. Originals unchanged (immutability).
        /// UC13: Delegates to PerformBaseArithmetic(SUBTRACT).
        /// </summary>
        public Quantity<U> Subtract(Quantity<U> other)
        {
            ValidateArithmeticOperands(other, null, false);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);
            double converted  = unit.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(RoundToTwoDecimals(converted), unit);
        }

        /// <summary>
        /// UC12: Subtracts another quantity from this one. Result in specified target unit.
        /// UC13: Delegates to PerformBaseArithmetic(SUBTRACT).
        /// </summary>
        public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
        {
            ValidateArithmeticOperands(other, targetUnit, true);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);
            double converted  = targetUnit!.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(RoundToTwoDecimals(converted), targetUnit);
        }

        // ===== Division (UC12, refactored UC13) =====

        /// <summary>
        /// UC12: Divides this quantity by another. Returns dimensionless scalar.
        /// Both quantities converted to base unit before dividing.
        /// Throws ArithmeticException for zero divisor. Result NOT rounded.
        /// UC13: Delegates to PerformBaseArithmetic(DIVIDE).
        /// </summary>
        public double Divide(Quantity<U> other)
        {
            ValidateArithmeticOperands(other, null, false);
            return PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);
        }

        // ===== Equality =====

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;

            var that = (Quantity<U>)obj;
            // UC14: Cross-category prevention — temperature cannot equal length/weight/volume.
            if (unit.GetType() != that.unit.GetType()) return false;

            double thisBase = unit.ConvertToBaseUnit(value);
            double thatBase = that.unit.ConvertToBaseUnit(that.value);
            // Epsilon 0.01 accommodates floating-point drift in temperature conversions (e.g. Kelvin offsets).
            return Math.Abs(thisBase - thatBase) < 0.01;
        }

        public override int GetHashCode()
            => unit.ConvertToBaseUnit(value).GetHashCode();

        public override string ToString()
            => $"Quantity({value}, {unit.GetUnitName()})";
    }
}
