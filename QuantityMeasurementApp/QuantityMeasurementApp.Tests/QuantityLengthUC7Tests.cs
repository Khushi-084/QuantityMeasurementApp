using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC7: Addition with explicit target unit specification.
    /// Ensures result unit is always the specified target unit and addition remains commutative.
    /// </summary>
    [TestFixture]
    public class QuantityLengthUC7Tests
    {
        private const double Epsilon = 1e-6;

        [Test]
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Feet);

            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Inches()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Inch);

            Assert.That(result.Value, Is.EqualTo(24.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Yards()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Yard);

            Assert.That(result.Value, Is.EqualTo(2.0 / 3.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Yard));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            var a = new QuantityLength(1.0, LengthUnit.Yard);
            var b = new QuantityLength(3.0, LengthUnit.Feet);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Yard);

            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Yard));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            var a = new QuantityLength(2.0, LengthUnit.Yard);
            var b = new QuantityLength(3.0, LengthUnit.Feet);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Feet);

            Assert.That(result.Value, Is.EqualTo(9.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_FeetFromInchesAndYards()
        {
            var a = new QuantityLength(36.0, LengthUnit.Inch);
            var b = new QuantityLength(1.0, LengthUnit.Yard);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Feet);

            Assert.That(result.Value, Is.EqualTo(6.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            var a = new QuantityLength(2.54, LengthUnit.Centimeter);
            var b = new QuantityLength(1.0, LengthUnit.Inch);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Centimeter);

            Assert.That(result.Value, Is.EqualTo(5.08).Within(1e-5));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Centimeter));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            var a = new QuantityLength(5.0, LengthUnit.Feet);
            var b = new QuantityLength(0.0, LengthUnit.Inch);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Yard);

            Assert.That(result.Value, Is.EqualTo(5.0 / 3.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Yard));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            var a = new QuantityLength(5.0, LengthUnit.Feet);
            var b = new QuantityLength(-2.0, LengthUnit.Feet);

            QuantityLength result = QuantityLength.Add(a, b, LengthUnit.Inch);

            Assert.That(result.Value, Is.EqualTo(36.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);

            QuantityLength result1 = QuantityLength.Add(a, b, LengthUnit.Yard);
            QuantityLength result2 = QuantityLength.Add(b, a, LengthUnit.Yard);

            Assert.That(result1.Unit, Is.EqualTo(LengthUnit.Yard));
            Assert.That(result2.Unit, Is.EqualTo(LengthUnit.Yard));
            Assert.That(result1.Value, Is.EqualTo(result2.Value).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Immutability_OriginalsUnchanged()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);

            _ = QuantityLength.Add(a, b, LengthUnit.Inch);

            Assert.That(a.Value, Is.EqualTo(1.0));
            Assert.That(a.Unit, Is.EqualTo(LengthUnit.Feet));
            Assert.That(b.Value, Is.EqualTo(12.0));
            Assert.That(b.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_NullTargetUnit_Throws()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);

            LengthUnit? target = null;
            Assert.Throws<ArgumentException>(() => QuantityLength.Add(a, b, target));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_InvalidTargetUnit_Throws()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);

            var invalidTarget = (LengthUnit)999;
            Assert.Throws<ArgumentException>(() => QuantityLength.Add(a, b, invalidTarget));
        }
    }
}

