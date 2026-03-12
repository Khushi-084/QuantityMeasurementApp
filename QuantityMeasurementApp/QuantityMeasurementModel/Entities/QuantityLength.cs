using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Generic Quantity class (UC3, UC4, UC5)
    /// Handles all length units using DRY principle.
    /// UC5: Provides unit-to-unit conversion via ConvertTo (instance) and Convert (static).
    /// Base unit: FEET. All conversions normalize through feet, rounded to 2 decimal places.
    /// </summary>
    public class QuantityLength
    {
        private readonly double value;
        private readonly LengthUnit unit;

        /// <summary>Numeric value in the quantity's unit. UC8: Exposed for tests and display.</summary>
        public double Value => value;
        /// <summary>Unit of this quantity. UC8: Exposed for tests and display.</summary>
        public LengthUnit Unit => unit;

        // Constructor
        public QuantityLength(double value, LengthUnit unit){
        // numeric validation
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Invalid numeric value.");

        // UNIT VALIDATION (THIS FIXES YOUR TEST)
        if (!Enum.IsDefined(typeof(LengthUnit), unit))
            throw new ArgumentException("Invalid unit provided.");

        this.value = value;
        this.unit = unit;
    }

        // ================= CONVERSION (UC8: delegate to unit) =================
        /// <summary>
        /// Converts this quantity to base unit (feet). UC8: Delegates to unit.ConvertToBaseUnit.
        /// </summary>
        public double ConvertToFeet()
        {
            return unit.ConvertToBaseUnit(value);
        }

        /// <summary>
        /// Private: value in feet. UC8: Delegates to unit.
        /// </summary>
        private double ConvertToBaseUnit()
        {
            return unit.ConvertToBaseUnit(value);
        }

        /// <summary>
        /// UC5: Converts this length to the specified target unit.
        /// UC8: Delegates to unit.ConvertToBaseUnit and targetUnit.ConvertFromBaseUnit.
        /// </summary>
        public QuantityLength ConvertTo(LengthUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target unit.");

            double valueInFeet = unit.ConvertToBaseUnit(value);
            double convertedValue = targetUnit.ConvertFromBaseUnit(valueInFeet);
            double rounded = Math.Round(convertedValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityLength(rounded, targetUnit);
        }

        /// <summary>
        /// UC5: Static conversion. UC8: Delegates to sourceUnit.ConvertToBaseUnit and targetUnit.ConvertFromBaseUnit.
        /// </summary>
        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Invalid value: must be finite (not NaN or Infinity).");

            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit))
                throw new ArgumentException("Invalid or unsupported source unit.");

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target unit.");

            double valueInFeet = sourceUnit.ConvertToBaseUnit(value);
            double result = targetUnit.ConvertFromBaseUnit(valueInFeet);
            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
        }

        // ================= UC6/UC7: ADDITION =================

        /// <summary>
        /// UC6: Adds another QuantityLength to this one. Result in unit of first operand.
        /// </summary>
        public QuantityLength Add(QuantityLength thatLength)
        {
            return AddAndConvert(thatLength, unit);
        }

        /// <summary>
        /// UC7: Adds another QuantityLength to this one with explicit target unit.
        /// Result is returned in the specified target unit.
        /// </summary>
        /// <param name="thatLength">The QuantityLength to add. Must not be null.</param>
        /// <param name="targetUnit">The unit for the result. Must not be null and must be valid.</param>
        public QuantityLength Add(QuantityLength thatLength, LengthUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target unit.");

            return AddAndConvert(thatLength, targetUnit);
        }

        /// <summary>
        /// UC8: Converts both to base unit via unit, sums, converts result via targetUnit.ConvertFromBaseUnit.
        /// </summary>
        private QuantityLength AddAndConvert(QuantityLength thatLength, LengthUnit targetUnit)
        {
            if (thatLength == null)
                throw new ArgumentNullException(nameof(thatLength), "Cannot add null QuantityLength.");

            if (!double.IsFinite(value) || !double.IsFinite(thatLength.ConvertToFeet()))
                throw new ArgumentException("Both values must be finite (not NaN or Infinity).");

            double thisInFeet = unit.ConvertToBaseUnit(value);
            double thatInFeet = thatLength.ConvertToFeet();
            double sumInFeet = thisInFeet + thatInFeet;

            double resultValue = targetUnit.ConvertFromBaseUnit(sumInFeet);
            double rounded = Math.Round(resultValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityLength(rounded, targetUnit);
        }

        // ================= EQUALITY =================

        public override bool Equals(object? obj)
        {
            if (this == obj)
                return true;

            if (obj == null || GetType() != obj.GetType())
                return false;

            QuantityLength other = (QuantityLength)obj;

            double thisFeet = ConvertToFeet();
            double otherFeet = other.ConvertToFeet();

            return Math.Abs(thisFeet - otherFeet) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ConvertToFeet().GetHashCode();
        }

        public override string ToString()
        {
            return $"{value} {unit}";
        }
    }
}
