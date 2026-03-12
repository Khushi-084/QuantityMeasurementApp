using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC9: Generic weight quantity using WeightUnit.
    /// Base unit: KILOGRAM. All conversions normalize through kilograms.
    /// </summary>
    public class QuantityWeight
    {
        private readonly double value;
        private readonly WeightUnit unit;

        public double Value => value;
        public WeightUnit Unit => unit;

        public QuantityWeight(double value, WeightUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid numeric value.");

            if (!Enum.IsDefined(typeof(WeightUnit), unit))
                throw new ArgumentException("Invalid weight unit provided.");

            this.value = value;
            this.unit = unit;
        }

        // ===== Conversion to base unit (kilogram) =====

        public double ConvertToKilograms()
        {
            return unit.ConvertToBaseUnit(value);
        }

        private double ConvertToBaseUnit()
        {
            return unit.ConvertToBaseUnit(value);
        }

        public QuantityWeight ConvertTo(WeightUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported weight target unit.");

            double valueInKg = unit.ConvertToBaseUnit(value);
            double converted = targetUnit.ConvertFromBaseUnit(valueInKg);
            double rounded = Math.Round(converted, 2, MidpointRounding.AwayFromZero);

            return new QuantityWeight(rounded, targetUnit);
        }

        public static double Convert(double value, WeightUnit sourceUnit, WeightUnit targetUnit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Invalid value: must be finite (not NaN or Infinity).");

            if (!Enum.IsDefined(typeof(WeightUnit), sourceUnit))
                throw new ArgumentException("Invalid or unsupported source weight unit.");

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target weight unit.");

            double valueInKg = sourceUnit.ConvertToBaseUnit(value);
            double result = targetUnit.ConvertFromBaseUnit(valueInKg);
            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
        }

        // ===== Addition =====

        public QuantityWeight Add(QuantityWeight that)
        {
            return AddAndConvert(that, unit);
        }

        public QuantityWeight Add(QuantityWeight that, WeightUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported weight target unit.");

            return AddAndConvert(that, targetUnit);
        }

        private QuantityWeight AddAndConvert(QuantityWeight that, WeightUnit targetUnit)
        {
            if (that == null)
                throw new ArgumentNullException(nameof(that), "Cannot add null QuantityWeight.");

            if (!double.IsFinite(value) || !double.IsFinite(that.value))
                throw new ArgumentException("Both weight values must be finite (not NaN or Infinity).");

            double thisKg = ConvertToBaseUnit();
            double thatKg = that.ConvertToBaseUnit();
            double sumKg = thisKg + thatKg;

            double resultValue = targetUnit.ConvertFromBaseUnit(sumKg);
            double rounded = Math.Round(resultValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityWeight(rounded, targetUnit);
        }

        // ===== Equality =====

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj == null || GetType() != obj.GetType())
                return false; // category safety: no comparison with other quantity types

            QuantityWeight other = (QuantityWeight)obj;

            double thisKg = ConvertToBaseUnit();
            double otherKg = other.ConvertToBaseUnit();

            return Math.Abs(thisKg - otherKg) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ConvertToBaseUnit().GetHashCode();
        }

        public override string ToString()
        {
            return $"{value} {unit}";
        }
    }
}

