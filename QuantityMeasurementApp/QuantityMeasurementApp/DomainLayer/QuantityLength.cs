using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC10 backward-compatibility wrapper.
    /// Delegates all logic to Quantity&lt;LengthUnitMeasurable&gt; so that
    /// all UC1–UC8 test files compile and pass without any changes.
    /// </summary>
    public class QuantityLength
    {
        public const double MaxLengthValue = 100000;

        // internal generic instance
        private readonly Quantity<LengthUnitMeasurable> _inner;

        public double Value => _inner.Value;
        public LengthUnit Unit => _inner.Unit.Unit;

        public QuantityLength(double value, LengthUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException(
                    "Value must be a finite number; NaN and infinity are not allowed.",
                    nameof(value));
            if (Math.Abs(value) > MaxLengthValue)
                throw new ArgumentException(
                    $"Value must be between -{MaxLengthValue:N0} and {MaxLengthValue:N0}.",
                    nameof(value));
            if (!Enum.IsDefined(typeof(LengthUnit), unit))
                throw new ArgumentException("Invalid unit type", nameof(unit));

            _inner = new Quantity<LengthUnitMeasurable>(value, new LengthUnitMeasurable(unit));
        }

        // private constructor used internally by Add / ConvertTo
        private QuantityLength(Quantity<LengthUnitMeasurable> inner)
        {
            _inner = inner;
        }

        public QuantityLength ConvertTo(LengthUnit targetUnit)
        {
            return new QuantityLength(_inner.ConvertTo(new LengthUnitMeasurable(targetUnit)));
        }

        public QuantityLength Add(QuantityLength other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            return new QuantityLength(Quantity<LengthUnitMeasurable>.Add(_inner, other._inner));
        }

        public static QuantityLength Add(QuantityLength a, QuantityLength b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return a.Add(b);
        }

        public static QuantityLength Add(QuantityLength a, QuantityLength b, LengthUnit targetUnit)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Target unit must be a valid LengthUnit.", nameof(targetUnit));
            return new QuantityLength(
                Quantity<LengthUnitMeasurable>.Add(a._inner, b._inner, new LengthUnitMeasurable(targetUnit)));
        }

        public static QuantityLength Add(QuantityLength a, QuantityLength b, LengthUnit? targetUnit)
        {
            if (targetUnit is null)
                throw new ArgumentException("Target unit must be provided.", nameof(targetUnit));
            return Add(a, b, targetUnit.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not QuantityLength other) return false;
            return _inner.Equals(other._inner);
        }

        public override int GetHashCode() => _inner.GetHashCode();

        public override string ToString() => $"{Value} {Unit}";
    }
}