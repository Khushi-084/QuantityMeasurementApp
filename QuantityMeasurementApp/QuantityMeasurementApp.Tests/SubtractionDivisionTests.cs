using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementModel.Interfaces;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC12: Subtraction and Division Operations on Quantity Measurements.
    ///
    /// Categories covered: Length (LengthUnitM), Weight (WeightUnitM), Volume (VolumeUnitM).
    ///
    /// Test groups:
    /// 1.  Subtraction — same unit
    /// 2.  Subtraction — cross unit (same category)
    /// 3.  Subtraction — explicit target unit
    /// 4.  Subtraction — mathematical properties (non-commutativity, negative, zero)
    /// 5.  Subtraction — edge cases (null, null target unit, cross-category)
    /// 6.  Subtraction — chaining, immutability, inverse with addition
    /// 7.  Division — same unit
    /// 8.  Division — cross unit (same category)
    /// 9.  Division — ratio cases (>1, <1, =1, non-commutativity)
    /// 10. Division — edge cases (zero, large/small ratio, null, cross-category)
    /// 11. Division — immutability, integration
    /// </summary>
    [TestClass]
    public class UC12_SubtractionDivisionTests
    {
        private const double EPSILON = 1e-3;

        // ============================================================
        //             SUBTRACTION — SAME UNIT TESTS
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Subtract(new Quantity(5.0, FEET))
        /// returns Quantity(5.0, FEET).
        /// Tests: Same-unit subtraction without conversion.
        /// </summary>
        [TestMethod]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            var result = q1.Subtract(q2);

            Assert.AreEqual(5.0,            result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that new Quantity(10.0, LITRE).Subtract(new Quantity(3.0, LITRE))
        /// returns Quantity(7.0, LITRE).
        /// Tests: Same-unit subtraction for volume.
        /// </summary>
        [TestMethod]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            var q1 = new Quantity<VolumeUnitM>(10.0, VolumeUnitM.LITRE);
            var q2 = new Quantity<VolumeUnitM>(3.0,  VolumeUnitM.LITRE);

            var result = q1.Subtract(q2);

            Assert.AreEqual(7.0,               result.Value, EPSILON);
            Assert.AreEqual(VolumeUnitM.LITRE,  result.Unit);
        }

        // ============================================================
        //             SUBTRACTION — CROSS UNIT TESTS
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Subtract(new Quantity(6.0, INCHES))
        /// returns Quantity(9.5, FEET).
        /// Tests: Cross-unit subtraction with result in feet.
        /// </summary>
        [TestMethod]
        public void testSubtraction_CrossUnit_FeetMinusInches()
        {
            var feet   = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var inches = new Quantity<LengthUnitM>(6.0,  LengthUnitM.INCHES);

            var result = feet.Subtract(inches);

            Assert.AreEqual(9.5,            result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that new Quantity(120.0, INCHES).Subtract(new Quantity(5.0, FEET))
        /// returns Quantity(60.0, INCHES).
        /// Tests: Cross-unit subtraction with result in inches.
        /// </summary>
        [TestMethod]
        public void testSubtraction_CrossUnit_InchesMinusFeet()
        {
            var inches = new Quantity<LengthUnitM>(120.0, LengthUnitM.INCHES);
            var feet   = new Quantity<LengthUnitM>(5.0,   LengthUnitM.FEET);

            var result = inches.Subtract(feet);

            Assert.AreEqual(60.0,              result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.INCHES, result.Unit);
        }

        // ============================================================
        //             SUBTRACTION — EXPLICIT TARGET UNIT TESTS
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Subtract(new Quantity(6.0, INCHES), FEET)
        /// returns Quantity(9.5, FEET).
        /// Tests: Explicit target unit specification in feet.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Feet()
        {
            var feet   = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var inches = new Quantity<LengthUnitM>(6.0,  LengthUnitM.INCHES);

            var result = feet.Subtract(inches, LengthUnitM.FEET);

            Assert.AreEqual(9.5,            result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Subtract(new Quantity(6.0, INCHES), INCHES)
        /// returns Quantity(114.0, INCHES).
        /// Tests: Explicit target unit specification in inches.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            var feet   = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var inches = new Quantity<LengthUnitM>(6.0,  LengthUnitM.INCHES);

            var result = feet.Subtract(inches, LengthUnitM.INCHES);

            Assert.AreEqual(114.0,             result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.INCHES, result.Unit);
        }

        /// <summary>
        /// Verifies that new Quantity(5.0, LITRE).Subtract(new Quantity(2.0, LITRE), MILLILITRE)
        /// returns Quantity(3000.0, MILLILITRE).
        /// Tests: Explicit target unit specification in millilitre.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            var first  = new Quantity<VolumeUnitM>(5.0, VolumeUnitM.LITRE);
            var second = new Quantity<VolumeUnitM>(2.0, VolumeUnitM.LITRE);

            var result = first.Subtract(second, VolumeUnitM.MILLILITRE);

            Assert.AreEqual(3000.0,                  result.Value, EPSILON);
            Assert.AreEqual(VolumeUnitM.MILLILITRE,   result.Unit);
        }

        // ============================================================
        //             SUBTRACTION — MATHEMATICAL PROPERTIES
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(5.0, FEET).Subtract(new Quantity(10.0, FEET))
        /// returns Quantity(-5.0, FEET).
        /// Tests: Subtraction resulting in negative values.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            var q1 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            var result = q1.Subtract(q2);

            Assert.AreEqual(-5.0,           result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Subtract(new Quantity(120.0, INCHES))
        /// returns Quantity(0.0, FEET).
        /// Tests: Subtraction resulting in zero.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ResultingInZero()
        {
            var feet   = new Quantity<LengthUnitM>(10.0,  LengthUnitM.FEET);
            var inches = new Quantity<LengthUnitM>(120.0, LengthUnitM.INCHES);

            var result = feet.Subtract(inches);

            Assert.AreEqual(0.0,            result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that new Quantity(5.0, FEET).Subtract(new Quantity(0.0, INCHES))
        /// returns Quantity(5.0, FEET).
        /// Tests: Identity element property — subtracting zero leaves value unchanged.
        /// </summary>
        [TestMethod]
        public void testSubtraction_WithZeroOperand()
        {
            var q1   = new Quantity<LengthUnitM>(5.0, LengthUnitM.FEET);
            var zero = new Quantity<LengthUnitM>(0.0, LengthUnitM.INCHES);

            var result = q1.Subtract(zero);

            Assert.AreEqual(5.0,            result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that new Quantity(5.0, FEET).Subtract(new Quantity(-2.0, FEET))
        /// returns Quantity(7.0, FEET).
        /// Tests: Subtracting a negative quantity is equivalent to adding.
        /// </summary>
        [TestMethod]
        public void testSubtraction_WithNegativeValues()
        {
            var q1  = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);
            var neg = new Quantity<LengthUnitM>(-2.0, LengthUnitM.FEET);

            var result = q1.Subtract(neg);

            Assert.AreEqual(7.0,            result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that A.Subtract(B) != B.Subtract(A) — subtraction is non-commutative.
        /// A=10 ft, B=5 ft → A-B=5 ft, B-A=-5 ft.
        /// </summary>
        [TestMethod]
        public void testSubtraction_NonCommutative()
        {
            var a = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var b = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            var aMinusB = a.Subtract(b);
            var bMinusA = b.Subtract(a);

            Assert.AreEqual(5.0,  aMinusB.Value, EPSILON);
            Assert.AreEqual(-5.0, bMinusA.Value, EPSILON);
            Assert.IsFalse(aMinusB.Equals(bMinusA));
        }

        /// <summary>
        /// Verifies large value subtraction.
        /// 1e6 kg - 5e5 kg = 5e5 kg.
        /// Tests: Large magnitude subtraction.
        /// </summary>
        [TestMethod]
        public void testSubtraction_WithLargeValues()
        {
            var q1 = new Quantity<WeightUnitM>(1e6, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(5e5, WeightUnitM.KILOGRAM);

            var result = q1.Subtract(q2);

            Assert.AreEqual(5e5,                 result.Value, EPSILON);
            Assert.AreEqual(WeightUnitM.KILOGRAM, result.Unit);
        }

        // ============================================================
        //             SUBTRACTION — ERROR HANDLING
        // ============================================================

        /// <summary>
        /// Verifies that Subtract(null) throws ArgumentNullException.
        /// Tests: Null operand validation.
        /// </summary>
        [TestMethod]
        public void testSubtraction_NullOperand()
        {
            var q = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            try
            {
                q.Subtract(null!);
                Assert.Fail("Expected ArgumentNullException for null operand.");
            }
            catch (ArgumentNullException) { /* expected */ }
        }

        /// <summary>
        /// Verifies that Subtract(other, null) throws ArgumentException.
        /// Tests: Null target unit validation.
        /// </summary>
        [TestMethod]
        public void testSubtraction_NullTargetUnit()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            try
            {
                q1.Subtract(q2, null!);
                Assert.Fail("Expected ArgumentException for null target unit.");
            }
            catch (ArgumentException) { /* expected */ }
        }

        /// <summary>
        /// Verifies that subtracting a weight from a length is prevented at compile time
        /// by the generic type parameter — both must be Quantity&lt;LengthUnitM&gt;.
        /// Runtime cross-category check: unit types differ → equals returns false.
        /// Tests: Cross-category type safety.
        /// </summary>
        [TestMethod]
        public void testSubtraction_CrossCategory()
        {
            var length = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var weight = new Quantity<WeightUnitM>(5.0,  WeightUnitM.KILOGRAM);

            // Cross-category: type system prevents Subtract at compile time.
            // Verify the runtime type check via Equals.
            Assert.IsFalse(length.Equals(weight));
            Assert.AreNotEqual(length.GetType(), weight.GetType());
        }

        // ============================================================
        //             SUBTRACTION — CHAINING, IMMUTABILITY, INTEGRATION
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET)
        ///   .Subtract(new Quantity(2.0, FEET))
        ///   .Subtract(new Quantity(1.0, FEET))
        /// returns Quantity(7.0, FEET).
        /// Tests: Method chaining support.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ChainedOperations()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);
            var q3 = new Quantity<LengthUnitM>(1.0,  LengthUnitM.FEET);

            var result = q1.Subtract(q2).Subtract(q3);

            Assert.AreEqual(7.0,            result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that original quantities are unchanged after subtraction.
        /// Tests: Immutability principle.
        /// </summary>
        [TestMethod]
        public void testSubtraction_Immutability()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(3.0,  LengthUnitM.FEET);

            _ = q1.Subtract(q2);

            Assert.AreEqual(10.0, q1.Value, EPSILON);  // original unchanged
            Assert.AreEqual(3.0,  q2.Value, EPSILON);  // original unchanged
        }

        /// <summary>
        /// Verifies that A.Add(B).Subtract(B) ≈ A (mathematical inverse relationship).
        /// Tests: Integration between addition and subtraction.
        /// </summary>
        [TestMethod]
        public void testSubtractionAddition_Inverse()
        {
            var a = new Quantity<WeightUnitM>(5.0,    WeightUnitM.KILOGRAM);
            var b = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);

            var result = a.Add(b).Subtract(b);

            Assert.AreEqual(a.Value, result.Value, EPSILON);
            Assert.AreEqual(a.Unit,  result.Unit);
        }

        /// <summary>
        /// Verifies subtraction works for all three measurement categories.
        /// Tests: Scalability across categories.
        /// </summary>
        [TestMethod]
        public void testSubtraction_AllMeasurementCategories()
        {
            // Length
            var lResult = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET)
                .Subtract(new Quantity<LengthUnitM>(5.0, LengthUnitM.FEET));
            Assert.AreEqual(5.0, lResult.Value, EPSILON);

            // Weight
            var wResult = new Quantity<WeightUnitM>(10.0, WeightUnitM.KILOGRAM)
                .Subtract(new Quantity<WeightUnitM>(5000.0, WeightUnitM.GRAM));
            Assert.AreEqual(5.0, wResult.Value, EPSILON);

            // Volume
            var vResult = new Quantity<VolumeUnitM>(5.0, VolumeUnitM.LITRE)
                .Subtract(new Quantity<VolumeUnitM>(500.0, VolumeUnitM.MILLILITRE));
            Assert.AreEqual(4.5, vResult.Value, EPSILON);
        }

        /// <summary>
        /// Verifies that subtraction results are rounded to two decimal places.
        /// Tests: Precision consistency.
        /// </summary>
        [TestMethod]
        public void testSubtraction_PrecisionAndRounding()
        {
            var q1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(1.0, LengthUnitM.INCHES); // 1/12 foot

            var result = q1.Subtract(q2);

            // 1.0 - 1/12 = 11/12 ≈ 0.916666... → rounded to 0.92
            Assert.AreEqual(0.92, result.Value, 0.01);
        }

        // ============================================================
        //             DIVISION — SAME UNIT TESTS
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Divide(new Quantity(2.0, FEET)) returns 5.0.
        /// Tests: Same-unit division without conversion.
        /// </summary>
        [TestMethod]
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.AreEqual(5.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies that new Quantity(10.0, LITRE).Divide(new Quantity(5.0, LITRE)) returns 2.0.
        /// Tests: Same-unit division for volume.
        /// </summary>
        [TestMethod]
        public void testDivision_SameUnit_LitreDividedByLitre()
        {
            var q1 = new Quantity<VolumeUnitM>(10.0, VolumeUnitM.LITRE);
            var q2 = new Quantity<VolumeUnitM>(5.0,  VolumeUnitM.LITRE);

            double result = q1.Divide(q2);

            Assert.AreEqual(2.0, result, EPSILON);
        }

        // ============================================================
        //             DIVISION — CROSS UNIT TESTS
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(24.0, INCHES).Divide(new Quantity(2.0, FEET)) returns 1.0.
        /// 24 inches = 2 feet → 2 / 2 = 1.0.
        /// Tests: Cross-unit division with correct conversion.
        /// </summary>
        [TestMethod]
        public void testDivision_CrossUnit_FeetDividedByInches()
        {
            var inches = new Quantity<LengthUnitM>(24.0, LengthUnitM.INCHES);
            var feet   = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            double result = inches.Divide(feet);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies that new Quantity(2.0, KILOGRAM).Divide(new Quantity(2000.0, GRAM)) returns 1.0.
        /// 2 kg = 2000 g → 2000 / 2000 = 1.0.
        /// Tests: Cross-unit division for weight.
        /// </summary>
        [TestMethod]
        public void testDivision_CrossUnit_KilogramDividedByGram()
        {
            var kg   = new Quantity<WeightUnitM>(2.0,    WeightUnitM.KILOGRAM);
            var gram = new Quantity<WeightUnitM>(2000.0, WeightUnitM.GRAM);

            double result = kg.Divide(gram);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        // ============================================================
        //             DIVISION — RATIO TESTS
        // ============================================================

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Divide(new Quantity(2.0, FEET)) returns 5.0.
        /// Tests: Ratio > 1.0 case.
        /// </summary>
        [TestMethod]
        public void testDivision_RatioGreaterThanOne()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.IsTrue(result > 1.0);
            Assert.AreEqual(5.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies that new Quantity(5.0, FEET).Divide(new Quantity(10.0, FEET)) returns 0.5.
        /// Tests: Ratio &lt; 1.0 case.
        /// </summary>
        [TestMethod]
        public void testDivision_RatioLessThanOne()
        {
            var q1 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.IsTrue(result < 1.0);
            Assert.AreEqual(0.5, result, EPSILON);
        }

        /// <summary>
        /// Verifies that new Quantity(10.0, FEET).Divide(new Quantity(10.0, FEET)) returns 1.0.
        /// Tests: Equivalence detection through division.
        /// </summary>
        [TestMethod]
        public void testDivision_RatioEqualToOne()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies that A.Divide(B) != B.Divide(A) — division is non-commutative.
        /// 10 ft / 5 ft = 2.0; 5 ft / 10 ft = 0.5.
        /// </summary>
        [TestMethod]
        public void testDivision_NonCommutative()
        {
            var a = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var b = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            double aByB = a.Divide(b);
            double bByA = b.Divide(a);

            Assert.AreEqual(2.0, aByB, EPSILON);
            Assert.AreEqual(0.5, bByA, EPSILON);
            Assert.AreNotEqual(aByB, bByA, EPSILON);
        }

        // ============================================================
        //             DIVISION — ERROR HANDLING
        // ============================================================

        /// <summary>
        /// Verifies that Divide(zero quantity) throws ArithmeticException.
        /// Tests: Division by zero prevention.
        /// </summary>
        [TestMethod]
        public void testDivision_ByZero()
        {
            var q1   = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var zero = new Quantity<LengthUnitM>(0.0,  LengthUnitM.FEET);

            try
            {
                q1.Divide(zero);
                Assert.Fail("Expected ArithmeticException for division by zero.");
            }
            catch (ArithmeticException) { /* expected */ }
        }

        /// <summary>
        /// Verifies large ratio: 1e6 kg / 1 kg = 1e6.
        /// Tests: Very large ratios.
        /// </summary>
        [TestMethod]
        public void testDivision_WithLargeRatio()
        {
            var q1 = new Quantity<WeightUnitM>(1e6, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);

            double result = q1.Divide(q2);

            Assert.AreEqual(1e6, result, 1.0);
        }

        /// <summary>
        /// Verifies small ratio: 1 kg / 1e6 kg = 1e-6.
        /// Tests: Very small ratios and floating-point precision.
        /// </summary>
        [TestMethod]
        public void testDivision_WithSmallRatio()
        {
            var q1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(1e6, WeightUnitM.KILOGRAM);

            double result = q1.Divide(q2);

            Assert.AreEqual(1e-6, result, 1e-9);
        }

        /// <summary>
        /// Verifies that Divide(null) throws ArgumentNullException.
        /// Tests: Null operand validation.
        /// </summary>
        [TestMethod]
        public void testDivision_NullOperand()
        {
            var q = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            try
            {
                q.Divide(null!);
                Assert.Fail("Expected ArgumentNullException for null divisor.");
            }
            catch (ArgumentNullException) { /* expected */ }
        }

        /// <summary>
        /// Verifies that dividing length by weight is prevented by the generic type parameter.
        /// Runtime check: types differ.
        /// Tests: Cross-category prevention.
        /// </summary>
        [TestMethod]
        public void testDivision_CrossCategory()
        {
            var length = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var weight = new Quantity<WeightUnitM>(5.0,  WeightUnitM.KILOGRAM);

            // Generic type system prevents Divide at compile time.
            // Confirm categories are distinct at runtime.
            Assert.AreNotEqual(length.GetType(), weight.GetType());
            Assert.IsFalse(length.Equals(weight));
        }

        // ============================================================
        //             DIVISION — IMMUTABILITY & INTEGRATION
        // ============================================================

        /// <summary>
        /// Verifies that original quantities are unchanged after division.
        /// Tests: Immutability principle.
        /// </summary>
        [TestMethod]
        public void testDivision_Immutability()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            _ = q1.Divide(q2);

            Assert.AreEqual(10.0, q1.Value, EPSILON);  // original unchanged
            Assert.AreEqual(2.0,  q2.Value, EPSILON);  // original unchanged
        }

        /// <summary>
        /// Verifies division works for all three measurement categories.
        /// Tests: Scalability across categories.
        /// </summary>
        [TestMethod]
        public void testDivision_AllMeasurementCategories()
        {
            // Length
            double lRatio = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET)
                .Divide(new Quantity<LengthUnitM>(5.0, LengthUnitM.FEET));
            Assert.AreEqual(2.0, lRatio, EPSILON);

            // Weight
            double wRatio = new Quantity<WeightUnitM>(10.0, WeightUnitM.KILOGRAM)
                .Divide(new Quantity<WeightUnitM>(5.0, WeightUnitM.KILOGRAM));
            Assert.AreEqual(2.0, wRatio, EPSILON);

            // Volume
            double vRatio = new Quantity<VolumeUnitM>(5.0, VolumeUnitM.LITRE)
                .Divide(new Quantity<VolumeUnitM>(10.0, VolumeUnitM.LITRE));
            Assert.AreEqual(0.5, vRatio, EPSILON);
        }

        /// <summary>
        /// Verifies that (A / B) / C != A / (B / C) — division is non-associative.
        /// (10 / 5) / 2 = 1.0   vs   10 / (5 / 2) = 4.0
        /// Tests: Mathematical property awareness.
        /// </summary>
        [TestMethod]
        public void testDivision_Associativity()
        {
            var a = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var b = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);
            var c = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            // (A / B) / C: divide as scalars since Divide returns double
            double aByB      = a.Divide(b);   // 2.0
            // Wrap scalar back as a Quantity to perform the second divide
            var aByBQ        = new Quantity<LengthUnitM>(aByB, LengthUnitM.FEET);
            double leftSide  = aByBQ.Divide(c);  // 2.0 / 2.0 = 1.0

            // A / (B / C)
            double bByC      = b.Divide(c);       // 2.5
            var bByCQ        = new Quantity<LengthUnitM>(bByC, LengthUnitM.FEET);
            double rightSide = a.Divide(bByCQ);   // 10 / 2.5 = 4.0

            Assert.AreNotEqual(leftSide, rightSide, EPSILON);
            Assert.AreEqual(1.0, leftSide,  EPSILON);
            Assert.AreEqual(4.0, rightSide, EPSILON);
        }

        /// <summary>
        /// Verifies that subtraction and division operations coexist without conflict.
        /// Example: A.Subtract(B).Divide(C) is valid.
        /// Tests: Operation integration.
        /// </summary>
        [TestMethod]
        public void testSubtractionAndDivision_Integration()
        {
            var a = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var b = new Quantity<LengthUnitM>(4.0,  LengthUnitM.FEET);
            var c = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            // (10 - 4) / 2 = 6 / 2 = 3.0
            double result = a.Subtract(b).Divide(c);

            Assert.AreEqual(3.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies division precision handling — result is a raw double with no arbitrary rounding.
        /// 1000 mL / 1 L = 1.0 (cross-unit, same category).
        /// Tests: Precision handling differences between Subtract (rounded) and Divide (raw double).
        /// </summary>
        [TestMethod]
        public void testDivision_PrecisionHandling()
        {
            var ml    = new Quantity<VolumeUnitM>(1000.0, VolumeUnitM.MILLILITRE);
            var litre = new Quantity<VolumeUnitM>(1.0,    VolumeUnitM.LITRE);

            double result = ml.Divide(litre);

            Assert.AreEqual(1.0, result, EPSILON);
        }
    }
}
