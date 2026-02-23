namespace QuantityMeasurementApp.DomainLayer
{
    public enum LengthUnit
    {
        Feet,
        Inch
    }

    public static class LengthUnitExtensions
    {
        public static double ToFeet(this LengthUnit unit)     //extension method to add new functionality to existing types
        {
            return unit switch
            {
                LengthUnit.Feet => 1.0,
                LengthUnit.Inch => 1.0 / 12.0,
                _ => throw new System.ArgumentException("Unsupported unit")    //default case 
            };
        }
    }
}
