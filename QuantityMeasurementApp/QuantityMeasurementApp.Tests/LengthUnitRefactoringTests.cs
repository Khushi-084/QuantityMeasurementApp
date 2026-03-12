using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementRepository;
using System.Reflection;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC8: Refactoring Unit Enum to Standalone with Conversion Responsibility.
    /// Only the test cases described in the UC8 spec are included here.
    /// </summary>
    [TestClass]
    public class UC8_RefactoringTests
    {
        private const double EPSILON = 0.01;

        // ---------- Standalone LengthUnit enum ----------

        [TestMethod]
        public void testLengthUnitEnum_FeetConstant()
        {
            Assert.AreEqual(1.0, LengthUnit.FEET.GetConversionFactor(), 1e-10);
        }

        [TestMethod]
        public void testLengthUnitEnum_InchesConstant()
        {
            Assert.AreEqual(1.0 / 12.0, LengthUnit.INCH.GetConversionFactor(), 1e-10);
        }

        [TestMethod]
        public void testLengthUnitEnum_YardsConstant()
        {
            Assert.AreEqual(3.0, LengthUnit.YARD.GetConversionFactor(), 1e-10);
        }

        [TestMethod]
        public void testLengthUnitEnum_CentimetersConstant()
        {
            Assert.AreEqual(0.0328084, LengthUnit.CENTIMETER.GetConversionFactor(), 1e-6);
        }

        // ---------- Base unit conversion (to feet) ----------

        [TestMethod]
        public void testConvertToBaseUnit_FeetToFeet()
        {
            Assert.AreEqual(5.0, LengthUnit.FEET.ConvertToBaseUnit(5.0), 1e-10);
        }

        [TestMethod]
        public void testConvertToBaseUnit_InchesToFeet()
        {
            Assert.AreEqual(1.0, LengthUnit.INCH.ConvertToBaseUnit(12.0), 1e-10);
        }

        [TestMethod]
        public void testConvertToBaseUnit_YardsToFeet()
        {
            Assert.AreEqual(3.0, LengthUnit.YARD.ConvertToBaseUnit(1.0), 1e-10);
        }

        [TestMethod]
        public void testConvertToBaseUnit_CentimetersToFeet()
        {
            Assert.AreEqual(1.0, LengthUnit.CENTIMETER.ConvertToBaseUnit(30.48), EPSILON);
        }

        // ---------- From base unit (feet → unit) ----------

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToFeet()
        {
            Assert.AreEqual(2.0, LengthUnit.FEET.ConvertFromBaseUnit(2.0), 1e-10);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToInches()
        {
            Assert.AreEqual(12.0, LengthUnit.INCH.ConvertFromBaseUnit(1.0), 1e-10);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToYards()
        {
            Assert.AreEqual(1.0, LengthUnit.YARD.ConvertFromBaseUnit(3.0), 1e-10);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToCentimeters()
        {
            Assert.AreEqual(30.48, LengthUnit.CENTIMETER.ConvertFromBaseUnit(1.0), EPSILON);
        }

        // ---------- QuantityLength refactored behavior ----------

        [TestMethod]
        public void testQuantityLengthRefactored_Equality()
        {
            var a = new QuantityLength(1.0, LengthUnit.FEET);
            var b = new QuantityLength(12.0, LengthUnit.INCH);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testQuantityLengthRefactored_ConvertTo()
        {
            var q = new QuantityLength(1.0, LengthUnit.FEET);
            var result = q.ConvertTo(LengthUnit.INCH);
            Assert.AreEqual(12.0, result.Value, 1e-10);
            Assert.AreEqual(LengthUnit.INCH, result.Unit);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_Add()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);
            var result = q1.Add(q2, LengthUnit.FEET);
            Assert.AreEqual(2.0, result.Value, 0.0001);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_AddWithTargetUnit()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);
            var result = q1.Add(q2, LengthUnit.YARD);
            Assert.AreEqual(0.67, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARD, result.Unit);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_NullUnit()
        {
            // Simulate a null/invalid unit using an out-of-range enum value.
            Assert.Throws<ArgumentException>(() => _ = new QuantityLength(1.0, (LengthUnit)(-1)));
        }

        [TestMethod]
        public void testQuantityLengthRefactored_InvalidValue()
        {
            Assert.Throws<ArgumentException>(() => _ = new QuantityLength(double.NaN, LengthUnit.FEET));
        }

        // ---------- Backward compatibility ----------

        [TestMethod]
        public void testBackwardCompatibility_UC5ConversionTests()
        {
            double feetToInch = QuantityLength.Convert(1.0, LengthUnit.FEET, LengthUnit.INCH);
            double inchToFeet = QuantityLength.Convert(24.0, LengthUnit.INCH, LengthUnit.FEET);

            Assert.AreEqual(12.0, feetToInch, 1e-10);
            Assert.AreEqual(2.0, inchToFeet, 1e-10);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC6AdditionTests()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(1.0, LengthUnit.FEET);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0, result.ConvertToFeet(), 0.0001);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC7AdditionWithTargetUnitTests()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.YARD);

            Assert.AreEqual(0.67, result.ConvertToFeet() / 3.0, EPSILON);
        }

        // ---------- Architectural / design checks ----------

        [TestMethod]
        public void testArchitecturalScalability_MultipleCategories()
        {
            // QuantityLength depends only on LengthUnit (no other measurement enums yet).
            FieldInfo? unitField = typeof(QuantityLength)
                .GetField("unit", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(unitField);
            Assert.AreEqual(typeof(LengthUnit), unitField!.FieldType);
        }

        [TestMethod]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double value = 100.0;
            double inFeet = LengthUnit.INCH.ConvertToBaseUnit(value);
            double back = LengthUnit.INCH.ConvertFromBaseUnit(inFeet);

            Assert.AreEqual(value, back, 1e-10);
        }

        [TestMethod]
        public void testUnitImmutability()
        {
            var feet1 = LengthUnit.FEET;
            var feet2 = LengthUnit.FEET;

            Assert.AreEqual(feet1, feet2);
            Assert.AreEqual(feet1.GetHashCode(), feet2.GetHashCode());
        }
    }
}
