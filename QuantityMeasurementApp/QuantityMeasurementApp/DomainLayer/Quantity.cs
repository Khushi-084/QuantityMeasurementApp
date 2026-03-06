using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC13: Identifies which arithmetic operation to perform in the centralized helper.
    /// Adding a new operation (e.g. Multiply):
    ///   1. Add "Multiply" to this enum.
    ///   2. Add "Multiply => a * b" to the switch in Compute().
    ///   3. Add a public Multiply() method to Quantity&lt;T&gt; that calls
    ///      PerformBaseArithmetic(other, ArithmeticOperation.Multiply).
    ///   No changes to ValidateArithmeticOperands or PerformBaseArithmetic needed.
    /// </summary>
    public enum ArithmeticOperation
    {
        Add,
        Subtract,
        Divide
    }

    /// <summary>
    /// UC13: Extension methods for ArithmeticOperation.
    /// Compute() executes the actual math for each operation constant.
    /// This is the C# equivalent of Java's enum abstract method pattern.
    /// </summary>
    public static class ArithmeticOperationExtensions
    {
        /// <summary>
        /// Applies this operation to two base-unit values and returns the result.
        /// </summary>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="a">Left-hand base-unit value (this quantity).</param>
        /// <param name="b">Right-hand base-unit value (other quantity).</param>
        /// <returns>Result of the arithmetic in base units.</returns>
        /// <exception cref="ArithmeticException">Thrown by Divide when b is zero.</exception>
        public static double Compute(this ArithmeticOperation operation, double a, double b)
        {
            return operation switch
            {
                ArithmeticOperation.Add      => a + b,
                ArithmeticOperation.Subtract => a - b,
                ArithmeticOperation.Divide   => Math.Abs(b) < 1e-10
                                                  ? throw new ArithmeticException(
                                                        "Division by zero: the divisor quantity evaluates to zero.")
                                                  : a / b,
                _ => throw new ArgumentOutOfRangeException(nameof(operation),
                         $"Unsupported arithmetic operation: {operation}")
            };
        }
    }

    /// <summary>
    /// UC10–UC14: Generic immutable value object for any measurement category.
    ///
    /// UC13 refactored Add / Subtract / Divide to use centralized private helpers (DRY).
    ///
    /// UC14 changes (fully backward compatible):
    ///   • Add()      — calls _unit.ValidateOperationSupport("Add") before any other logic.
    ///   • Subtract() — calls _unit.ValidateOperationSupport("Subtract") before any other logic.
    ///   • Divide()   — calls _unit.ValidateOperationSupport("Divide") before any other logic.
    ///   • ConvertTo() — unchanged; the Kelvin base-unit round-trip works correctly for
    ///                   TemperatureUnitMeasurable without any special branching.
    ///
    /// All UC1–UC13 tests pass without modification because:
    ///   • LengthUnitMeasurable / WeightUnitMeasurable / VolumeUnitMeasurable inherit the
    ///     default no-op ValidateOperationSupport from IMeasurable — nothing changes for them.
    /// </summary>
    public class Quantity<T> where T : IMeasurable
    {
        private readonly double _value;
        private readonly T _unit;

        public double Value => _value;
        public T Unit  => _unit;

        // ─────────────────────────── Constructor ──────────────────────────────

        /// <summary>
        /// Creates an immutable Quantity.
        /// </summary>
        /// <param name="value">Numeric value. Must be finite (no NaN / infinity).</param>
        /// <param name="unit">Measurement unit. Must be non-null.</param>
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

        // ─────────────────────────── Conversion ───────────────────────────────

        /// <summary>
        /// UC10 / UC14: Converts this quantity to the specified target unit.
        ///
        /// Works uniformly for ALL unit types including TemperatureUnitMeasurable,
        /// because ConvertToBaseUnit (→ Kelvin) and ConvertFromBaseUnit (← Kelvin)
        /// already encode the non-linear temperature formulas.
        /// No special branching is needed here.
        /// </summary>
        public Quantity<T> ConvertTo(T targetUnit)
        {
            if (targetUnit is null)
                throw new ArgumentNullException(nameof(targetUnit),
                    "Target unit must not be null.");

            double baseValue = _unit.ConvertToBaseUnit(_value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            return new Quantity<T>(converted, targetUnit);
        }

        // ─────────────────────────── Addition ─────────────────────────────────

        /// <summary>
        /// UC13 / UC14: Adds another quantity to this one.
        /// Result is expressed in this instance's unit (implicit target unit).
        ///
        /// UC14: ValidateOperationSupport is called first — throws NotSupportedException
        /// for TemperatureUnitMeasurable; is a no-op for all other unit types.
        /// </summary>
        public Quantity<T> Add(Quantity<T> other)
        {
            // UC14: validate operation support before any other processing
            _unit.ValidateOperationSupport(nameof(ArithmeticOperation.Add));

            ValidateArithmeticOperands(other, targetUnit: default, targetUnitRequired: false);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.Add);
            return new Quantity<T>(_unit.ConvertFromBaseUnit(baseResult), _unit);
        }

        /// <summary>UC13: Static overload — adds two quantities; result is in q1's unit.</summary>
        public static Quantity<T> Add(Quantity<T> q1, Quantity<T> q2)
        {
            if (q1 is null) throw new ArgumentNullException(nameof(q1));
            if (q2 is null) throw new ArgumentNullException(nameof(q2));
            return q1.Add(q2);
        }

        /// <summary>
        /// UC13: Static overload — adds two quantities; result is in the specified target unit.
        /// UC14: ValidateOperationSupport called before other checks.
        /// </summary>
        public static Quantity<T> Add(Quantity<T> q1, Quantity<T> q2, T targetUnit)
        {
            if (q1 is null)         throw new ArgumentNullException(nameof(q1));
            if (q2 is null)         throw new ArgumentNullException(nameof(q2));
            if (targetUnit is null) throw new ArgumentNullException(nameof(targetUnit));

            // UC14: validate before other checks
            q1._unit.ValidateOperationSupport(nameof(ArithmeticOperation.Add));

            q1.ValidateArithmeticOperands(q2, targetUnit, targetUnitRequired: true);
            double baseResult = q1.PerformBaseArithmetic(q2, ArithmeticOperation.Add);
            return new Quantity<T>(targetUnit.ConvertFromBaseUnit(baseResult), targetUnit);
        }

        // ─────────────────────────── Subtraction ──────────────────────────────

        /// <summary>
        /// UC13 / UC14: Subtracts another quantity from this one.
        /// Result is expressed in this instance's unit.
        /// Result is rounded to two decimal places (UC12 behaviour preserved).
        ///
        /// UC14: ValidateOperationSupport called first.
        /// </summary>
        public Quantity<T> Subtract(Quantity<T> other)
        {
            // UC14: validate operation support before any other processing
            _unit.ValidateOperationSupport(nameof(ArithmeticOperation.Subtract));

            ValidateArithmeticOperands(other, targetUnit: default, targetUnitRequired: false);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.Subtract);
            return new Quantity<T>(RoundToTwoDecimals(_unit.ConvertFromBaseUnit(baseResult)), _unit);
        }

        /// <summary>
        /// UC13 / UC14: Subtracts another quantity, returning result in the specified target unit.
        /// Result is rounded to two decimal places.
        /// UC14: ValidateOperationSupport called first.
        /// </summary>
        public Quantity<T> Subtract(Quantity<T> other, T targetUnit)
        {
            // UC14: validate operation support before any other processing
            _unit.ValidateOperationSupport(nameof(ArithmeticOperation.Subtract));

            ValidateArithmeticOperands(other, targetUnit, targetUnitRequired: true);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.Subtract);
            return new Quantity<T>(RoundToTwoDecimals(targetUnit.ConvertFromBaseUnit(baseResult)), targetUnit);
        }

        // ─────────────────────────── Division ─────────────────────────────────

        /// <summary>
        /// UC13 / UC14: Divides this quantity by another, returning a dimensionless scalar ratio.
        /// Result is NOT rounded — division returns a raw double.
        ///
        /// UC14: ValidateOperationSupport called first.
        /// </summary>
        public double Divide(Quantity<T> other)
        {
            // UC14: validate operation support before any other processing
            _unit.ValidateOperationSupport(nameof(ArithmeticOperation.Divide));

            ValidateArithmeticOperands(other, targetUnit: default, targetUnitRequired: false);
            return PerformBaseArithmetic(other, ArithmeticOperation.Divide);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UC13: CENTRALIZED PRIVATE HELPERS  (DRY enforcement)
        //  UC14: No changes required here — validation is at entry points above.
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// UC13: Centralized validation helper shared by ALL arithmetic operations.
        ///
        /// Checks (in order):
        ///   1. Null check on the operand.
        ///   2. Same measurement category — unit runtime types must match.
        ///   3. Finite value on this quantity (no NaN / infinity).
        ///   4. Finite value on the other quantity (no NaN / infinity).
        ///   5. Non-null target unit — only when targetUnitRequired is true.
        /// </summary>
        private void ValidateArithmeticOperands(Quantity<T>? other, T? targetUnit, bool targetUnitRequired)
        {
            // 1. Null operand check
            if (other is null)
                throw new ArgumentNullException(nameof(other),
                    "Operand quantity must not be null.");

            // 2. Same measurement category check
            if (_unit.GetType() != other._unit.GetType())
                throw new ArgumentException(
                    $"Cross-category arithmetic is not allowed. " +
                    $"This quantity uses {_unit.GetType().Name}, " +
                    $"but the other uses {other._unit.GetType().Name}.",
                    nameof(other));

            // 3. Finiteness check on this quantity's value
            if (!double.IsFinite(_value))
                throw new ArgumentException(
                    "This quantity's value must be finite (no NaN or infinity).",
                    nameof(_value));

            // 4. Finiteness check on the other quantity's value
            if (!double.IsFinite(other._value))
                throw new ArgumentException(
                    "The operand quantity's value must be finite (no NaN or infinity).",
                    nameof(other));

            // 5. Target unit null check (only for explicit-unit overloads)
            if (targetUnitRequired && targetUnit is null)
                throw new ArgumentNullException(nameof(targetUnit),
                    "Target unit must not be null.");
        }

        /// <summary>
        /// UC13: Core arithmetic helper — centralises base-unit conversion and
        /// operation dispatch for ALL arithmetic operations.
        ///
        /// Flow:
        ///   1. Convert this quantity's value to base unit.
        ///   2. Convert other quantity's value to base unit.
        ///   3. Execute operation.Compute(thisBase, otherBase).
        ///   4. Return the raw base-unit result.
        /// </summary>
        private double PerformBaseArithmetic(Quantity<T> other, ArithmeticOperation operation)
        {
            double thisBase  = _unit.ConvertToBaseUnit(_value);
            double otherBase = other._unit.ConvertToBaseUnit(other._value);
            return operation.Compute(thisBase, otherBase);
        }

        /// <summary>
        /// UC13: Rounds a value to two decimal places.
        /// Applied to Subtract results only — Add and Divide are not rounded.
        /// </summary>
        private static double RoundToTwoDecimals(double value)
            => Math.Round(value, 2);

        // ─────────────────────────── Equality ─────────────────────────────────

        /// <summary>
        /// UC10: Cross-unit equality — compares physical values after converting to base unit.
        /// Cross-category comparisons (e.g. Celsius vs Feet) always return false.
        /// UC14: Works correctly for temperature — Kelvin base unit handles offsets.
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