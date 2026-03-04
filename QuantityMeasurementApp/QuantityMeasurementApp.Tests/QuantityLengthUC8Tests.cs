using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC8: Refactoring Unit Enum to Standalone with Conversion Responsibility.
    /// Tests LengthUnit conversion methods, QuantityLength refactored design, and backward compatibility.
    /// </summary>
    [TestFixture]
    public class QuantityLengthUC8Tests
    {
        private const double Epsilon = 1e-6;

        // --------------------- LengthUnit Enum Constants ---------------------

        [Test]
        public void testLengthUnitEnum_FeetConstant()
        {
            Assert.That(LengthUnit.Feet.GetConversionFactor(), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testLengthUnitEnum_InchesConstant()
        {
            Assert.That(LengthUnit.Inch.GetConversionFactor(), Is.EqualTo(1.0 / 12.0).Within(Epsilon));
        }

        [Test]
        public void testLengthUnitEnum_YardsConstant()
        {
            Assert.That(LengthUnit.Yard.GetConversionFactor(), Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testLengthUnitEnum_CentimetersConstant()
        {
            Assert.That(LengthUnit.Centimeter.GetConversionFactor(), Is.EqualTo(1.0 / 30.48).Within(Epsilon));
        }

        // --------------------- ConvertToBaseUnit ---------------------

        [Test]
        public void testConvertToBaseUnit_FeetToFeet()
        {
            Assert.That(LengthUnit.Feet.ConvertToBaseUnit(5.0), Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_InchesToFeet()
        {
            Assert.That(LengthUnit.Inch.ConvertToBaseUnit(12.0), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_YardsToFeet()
        {
            Assert.That(LengthUnit.Yard.ConvertToBaseUnit(1.0), Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_CentimetersToFeet()
        {
            Assert.That(LengthUnit.Centimeter.ConvertToBaseUnit(30.48), Is.EqualTo(1.0).Within(Epsilon));
        }

        // --------------------- ConvertFromBaseUnit ---------------------

        [Test]
        public void testConvertFromBaseUnit_FeetToFeet()
        {
            Assert.That(LengthUnit.Feet.ConvertFromBaseUnit(2.0), Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToInches()
        {
            Assert.That(LengthUnit.Inch.ConvertFromBaseUnit(1.0), Is.EqualTo(12.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToYards()
        {
            Assert.That(LengthUnit.Yard.ConvertFromBaseUnit(3.0), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToCentimeters()
        {
            Assert.That(LengthUnit.Centimeter.ConvertFromBaseUnit(1.0), Is.EqualTo(30.48).Within(Epsilon));
        }

        // --------------------- QuantityLength Refactored ---------------------

        [Test]
        public void testQuantityLengthRefactored_Equality()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testQuantityLengthRefactored_ConvertTo()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength converted = a.ConvertTo(LengthUnit.Inch);
            Assert.That(converted.Value, Is.EqualTo(12.0).Within(Epsilon));
            Assert.That(converted.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testQuantityLengthRefactored_Add()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Feet);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testQuantityLengthRefactored_AddWithTargetUnit()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Yard);
            Assert.That(result.Value, Is.EqualTo(2.0 / 3.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Yard));
        }

        [Test]
        public void testQuantityLengthRefactored_NullUnit()
        {
            Assert.Throws<ArgumentException>(() => new QuantityLength(1.0, (LengthUnit)(-1)));
        }

        [Test]
        public void testQuantityLengthRefactored_InvalidValue()
        {
            Assert.Throws<ArgumentException>(() => new QuantityLength(double.NaN, LengthUnit.Feet));
        }

        // --------------------- Round-trip Conversion ---------------------

        [Test]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double original = 7.5;
            double aToB = QuantityLengthService.Convert(original, LengthUnit.Feet, LengthUnit.Inch);
            double bToA = QuantityLengthService.Convert(aToB, LengthUnit.Inch, LengthUnit.Feet);
            Assert.That(bToA, Is.EqualTo(original).Within(Epsilon));
        }

        // --------------------- Example Output Verification ---------------------

        [Test]
        public void testExampleOutput_ConvertFeetToInches()
        {
            var q = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength result = q.ConvertTo(LengthUnit.Inch);
            Assert.That(result.Value, Is.EqualTo(12.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testExampleOutput_AddFeetAndInches()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Feet);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testExampleOutput_EqualityInchesAndYards()
        {
            var a = new QuantityLength(36.0, LengthUnit.Inch);
            var b = new QuantityLength(1.0, LengthUnit.Yard);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testExampleOutput_AddYardsAndFeet()
        {
            var a = new QuantityLength(1.0, LengthUnit.Yard);
            var b = new QuantityLength(3.0, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Yard);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Yard));
        }

        [Test]
        public void testExampleOutput_CentimetersToInches()
        {
            var q = new QuantityLength(2.54, LengthUnit.Centimeter);
            QuantityLength result = q.ConvertTo(LengthUnit.Inch);
            Assert.That(result.Value, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testExampleOutput_AddWithZero()
        {
            var a = new QuantityLength(5.0, LengthUnit.Feet);
            var b = new QuantityLength(0.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Feet);
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }
    }
}
