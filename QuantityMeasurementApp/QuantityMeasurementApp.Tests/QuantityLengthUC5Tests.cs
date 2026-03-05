using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC5: Unit-to-unit conversion (same measurement type) tests.
    /// Covers static Convert API, instance ConvertTo, validation, precision, and round-trip.
    /// </summary>
    [TestFixture]
    public class QuantityLengthUC5Tests
    {
        private const double Epsilon = 1e-6;

        // --------------------- Basic unit conversion ---------------------

        [Test]
        public void testConversion_FeetToInches()
        {
            double result = QuantityLengthService.Convert(1.0, LengthUnit.Feet, LengthUnit.Inch);
            Assert.That(result, Is.EqualTo(12.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_InchesToFeet()
        {
            double result = QuantityLengthService.Convert(24.0, LengthUnit.Inch, LengthUnit.Feet);
            Assert.That(result, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_YardsToInches()
        {
            double result = QuantityLengthService.Convert(1.0, LengthUnit.Yard, LengthUnit.Inch);
            Assert.That(result, Is.EqualTo(36.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_InchesToYards()
        {
            double result = QuantityLengthService.Convert(72.0, LengthUnit.Inch, LengthUnit.Yard);
            Assert.That(result, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_CentimetersToInches()
        {
            double result = QuantityLengthService.Convert(2.54, LengthUnit.Centimeter, LengthUnit.Inch);
            Assert.That(result, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_FeetToYards()
        {
            double result = QuantityLengthService.Convert(6.0, LengthUnit.Feet, LengthUnit.Yard);
            Assert.That(result, Is.EqualTo(2.0).Within(Epsilon));
        }

        // --------------------- Round-trip and same-unit ---------------------

        [Test]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double v = 7.5;
            double aToB = QuantityLengthService.Convert(v, LengthUnit.Feet, LengthUnit.Inch);
            double bToA = QuantityLengthService.Convert(aToB, LengthUnit.Inch, LengthUnit.Feet);
            Assert.That(bToA, Is.EqualTo(v).Within(Epsilon));
        }

        [Test]
        public void testConversion_SameUnit_ReturnsSameValue()
        {
            double result = QuantityLengthService.Convert(5.0, LengthUnit.Feet, LengthUnit.Feet);
            Assert.That(result, Is.EqualTo(5.0).Within(Epsilon));
        }

        // --------------------- Zero and negative ---------------------

        [Test]
        public void testConversion_ZeroValue()
        {
            double result = QuantityLengthService.Convert(0.0, LengthUnit.Feet, LengthUnit.Inch);
            Assert.That(result, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            double result = QuantityLengthService.Convert(-1.0, LengthUnit.Feet, LengthUnit.Inch);
            Assert.That(result, Is.EqualTo(-12.0).Within(Epsilon));
        }

        // --------------------- Invalid input: NaN / Infinite ---------------------

        [Test]
        public void testConversion_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                QuantityLengthService.Convert(double.NaN, LengthUnit.Feet, LengthUnit.Inch));
        }

        [Test]
        public void testConversion_PositiveInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                QuantityLengthService.Convert(double.PositiveInfinity, LengthUnit.Feet, LengthUnit.Inch));
        }

        [Test]
        public void testConversion_NegativeInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                QuantityLengthService.Convert(double.NegativeInfinity, LengthUnit.Feet, LengthUnit.Inch));
        }

        // --------------------- Invalid unit (invalid enum value) ---------------------

        [Test]
        public void testConversion_InvalidSourceUnit_Throws()
        {
            var invalidUnit = (LengthUnit)(-1);
            Assert.Throws<ArgumentException>(() =>
                QuantityLengthService.Convert(1.0, invalidUnit, LengthUnit.Inch));
        }

        [Test]
        public void testConversion_InvalidTargetUnit_Throws()
        {
            var invalidUnit = (LengthUnit)99;
            Assert.Throws<ArgumentException>(() =>
                QuantityLengthService.Convert(1.0, LengthUnit.Feet, invalidUnit));
        }

        // --------------------- Precision tolerance ---------------------

        [Test]
        public void testConversion_PrecisionTolerance()
        {
            double result = QuantityLengthService.Convert(1.0, LengthUnit.Centimeter, LengthUnit.Inch);
            Assert.That(result, Is.EqualTo(0.393701).Within(QuantityLengthService.DefaultEpsilon));
        }

        // --------------------- Instance method ConvertTo ---------------------

        [Test]
        public void testConversion_InstanceConvertTo_FeetToInches()
        {
            var length = new QuantityLength(3.0, LengthUnit.Feet);
            QuantityLength converted = length.ConvertTo(LengthUnit.Inch);
            Assert.That(converted.Equals(new QuantityLength(36.0, LengthUnit.Inch)));
        }

        [Test]
        public void testConversion_InstanceConvertTo_YardsToFeet()
        {
            var length = new QuantityLength(2.0, LengthUnit.Yard);
            QuantityLength converted = length.ConvertTo(LengthUnit.Feet);
            Assert.That(converted.Equals(new QuantityLength(6.0, LengthUnit.Feet)));
        }

        [Test]
        public void testConversion_InstanceConvertTo_ReturnsNewInstance()
        {
            var length = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength converted = length.ConvertTo(LengthUnit.Inch);
            Assert.AreNotSame(length, converted);
            Assert.That(converted.Equals(new QuantityLength(12.0, LengthUnit.Inch)));
        }

        // --------------------- ToString ---------------------

        [Test]
        public void testConversion_ToString_Readable()
        {
            var length = new QuantityLength(3.5, LengthUnit.Feet);
            string s = length.ToString();
            Assert.That(s, Does.Contain("3.5").And.Contain("Feet"));
        }
    }
}
