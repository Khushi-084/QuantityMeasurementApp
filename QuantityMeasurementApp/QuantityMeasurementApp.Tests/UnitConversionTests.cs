using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Service;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC5 - Unit-to-Unit Conversion (Same Measurement Type)
    /// Tests static Convert(double value, LengthUnit source, LengthUnit target) API.
    /// </summary>
    [TestClass]
    public class UC5_UnitConversionTests
    {
        [TestMethod]
        public void testConversion_FeetToInches()
        {
            // convert(1.0, FEET, INCHES) should return 12.0
            double result = QuantityLength.Convert(1.0, LengthEnum.FEET, LengthEnum.INCH);
            Assert.AreEqual(12.0, result);
        }

        [TestMethod]
        public void testConversion_InchesToFeet()
        {
            // convert(24.0, INCHES, FEET) should return 2.0
            double result = QuantityLength.Convert(24.0, LengthEnum.INCH, LengthEnum.FEET);
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void testConversion_YardsToInches()
        {
            // convert(1.0, YARDS, INCHES) should return 36.0
            double result = QuantityLength.Convert(1.0, LengthEnum.YARD, LengthEnum.INCH);
            Assert.AreEqual(36.0, result);
        }

        [TestMethod]
        public void testConversion_InchesToYards()
        {
            // convert(72.0, INCHES, YARDS) should return 2.0
            double result = QuantityLength.Convert(72.0, LengthEnum.INCH, LengthEnum.YARD);
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void testConversion_CentimetersToInches()
        {
            // convert(2.54, CENTIMETERS, INCHES) should return ~1.0 (within epsilon)
            const double epsilon = 1e-6;
            double result = QuantityLength.Convert(2.54, LengthEnum.CENTIMETER, LengthEnum.INCH);
            Assert.AreEqual(1.0, result, epsilon);
        }

        [TestMethod]
        public void testConversion_FeatToYard()
        {
            // convert(6.0, FEET, YARDS) should return 2.0
            double result = QuantityLength.Convert(6.0, LengthEnum.FEET, LengthEnum.YARD);
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            // convert(convert(v, A, B), B, A) ≈ v within defined tolerance
            double v = 6.0;  // Uses whole numbers so round-trip preserves value (6 ft = 2 yd, 2 yd = 6 ft)
            LengthEnum A = LengthEnum.FEET;
            LengthEnum B = LengthEnum.YARD;
            const double tolerance = 1e-6;

            double step1 = QuantityLength.Convert(v, A, B);
            double step2 = QuantityLength.Convert(step1, B, A);
            Assert.AreEqual(v, step2, tolerance);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            // convert(0.0, FEET, INCHES) should return 0.0
            double result = QuantityLength.Convert(0.0, LengthEnum.FEET, LengthEnum.INCH);
            Assert.AreEqual(0.0, result);
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            // convert(-1.0, FEET, INCHES) should return -12.0
            double result = QuantityLength.Convert(-1.0, LengthEnum.FEET, LengthEnum.INCH);
            Assert.AreEqual(-12.0, result);
        }

        [TestMethod]
        public void testConversion_InvalidUnit_Throws()
        {
            // Passing null or unsupported unit should throw ArgumentException
            try
            {
                QuantityLength.Convert(1.0, (LengthEnum)(-1), LengthEnum.INCH);
                Assert.Fail("Expected ArgumentException for invalid source unit");
            }
            catch (ArgumentException) { }

            try
            {
                QuantityLength.Convert(1.0, LengthEnum.FEET, (LengthEnum)(-1));
                Assert.Fail("Expected ArgumentException for invalid target unit");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void testConversion_NaNOrInfinite_Throws()
        {
            // Passing NaN or +/-Infinity should result in validation failure
            try
            {
                QuantityLength.Convert(double.NaN, LengthEnum.FEET, LengthEnum.INCH);
                Assert.Fail("Expected ArgumentException for NaN");
            }
            catch (ArgumentException) { }

            try
            {
                QuantityLength.Convert(double.PositiveInfinity, LengthEnum.FEET, LengthEnum.INCH);
                Assert.Fail("Expected ArgumentException for PositiveInfinity");
            }
            catch (ArgumentException) { }

            try
            {
                QuantityLength.Convert(double.NegativeInfinity, LengthEnum.FEET, LengthEnum.INCH);
                Assert.Fail("Expected ArgumentException for NegativeInfinity");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void testConversion_PrecisionTolerance()
        {
            // Conversion results compared using small epsilon (e.g., 1e-6) for floating-point rounding
            const double epsilon = 1e-6;
            double result = QuantityLength.Convert(2.54, LengthEnum.CENTIMETER, LengthEnum.INCH);
            Assert.AreEqual(1.0, result, epsilon);
        }
    }
}