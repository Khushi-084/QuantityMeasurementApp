using QuantityMeasurementApp.DomainLayer;

namespace QuantityMeasurementApp.ServiceLayer
{
    public static class QuantityLengthService
    {
        public static bool Compare(double value1, LengthUnit unit1,
                                   double value2, LengthUnit unit2)
        {
            var quantityLength1 = new QuantityLength(value1, unit1);
            var quantityLength2 = new QuantityLength(value2, unit2);

            return quantityLength1.Equals(quantityLength2);
        }
    }
}