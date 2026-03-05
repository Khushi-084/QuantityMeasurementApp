using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC11: QuantityVolume wrapper — mirrors QuantityLength and QuantityWeight.
    /// Adds range validation (MaxVolumeValue = 100000) and enum validation
    /// on top of the generic Quantity&lt;VolumeUnitMeasurable&gt;.
    /// </summary>
    public class QuantityVolume
    {
        public const double MaxVolumeValue = 100000;

        private readonly Quantity<VolumeUnitMeasurable> _inner;

        public double     Value => _inner.Value;
        public VolumeUnit Unit  => _inner.Unit.Unit;

        public QuantityVolume(double value, VolumeUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException(
                    "Value must be a finite number; NaN and infinity are not allowed.",
                    nameof(value));

            if (Math.Abs(value) > MaxVolumeValue)
                throw new ArgumentException(
                    $"Value must be between -{MaxVolumeValue:N0} and {MaxVolumeValue:N0}.",
                    nameof(value));

            if (!Enum.IsDefined(typeof(VolumeUnit), unit))
                throw new ArgumentException("Invalid volume unit type", nameof(unit));

            _inner = new Quantity<VolumeUnitMeasurable>(value, new VolumeUnitMeasurable(unit));
        }

        // private constructor used internally by Add / ConvertTo
        private QuantityVolume(Quantity<VolumeUnitMeasurable> inner)
        {
            _inner = inner;
        }

        public QuantityVolume ConvertTo(VolumeUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(VolumeUnit), targetUnit))
                throw new ArgumentException("Invalid volume unit type", nameof(targetUnit));
            return new QuantityVolume(_inner.ConvertTo(new VolumeUnitMeasurable(targetUnit)));
        }

        public QuantityVolume Add(QuantityVolume other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            return new QuantityVolume(Quantity<VolumeUnitMeasurable>.Add(_inner, other._inner));
        }

        public static QuantityVolume Add(QuantityVolume a, QuantityVolume b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return a.Add(b);
        }

        public static QuantityVolume Add(QuantityVolume a, QuantityVolume b, VolumeUnit targetUnit)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            if (!Enum.IsDefined(typeof(VolumeUnit), targetUnit))
                throw new ArgumentException("Target unit must be a valid VolumeUnit.", nameof(targetUnit));
            return new QuantityVolume(
                Quantity<VolumeUnitMeasurable>.Add(a._inner, b._inner, new VolumeUnitMeasurable(targetUnit)));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not QuantityVolume other) return false;
            return _inner.Equals(other._inner);
        }

        public override int GetHashCode() => _inner.GetHashCode();

        public override string ToString() => $"{Value} {Unit}";
    }
}