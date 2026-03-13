using System;

namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC10: WeightUnit reimplemented as a class-based enum implementing IMeasurable.
    /// Base unit: KILOGRAM.
    /// </summary>
    public class WeightUnitM : IMeasurable
    {
        public static readonly WeightUnitM KILOGRAM = new WeightUnitM("KILOGRAM", 1.0);
        public static readonly WeightUnitM GRAM     = new WeightUnitM("GRAM",     0.001);
        public static readonly WeightUnitM POUND    = new WeightUnitM("POUND",    0.453592);

        private readonly string name;
        private readonly double factor; // multiply by this to get base unit (kg)

        private WeightUnitM(string name, double factor)
        {
            this.name = name;
            this.factor = factor;
        }

        public double GetConversionFactor()   => factor;
        public double ConvertToBaseUnit(double value)   => value * factor;
        public double ConvertFromBaseUnit(double baseValue)
        {
            if (Math.Abs(factor) < 1e-15)
                throw new ArgumentException("Unsupported weight unit");
            return baseValue / factor;
        }
        public string GetUnitName() => name;

        public override string ToString() => name;
    }
}
