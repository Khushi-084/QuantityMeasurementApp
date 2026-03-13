using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Service;


namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC9: Weight Measurement Equality, Conversion, and Addition
    /// 
    /// Units Covered:
    /// - Kilogram (KILOGRAM)
    /// - Gram (GRAM)
    /// - Pound (POUND)
    /// 
    /// This test class verifies:
    /// 1. Equality logic across units
    /// 2. Conversion accuracy
    /// 3. Addition behavior (same unit + cross unit)
    /// 4. Mathematical properties (reflexive, transitive, commutative)
    /// 5. Edge cases (zero, negative, large values)
    /// </summary>
    [TestClass]
    public class WeightMeasurementTests
    {
        // Floating point comparison tolerance
        // Used because double comparisons can have small rounding differences
        private const double EPSILON = 1e-3;

        // ============================================================
        //                     EQUALITY TESTS
        // ============================================================

        /// <summary>
        /// Verifies equality when both quantities have
        /// the same value and same unit.
        /// Expected: TRUE
        /// </summary>
        [TestMethod]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            var a = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies equality returns FALSE when values differ
        /// even if units are the same.
        /// </summary>
        [TestMethod]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            var a = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(2.0, WeightUnit.KILOGRAM);

            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Verifies 1 Kilogram equals 1000 Gram.
        /// Tests internal conversion logic inside Equals().
        /// </summary>
        [TestMethod]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            var a = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(1000.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies symmetry property:
        /// If A equals B then B equals A.
        /// </summary>
        [TestMethod]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            var a = new QuantityWeight(1000.0, WeightUnit.GRAM);
            var b = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies incompatible types return FALSE.
        /// Weight should not equal Length.
        /// </summary>
        [TestMethod]
        public void testEquality_WeightVsLength_Incompatible()
        {
            var weight = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var length = new QuantityLength(1.0, LengthEnum.FEET);

            Assert.IsFalse(weight.Equals(length));
        }

        /// <summary>
        /// Verifies comparing with null returns FALSE.
        /// </summary>
        [TestMethod]
        public void testEquality_NullComparison()
        {
            var weight = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            Assert.IsFalse(weight.Equals(null));
        }

        /// <summary>
        /// Verifies reflexive property:
        /// An object must equal itself.
        /// </summary>
        [TestMethod]
        public void testEquality_SameReference()
        {
            var weight = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(weight.Equals(weight));
        }

        /// <summary>
        /// Verifies transitive property:
        /// If A == B and B == C then A == C.
        /// </summary>
        [TestMethod]
        public void testEquality_TransitiveProperty()
        {
            var a = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(1000.0, WeightUnit.GRAM);
            var c = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        /// <summary>
        /// Verifies zero values across different units are equal.
        /// 0 kg == 0 g
        /// </summary>
        [TestMethod]
        public void testEquality_ZeroValue()
        {
            var a = new QuantityWeight(0.0, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(0.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies negative weight comparison works correctly.
        /// -1 kg == -1000 g
        /// </summary>
        [TestMethod]
        public void testEquality_NegativeWeight()
        {
            var a = new QuantityWeight(-1.0, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(-1000.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies large values maintain precision.
        /// </summary>
        [TestMethod]
        public void testEquality_LargeWeightValue()
        {
            var a = new QuantityWeight(1000000.0, WeightUnit.GRAM);
            var b = new QuantityWeight(1000.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies small decimal precision is maintained.
        /// </summary>
        [TestMethod]
        public void testEquality_SmallWeightValue()
        {
            var a = new QuantityWeight(0.001, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(1.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b));
        }

        // ============================================================
        //                     CONVERSION TESTS
        // ============================================================

        /// <summary>
        /// Verifies Pound to Kilogram conversion.
        /// 2.20462 lb ≈ 1 kg
        /// </summary>
        [TestMethod]
        public void testConversion_PoundToKilogram()
        {
            var q = new QuantityWeight(2.20462, WeightUnit.POUND);
            var result = q.ConvertTo(WeightUnit.KILOGRAM);

            Assert.AreEqual(1.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies Kilogram to Pound conversion.
        /// 1 kg ≈ 2.20462 lb
        /// </summary>
        [TestMethod]
        public void testConversion_KilogramToPound()
        {
            var q = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var result = q.ConvertTo(WeightUnit.POUND);

            Assert.AreEqual(2.20462, result.Value, 1e-2);
        }

        /// <summary>
        /// Verifies converting to same unit
        /// does not change the value.
        /// </summary>
        [TestMethod]
        public void testConversion_SameUnit()
        {
            var q = new QuantityWeight(5.0, WeightUnit.KILOGRAM);
            var result = q.ConvertTo(WeightUnit.KILOGRAM);

            Assert.AreEqual(5.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, result.Unit);
        }

        /// <summary>
        /// Verifies zero conversion across units.
        /// </summary>
        [TestMethod]
        public void testConversion_ZeroValue()
        {
            var q = new QuantityWeight(0.0, WeightUnit.KILOGRAM);
            var result = q.ConvertTo(WeightUnit.GRAM);

            Assert.AreEqual(0.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies negative value conversion preserves sign.
        /// </summary>
        [TestMethod]
        public void testConversion_NegativeValue()
        {
            var q = new QuantityWeight(-1.0, WeightUnit.KILOGRAM);
            var result = q.ConvertTo(WeightUnit.GRAM);

            Assert.AreEqual(-1000.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies round-trip conversion accuracy.
        /// kg → g → kg should return original value.
        /// </summary>
        [TestMethod]
        public void testConversion_RoundTrip()
        {
            var q = new QuantityWeight(1.5, WeightUnit.KILOGRAM);
            var grams = q.ConvertTo(WeightUnit.GRAM);
            var back = grams.ConvertTo(WeightUnit.KILOGRAM);

            Assert.AreEqual(1.5, back.Value, EPSILON);
        }

        // ============================================================
        //                     ADDITION TESTS
        // ============================================================

        /// <summary>
        /// Same-unit addition.
        /// </summary>
        [TestMethod]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            var q1 = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var q2 = new QuantityWeight(2.0, WeightUnit.KILOGRAM);

            var result = q1.Add(q2);

            Assert.AreEqual(3.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, result.Unit);
        }

        /// <summary>
        /// Cross-unit addition.
        /// Result should be in first operand's unit.
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            var q1 = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var q2 = new QuantityWeight(1000.0, WeightUnit.GRAM);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, result.Unit);
        }

        /// <summary>
        /// Mixed imperial + metric addition.
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            var q1 = new QuantityWeight(2.20462, WeightUnit.POUND);
            var q2 = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            var result = q1.Add(q2);

            Assert.AreEqual(4.40924, result.Value, 1e-2);
            Assert.AreEqual(WeightUnit.POUND, result.Unit);
        }

        /// <summary>
        /// Explicit target unit addition.
        /// </summary>
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Kilogram()
        {
            var q1 = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var q2 = new QuantityWeight(1000.0, WeightUnit.GRAM);

            var result = q1.Add(q2, WeightUnit.GRAM);

            Assert.AreEqual(2000.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnit.GRAM, result.Unit);
        }

        /// <summary>
        /// Commutative property:
        /// A + B == B + A (after conversion).
        /// </summary>
        [TestMethod]
        public void testAddition_Commutativity()
        {
            var a = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var b = new QuantityWeight(1000.0, WeightUnit.GRAM);

            var result1 = a.Add(b);
            var result2 = b.Add(a);

            Assert.IsTrue(result1.Equals(result2.ConvertTo(result1.Unit)));
        }

        /// <summary>
        /// Adding zero should return original value.
        /// </summary>
        [TestMethod]
        public void testAddition_WithZero()
        {
            var q1 = new QuantityWeight(5.0, WeightUnit.KILOGRAM);
            var q2 = new QuantityWeight(0.0, WeightUnit.GRAM);

            var result = q1.Add(q2);

            Assert.AreEqual(5.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Addition with negative values.
        /// </summary>
        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var q1 = new QuantityWeight(5.0, WeightUnit.KILOGRAM);
            var q2 = new QuantityWeight(-2000.0, WeightUnit.GRAM);

            var result = q1.Add(q2);

            Assert.AreEqual(3.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Addition with very large numbers.
        /// </summary>
        [TestMethod]
        public void testAddition_LargeValues()
        {
            var q1 = new QuantityWeight(1e6, WeightUnit.KILOGRAM);
            var q2 = new QuantityWeight(1e6, WeightUnit.KILOGRAM);

            var result = q1.Add(q2);

            Assert.AreEqual(2e6, result.Value, EPSILON);
        }
    }
}