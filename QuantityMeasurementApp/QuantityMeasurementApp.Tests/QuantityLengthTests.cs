using NUnit.Framework;
using QuantityMeasurementApp.DomainLayer;

namespace QuantityMeasurementApp.Tests
{
    public class QuantityLengthTests
    {
        [Test]
        public void Feet_To_Feet_SameValue_ShouldBeEqual()
        {
            var quantityLength1 = new QuantityLength(1.0, LengthUnit.Feet);
            var quantityLength2 = new QuantityLength(1.0, LengthUnit.Feet);

            Assert.That(quantityLength1.Equals(quantityLength2), Is.True);
        }

        [Test]
        public void Inch_To_Inch_SameValue_ShouldBeEqual()
        {
            var quantityLength1 = new QuantityLength(1.0, LengthUnit.Inch);
            var quantityLength2 = new QuantityLength(1.0, LengthUnit.Inch);

            Assert.That(quantityLength1.Equals(quantityLength2), Is.True);
        }

        [Test]
        public void Feet_To_Inch_Equivalent_ShouldBeEqual()
        {
            var quantityLength1 = new QuantityLength(1.0, LengthUnit.Feet);
            var quantityLength2 = new QuantityLength(12.0, LengthUnit.Inch);

            Assert.That(quantityLength1.Equals(quantityLength2), Is.True);
        }

        [Test]
        public void Different_Feet_ShouldNotBeEqual()
        {
            var quantityLength1 = new QuantityLength(1.0, LengthUnit.Feet);
            var quantityLength2 = new QuantityLength(2.0, LengthUnit.Feet);

            Assert.That(quantityLength1.Equals(quantityLength2), Is.False);
        }

        [Test]
        public void Negative_Value_ShouldThrowException()
        {
            Assert.Throws<System.ArgumentException>(() =>
                new QuantityLength(-1, LengthUnit.Feet));
        }

        [Test]
        public void Very_Large_Value_ShouldThrowException()
        {
            Assert.Throws<System.ArgumentException>(() =>
                new QuantityLength(20000, LengthUnit.Feet));
        }

        [Test]
        public void Null_Comparison_ShouldReturnFalse()
        {
            var quantityLength1 = new QuantityLength(1, LengthUnit.Feet);

            Assert.That(quantityLength1.Equals(null), Is.False);
        }
    }
}