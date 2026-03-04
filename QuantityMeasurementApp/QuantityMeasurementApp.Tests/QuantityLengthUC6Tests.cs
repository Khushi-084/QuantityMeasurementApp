using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC6: Addition of two length units (same category). Same-unit and cross-unit addition,
    /// commutativity, identity, negative values, null/invalid handling, large/small values.
    /// </summary>
    [TestFixture]
    public class QuantityLengthUC6Tests
    {
        private const double Epsilon = 1e-6;

        // --------------------- Same-unit addition ---------------------

        [Test]
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(2.0, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testAddition_SameUnit_InchPlusInch()
        {
            var a = new QuantityLength(6.0, LengthUnit.Inch);
            var b = new QuantityLength(6.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(12.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        // --------------------- Cross-unit addition ---------------------

        [Test]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            var a = new QuantityLength(12.0, LengthUnit.Inch);
            var b = new QuantityLength(1.0, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(24.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            var a = new QuantityLength(1.0, LengthUnit.Yard);
            var b = new QuantityLength(3.0, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Yard));
        }

        [Test]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            var a = new QuantityLength(2.54, LengthUnit.Centimeter);
            var b = new QuantityLength(1.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b);
            // Centimeter conversion uses 0.393701/12; floating-point accumulates small error
            Assert.That(result.Value, Is.EqualTo(5.08).Within(1e-5));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Centimeter));
        }

        // --------------------- Commutativity ---------------------

        [Test]
        public void testAddition_Commutativity()
        {
            var feetFirst = new QuantityLength(1.0, LengthUnit.Feet);
            var inchesFirst = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength result1 = QuantityLength.Add(feetFirst, inchesFirst);
            QuantityLength result2 = QuantityLength.Add(inchesFirst, feetFirst);
            // 1 ft + 12 in = 2 ft; 12 in + 1 ft = 24 in; same physical length
            Assert.That(result1.Equals(result2.ConvertTo(LengthUnit.Feet)));
            Assert.That(result2.Equals(result1.ConvertTo(LengthUnit.Inch)));
        }

        // --------------------- Identity (zero) ---------------------

        [Test]
        public void testAddition_WithZero()
        {
            var a = new QuantityLength(5.0, LengthUnit.Feet);
            var b = new QuantityLength(0.0, LengthUnit.Inch);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // --------------------- Negative values ---------------------

        [Test]
        public void testAddition_NegativeValues()
        {
            var a = new QuantityLength(5.0, LengthUnit.Feet);
            var b = new QuantityLength(-2.0, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // --------------------- Null operand ---------------------

        [Test]
        public void testAddition_NullSecondOperand()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            Assert.Throws<ArgumentNullException>(() => QuantityLength.Add(a, null!));
        }

        [Test]
        public void testAddition_NullFirstOperand()
        {
            var b = new QuantityLength(1.0, LengthUnit.Feet);
            Assert.Throws<ArgumentNullException>(() => QuantityLength.Add(null!, b));
        }

        // --------------------- Large and small values ---------------------

        [Test]
        public void testAddition_LargeValues()
        {
            // Uses values within QuantityLength.MaxLengthValue (100,000)
            var a = new QuantityLength(50000, LengthUnit.Feet);
            var b = new QuantityLength(50000, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(100000).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testAddition_SmallValues()
        {
            var a = new QuantityLength(0.001, LengthUnit.Feet);
            var b = new QuantityLength(0.002, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(0.003).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // --------------------- Instance method Add ---------------------

        [Test]
        public void testAddition_InstanceMethod_ResultInFirstOperandUnit()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(12.0, LengthUnit.Inch);
            QuantityLength result = a.Add(b);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testAddition_Immutability_OriginalsUnchanged()
        {
            var a = new QuantityLength(1.0, LengthUnit.Feet);
            var b = new QuantityLength(2.0, LengthUnit.Feet);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(a.Value, Is.EqualTo(1.0));
            Assert.That(b.Value, Is.EqualTo(2.0));
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        // --------------------- Yard + inches (example from spec) ---------------------

        [Test]
        public void testAddition_ValueExceedsMaxLimit_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuantityLength(QuantityLength.MaxLengthValue + 1, LengthUnit.Feet));
        }

        [Test]
        public void testAddition_NegativeExceedsMaxLimit_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuantityLength(-QuantityLength.MaxLengthValue - 1, LengthUnit.Feet));
        }

        [Test]
        public void testAddition_CrossUnit_InchesPlusYard()
        {
            var a = new QuantityLength(36.0, LengthUnit.Inch);
            var b = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength result = QuantityLength.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(72.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(LengthUnit.Inch));
        }
    }
}
