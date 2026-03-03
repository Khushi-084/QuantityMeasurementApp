using System;

namespace QuantityMeasurementApp.Domain
{
    /// <summary>
    /// UC9: Immutable value object representing a weight measurement in a given unit.
    /// Base unit is kilogram.
    /// </summary>
    public class QuantityWeight
    {
        // Keep constant for PresentationLayer compatibility
        public const double MaxWeightValue = 100000;

        private readonly double value;
        private readonly WeightUnit unit;

        public double Value => value;
        public WeightUnit Unit => unit;

        public QuantityWeight(double value, WeightUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException(
                    "Value must be a finite number; NaN and infinity are not allowed.",
                    nameof(value));

            if (!Enum.IsDefined(typeof(WeightUnit), unit))
                throw new ArgumentException("Invalid weight unit type", nameof(unit));

            this.value = value;
            this.unit = unit;
        }

        public QuantityWeight Add(QuantityWeight other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            double sumInBase =
                unit.ConvertToBaseUnit(value) +
                other.unit.ConvertToBaseUnit(other.value);

            double resultValue = unit.ConvertFromBaseUnit(sumInBase);

            return new QuantityWeight(resultValue, unit);
        }

        public static QuantityWeight Add(QuantityWeight weight1, QuantityWeight weight2)
        {
            if (weight1 is null)
                throw new ArgumentNullException(nameof(weight1));
            if (weight2 is null)
                throw new ArgumentNullException(nameof(weight2));

            return weight1.Add(weight2);
        }

        public static QuantityWeight Add(
            QuantityWeight weight1,
            QuantityWeight weight2,
            WeightUnit targetUnit)
        {
            if (weight1 is null)
                throw new ArgumentNullException(nameof(weight1));
            if (weight2 is null)
                throw new ArgumentNullException(nameof(weight2));
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException(
                    "Target unit must be a valid WeightUnit.",
                    nameof(targetUnit));

            double sumInBase =
                weight1.unit.ConvertToBaseUnit(weight1.value) +
                weight2.unit.ConvertToBaseUnit(weight2.value);

            double resultValue = targetUnit.ConvertFromBaseUnit(sumInBase);

            return new QuantityWeight(resultValue, targetUnit);
        }

        public QuantityWeight ConvertTo(WeightUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Invalid weight unit type", nameof(targetUnit));

            double valueInBase = unit.ConvertToBaseUnit(value);
            double converted = targetUnit.ConvertFromBaseUnit(valueInBase);

            return new QuantityWeight(converted, targetUnit);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not QuantityWeight other)
                return false;

            double thisInBase = unit.ConvertToBaseUnit(value);
            double otherInBase = other.unit.ConvertToBaseUnit(other.value);

            return Math.Abs(thisInBase - otherInBase) < 1e-4;
        }

        public override int GetHashCode()
        {
            double normalized = unit.ConvertToBaseUnit(value);
            return Math.Round(normalized, 4).GetHashCode();
        }

        public override string ToString()
        {
            return $"{value} {unit}";
        }
    }
}