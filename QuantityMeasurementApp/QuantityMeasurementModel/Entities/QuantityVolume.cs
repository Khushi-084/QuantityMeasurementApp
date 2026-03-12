using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC11: Volume quantity entity using VolumeUnit.
    /// Follows the exact same pattern as QuantityWeight (UC9).
    /// Base unit: LITRE. All conversions normalize through litres.
    /// Supports: LITRE, MILLILITRE, GALLON.
    /// </summary>
    public class QuantityVolume
    {
        private readonly double value;
        private readonly VolumeUnit unit;

        public double Value => value;
        public VolumeUnit Unit => unit;

        public QuantityVolume(double value, VolumeUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid numeric value.");

            if (!Enum.IsDefined(typeof(VolumeUnit), unit))
                throw new ArgumentException("Invalid volume unit provided.");

            this.value = value;
            this.unit = unit;
        }

        // ===== Conversion to base unit (litre) =====

        public double ConvertToLitres()
        {
            return unit.ConvertToBaseUnit(value);
        }

        private double ConvertToBaseUnit()
        {
            return unit.ConvertToBaseUnit(value);
        }

        /// <summary>Converts this volume to the specified target unit.</summary>
        public QuantityVolume ConvertTo(VolumeUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(VolumeUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported volume target unit.");

            double valueInLitres = unit.ConvertToBaseUnit(value);
            double converted = targetUnit.ConvertFromBaseUnit(valueInLitres);
            double rounded = Math.Round(converted, 2, MidpointRounding.AwayFromZero);

            return new QuantityVolume(rounded, targetUnit);
        }

        /// <summary>Static conversion between volume units.</summary>
        public static double Convert(double value, VolumeUnit sourceUnit, VolumeUnit targetUnit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Invalid value: must be finite (not NaN or Infinity).");

            if (!Enum.IsDefined(typeof(VolumeUnit), sourceUnit))
                throw new ArgumentException("Invalid or unsupported source volume unit.");

            if (!Enum.IsDefined(typeof(VolumeUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target volume unit.");

            double valueInLitres = sourceUnit.ConvertToBaseUnit(value);
            double result = targetUnit.ConvertFromBaseUnit(valueInLitres);
            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
        }

        // ===== Addition =====

        /// <summary>UC11: Adds another QuantityVolume. Result in unit of first operand.</summary>
        public QuantityVolume Add(QuantityVolume that)
        {
            return AddAndConvert(that, unit);
        }

        /// <summary>UC11: Adds another QuantityVolume with explicit target unit.</summary>
        public QuantityVolume Add(QuantityVolume that, VolumeUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(VolumeUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported volume target unit.");

            return AddAndConvert(that, targetUnit);
        }

        private QuantityVolume AddAndConvert(QuantityVolume that, VolumeUnit targetUnit)
        {
            if (that == null)
                throw new ArgumentNullException(nameof(that), "Cannot add null QuantityVolume.");

            if (!double.IsFinite(value) || !double.IsFinite(that.value))
                throw new ArgumentException("Both volume values must be finite (not NaN or Infinity).");

            double thisLitres = ConvertToBaseUnit();
            double thatLitres = that.ConvertToBaseUnit();
            double sumLitres  = thisLitres + thatLitres;

            double resultValue = targetUnit.ConvertFromBaseUnit(sumLitres);
            double rounded = Math.Round(resultValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityVolume(rounded, targetUnit);
        }

        // ===== Equality =====

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj == null || GetType() != obj.GetType())
                return false;

            QuantityVolume other = (QuantityVolume)obj;

            double thisLitres  = ConvertToBaseUnit();
            double otherLitres = other.ConvertToBaseUnit();

            return Math.Abs(thisLitres - otherLitres) < 0.0001;
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
