using System;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// Generic Quantity class
    /// Base unit: FEET
    /// </summary>
    public class QuantityLength
    {
        private readonly double value;
        private readonly LengthEnum unit;

        public double Value => value;
        public LengthEnum Unit => unit;

        // Constructor
        public QuantityLength(double value, LengthEnum unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid numeric value.");

            if (!Enum.IsDefined(typeof(LengthEnum), unit))
                throw new ArgumentException("Invalid unit provided.");

            this.value = value;
            this.unit = unit;
        }

        // ================= CONVERSION =================

        public double ConvertToFeet()
        {
            return unit.ConvertToBaseUnit(value);
        }

        private double ConvertToBaseUnit()
        {
            return unit.ConvertToBaseUnit(value);
        }

        public QuantityLength ConvertTo(LengthEnum targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthEnum), targetUnit))
                throw new ArgumentException("Invalid or unsupported target unit.");

            double valueInFeet = unit.ConvertToBaseUnit(value);
            double convertedValue = targetUnit.ConvertFromBaseUnit(valueInFeet);
            double rounded = Math.Round(convertedValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityLength(rounded, targetUnit);
        }

        public static double Convert(double value, LengthEnum sourceUnit, LengthEnum targetUnit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Invalid value: must be finite.");

            if (!Enum.IsDefined(typeof(LengthEnum), sourceUnit))
                throw new ArgumentException("Invalid source unit.");

            if (!Enum.IsDefined(typeof(LengthEnum), targetUnit))
                throw new ArgumentException("Invalid target unit.");

            double valueInFeet = sourceUnit.ConvertToBaseUnit(value);
            double result = targetUnit.ConvertFromBaseUnit(valueInFeet);

            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
        }

        // ================= ADDITION =================

        public QuantityLength Add(QuantityLength thatLength)
        {
            return AddAndConvert(thatLength, unit);
        }

        public QuantityLength Add(QuantityLength thatLength, LengthEnum targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthEnum), targetUnit))
                throw new ArgumentException("Invalid target unit.");

            return AddAndConvert(thatLength, targetUnit);
        }

        private QuantityLength AddAndConvert(QuantityLength thatLength, LengthEnum targetUnit)
        {
            if (thatLength == null)
                throw new ArgumentNullException(nameof(thatLength));

            double thisInFeet  = unit.ConvertToBaseUnit(value);
            double thatInFeet  = thatLength.ConvertToFeet();
            double sumInFeet   = thisInFeet + thatInFeet;

            double resultValue = targetUnit.ConvertFromBaseUnit(sumInFeet);
            double rounded     = Math.Round(resultValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityLength(rounded, targetUnit);
        }

        // ================= EQUALITY =================

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            QuantityLength other = (QuantityLength)obj;

            double thisFeet  = ConvertToFeet();
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