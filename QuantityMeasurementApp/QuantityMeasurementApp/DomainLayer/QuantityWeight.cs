using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC10 backward-compatibility wrapper.
    /// Delegates all logic to Quantity&lt;WeightUnitMeasurable&gt; so that
    /// all UC9 test files compile and pass without any changes.
    /// </summary>
    public class QuantityWeight
    {
        public const double MaxWeightValue = 100000;

        // internal generic instance
        private readonly Quantity<WeightUnitMeasurable> _inner;

        public double Value => _inner.Value;
        public WeightUnit Unit => _inner.Unit.Unit;

        public QuantityWeight(double value, WeightUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException(
                    "Value must be a finite number; NaN and infinity are not allowed.",
                    nameof(value));
            if (!Enum.IsDefined(typeof(WeightUnit), unit))
                throw new ArgumentException("Invalid weight unit type", nameof(unit));

            _inner = new Quantity<WeightUnitMeasurable>(value, new WeightUnitMeasurable(unit));
        }

        // private constructor used internally by Add / ConvertTo
        private QuantityWeight(Quantity<WeightUnitMeasurable> inner)
        {
            _inner = inner;
        }

        public QuantityWeight ConvertTo(WeightUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Invalid weight unit type", nameof(targetUnit));
            return new QuantityWeight(_inner.ConvertTo(new WeightUnitMeasurable(targetUnit)));
        }

        public QuantityWeight Add(QuantityWeight other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            return new QuantityWeight(Quantity<WeightUnitMeasurable>.Add(_inner, other._inner));
        }

        public static QuantityWeight Add(QuantityWeight a, QuantityWeight b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return a.Add(b);
        }

        public static QuantityWeight Add(QuantityWeight a, QuantityWeight b, WeightUnit targetUnit)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Target unit must be a valid WeightUnit.", nameof(targetUnit));
            return new QuantityWeight(
                Quantity<WeightUnitMeasurable>.Add(a._inner, b._inner, new WeightUnitMeasurable(targetUnit)));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not QuantityWeight other) return false;
            return _inner.Equals(other._inner);
        }

        public override int GetHashCode() => _inner.GetHashCode();

        public override string ToString() => $"{Value} {Unit}";
    }
}