using System;

namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC12: VolumeUnit reimplemented as a class-based enum implementing IMeasurable.
    /// Enables volume measurements to work with the generic Quantity&lt;U&gt; class (UC10)
    /// for subtraction and division operations introduced in UC12.
    ///
    /// Follows the exact same pattern as LengthUnitM and WeightUnitM.
    /// Base unit: LITRE. All conversions normalise through litres.
    /// Supports: LITRE, MILLILITRE, GALLON.
    /// </summary>
    public class VolumeUnitM : IMeasurable
    {
        public static readonly VolumeUnitM LITRE      = new VolumeUnitM("LITRE",       1.0);
        public static readonly VolumeUnitM MILLILITRE = new VolumeUnitM("MILLILITRE",  0.001);     // 1 mL = 0.001 L
        public static readonly VolumeUnitM GALLON     = new VolumeUnitM("GALLON",      3.78541);   // 1 US gallon = 3.78541 L

        private readonly string name;
        private readonly double factor;  // multiply value by factor to get base unit (litres)

        private VolumeUnitM(string name, double factor)
        {
            this.name   = name;
            this.factor = factor;
        }

        public double GetConversionFactor()           => factor;
        public double ConvertToBaseUnit(double value) => value * factor;

        public double ConvertFromBaseUnit(double baseValue)
        {
            if (Math.Abs(factor) < 1e-15)
                throw new ArgumentException("Unsupported volume unit.");
            return baseValue / factor;
        }

        public string GetUnitName() => name;

        public override string ToString() => name;
    }
}
