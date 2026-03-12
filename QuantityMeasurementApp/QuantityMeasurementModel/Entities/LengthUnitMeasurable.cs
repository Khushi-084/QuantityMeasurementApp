using System;
using QuantityMeasurementModel.Interfaces;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC10: LengthUnit reimplemented as a class-based enum implementing IMeasurable.
    /// Replaces the separate enum + extension methods pattern.
    /// Base unit: FEET.
    /// </summary>
    public class LengthUnitM : IMeasurable
    {
        public static readonly LengthUnitM FEET        = new LengthUnitM("FEET",        1.0);
        public static readonly LengthUnitM INCHES      = new LengthUnitM("INCHES",      1.0 / 12.0);
        public static readonly LengthUnitM YARDS       = new LengthUnitM("YARDS",       3.0);
        public static readonly LengthUnitM CENTIMETERS = new LengthUnitM("CENTIMETERS", 0.0328084);

        private readonly string name;
        private readonly double factor; // multiply by this to get base unit (feet)

        private LengthUnitM(string name, double factor)
        {
            this.name = name;
            this.factor = factor;
        }

        public double GetConversionFactor()   => factor;
        public double ConvertToBaseUnit(double value)   => value * factor;
        public double ConvertFromBaseUnit(double baseValue)
        {
            if (Math.Abs(factor) < 1e-15)
                throw new ArgumentException("Unsupported unit");
            return baseValue / factor;
        }
        public string GetUnitName() => name;

        public override string ToString() => name;
    }
}
