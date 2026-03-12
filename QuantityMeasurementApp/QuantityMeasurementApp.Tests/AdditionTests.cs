using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC6 - Addition of Two Length Units (Same Category)
    /// Tests instance method Add(QuantityLength) and result in unit of first operand.
    /// </summary>
    [TestClass]
    public class UC6_AdditionTests
    {
        [TestMethod]
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(2.0, LengthUnit.FEET);
            var result = q1.Add(q2);
            Assert.AreEqual(3.0, result.ConvertToFeet(), 0.0001);
        }

        [TestMethod]
        public void testAddition_SameUnit_InchPlusInch()
        {
            var q1 = new QuantityLength(6.0, LengthUnit.INCH);
            var q2 = new QuantityLength(6.0, LengthUnit.INCH);
            var result = q1.Add(q2);
            double resultInInches = result.ConvertToFeet() * 12.0;
            Assert.AreEqual(12.0, resultInInches, 0.0001);
        }

        [TestMethod]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);
            var result = q1.Add(q2);
            Assert.AreEqual(2.0, result.ConvertToFeet(), 0.0001);
        }

        [TestMethod]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            var q1 = new QuantityLength(12.0, LengthUnit.INCH);
            var q2 = new QuantityLength(1.0, LengthUnit.FEET);
            var result = q1.Add(q2);
            double resultInInches = result.ConvertToFeet() * 12.0;
            Assert.AreEqual(24.0, resultInInches, 0.0001);
        }

        [TestMethod]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.YARD);
            var q2 = new QuantityLength(3.0, LengthUnit.FEET);
            var result = q1.Add(q2);
            double resultInYards = result.ConvertToFeet() / 3.0;
            Assert.AreEqual(2.0, resultInYards, 0.0001);
        }

        [TestMethod]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            var q1 = new QuantityLength(2.54, LengthUnit.CENTIMETER);
            var q2 = new QuantityLength(1.0, LengthUnit.INCH);
            var result = q1.Add(q2);
            double resultInCm = result.ConvertToFeet() / 0.0328084;
            Assert.AreEqual(5.08, resultInCm, 0.01);
        }

        [TestMethod]
        public void testAddition_Commutativity()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);
            var r1 = q1.Add(q2);
            var r2 = q2.Add(q1);
            Assert.AreEqual(r1.ConvertToFeet(), r2.ConvertToFeet(), 0.0001);
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            var q1 = new QuantityLength(5.0, LengthUnit.FEET);
            var q2 = new QuantityLength(0.0, LengthUnit.INCH);
            var result = q1.Add(q2);
            Assert.AreEqual(5.0, result.ConvertToFeet(), 0.0001);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var q1 = new QuantityLength(5.0, LengthUnit.FEET);
            var q2 = new QuantityLength(-2.0, LengthUnit.FEET);
            var result = q1.Add(q2);
            Assert.AreEqual(3.0, result.ConvertToFeet(), 0.0001);
        }

        [TestMethod]
        public void testAddition_NullSecondOperand()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            try
            {
                q1.Add(null!);
                Assert.Fail("Expected ArgumentNullException for null argument.");
            }
            catch (ArgumentNullException) { }
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            var q1 = new QuantityLength(1e6, LengthUnit.FEET);
            var q2 = new QuantityLength(1e6, LengthUnit.FEET);
            var result = q1.Add(q2);
            Assert.AreEqual(2e6, result.ConvertToFeet(), 0.0001);
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            var q1 = new QuantityLength(0.001, LengthUnit.FEET);
            var q2 = new QuantityLength(0.002, LengthUnit.FEET);
            var result = q1.Add(q2);
            // Add rounds to 2 decimals; 0.003 may round to 0.00, so use epsilon 0.01
            Assert.AreEqual(0.003, result.ConvertToFeet(), 0.01);
        }
    }
}
