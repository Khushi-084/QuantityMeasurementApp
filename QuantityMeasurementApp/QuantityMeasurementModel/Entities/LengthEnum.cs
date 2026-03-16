using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC8: Standalone length unit enum with conversion responsibility.
    /// Base unit is FEET. All conversions normalize through feet.
    /// </summary>
    public enum LengthEnum
    {
        FEET,
        INCH,
        YARD,
        CENTIMETER
    }

}
