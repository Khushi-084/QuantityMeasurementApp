using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementBusinessLayer.Service;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC11: Volume Measurement Equality, Conversion, and Addition
    ///
    /// Units Covered:
    /// - Litre      (LITRE)
    /// - Millilitre (MILLILITRE)
    /// - Gallon     (GALLON)
    ///
    /// This test class verifies:
    /// 1. Equality logic across units
    /// 2. Conversion accuracy
    /// 3. Addition behaviour (same unit + cross unit)
    /// 4. Mathematical properties (reflexive, transitive, commutative)
    /// 5. Edge cases (zero, negative, large, small values)
    /// 6. VolumeUnit enum constants and extension methods
    /// 7. Backward compatibility with UC1–UC10
    /// </summary>
    [TestClass]
    public class VolumeMeasurementTest
    {
        private const double EPSILON          = 1e-3;
        private const double LitresPerGallon  = 3.78541;

        // ============================================================
        //                      EQUALITY TESTS
        // ============================================================

        /// <summary>
        /// Verifies equality when both quantities have
        /// the same value and same unit.
        /// Expected: TRUE
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToLitre_SameValue()
        {
            var first  = new QuantityVolume(1.0, VolumeUnit.LITRE);
            var second = new QuantityVolume(1.0, VolumeUnit.LITRE);

            Assert.IsTrue(first.Equals(second));
        }

        /// <summary>
        /// Verifies equality returns FALSE when values differ
        /// even if units are the same.
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToLitre_DifferentValue()
        {
            var first  = new QuantityVolume(1.0, VolumeUnit.LITRE);
            var second = new QuantityVolume(2.0, VolumeUnit.LITRE);

            Assert.IsFalse(first.Equals(second));
        }

        /// <summary>
        /// Verifies 1 Litre equals 1000 Millilitres.
        /// Tests internal conversion logic inside Equals().
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToMillilitre_EquivalentValue()
        {
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);

            Assert.IsTrue(litre.Equals(millilitre));
        }

        /// <summary>
        /// Verifies symmetry property:
        /// If A equals B then B equals A.
        /// 1000 mL == 1 L
        /// </summary>
        [TestMethod]
        public void testEquality_MillilitreToLitre_EquivalentValue()
        {
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);

            Assert.IsTrue(millilitre.Equals(litre));
        }

        /// <summary>
        /// Verifies 1 Litre equals ~0.264172 Gallon.
        /// 1 L = 1 / 3.78541 US gallons
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToGallon_EquivalentValue()
        {
            double gallonEquiv = 1.0 / LitresPerGallon;
            var litre  = new QuantityVolume(1.0,         VolumeUnit.LITRE);
            var gallon = new QuantityVolume(gallonEquiv, VolumeUnit.GALLON);

            Assert.IsTrue(litre.Equals(gallon));
        }

        /// <summary>
        /// Verifies 1 Gallon equals ~3.78541 Litres (symmetry).
        /// </summary>
        [TestMethod]
        public void testEquality_GallonToLitre_EquivalentValue()
        {
            var gallon = new QuantityVolume(1.0,           VolumeUnit.GALLON);
            var litre  = new QuantityVolume(LitresPerGallon, VolumeUnit.LITRE);

            Assert.IsTrue(gallon.Equals(litre));
        }

        /// <summary>
        /// Verifies incompatible types return FALSE.
        /// Volume should not equal Length.
        /// </summary>
        [TestMethod]
        public void testEquality_VolumeVsLength_Incompatible()
        {
            var volume = new QuantityVolume(1.0, VolumeUnit.LITRE);
            var length = new QuantityLength(1.0, LengthEnum.FEET);

            Assert.IsFalse(volume.Equals(length));
        }

        /// <summary>
        /// Verifies incompatible types return FALSE.
        /// Volume should not equal Weight.
        /// </summary>
        [TestMethod]
        public void testEquality_VolumeVsWeight_Incompatible()
        {
            var volume = new QuantityVolume(1.0, VolumeUnit.LITRE);
            var weight = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            Assert.IsFalse(volume.Equals(weight));
        }

        /// <summary>
        /// Verifies comparing with null returns FALSE.
        /// </summary>
        [TestMethod]
        public void testEquality_NullComparison()
        {
            var litre = new QuantityVolume(1.0, VolumeUnit.LITRE);

            Assert.IsFalse(litre.Equals(null));
        }

        /// <summary>
        /// Verifies reflexive property:
        /// An object must equal itself.
        /// </summary>
        [TestMethod]
        public void testEquality_SameReference()
        {
            var litre = new QuantityVolume(1.0, VolumeUnit.LITRE);

            Assert.IsTrue(litre.Equals(litre));
        }

        /// <summary>
        /// Verifies constructor throws ArgumentException for an invalid/unknown unit.
        /// </summary>
        [TestMethod]
        public void testEquality_NullUnit()
        {
            try
            {
                new QuantityVolume(1.0, (VolumeUnit)999);
                Assert.Fail("Expected ArgumentException for invalid unit.");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        /// <summary>
        /// Verifies transitive property:
        /// If A == B and B == C then A == C.
        /// </summary>
        [TestMethod]
        public void testEquality_TransitiveProperty()
        {
            var a = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var b = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var c = new QuantityVolume(1.0,    VolumeUnit.LITRE);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        /// <summary>
        /// Verifies zero values across different units are equal.
        /// 0 L == 0 mL
        /// </summary>
        [TestMethod]
        public void testEquality_ZeroValue()
        {
            var a = new QuantityVolume(0.0, VolumeUnit.LITRE);
            var b = new QuantityVolume(0.0, VolumeUnit.MILLILITRE);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies negative volume comparison works correctly.
        /// -1 L == -1000 mL
        /// </summary>
        [TestMethod]
        public void testEquality_NegativeVolume()
        {
            var a = new QuantityVolume(-1.0,    VolumeUnit.LITRE);
            var b = new QuantityVolume(-1000.0, VolumeUnit.MILLILITRE);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies large values maintain precision.
        /// 1,000,000 mL == 1000 L
        /// </summary>
        [TestMethod]
        public void testEquality_LargeVolumeValue()
        {
            var a = new QuantityVolume(1_000_000.0, VolumeUnit.MILLILITRE);
            var b = new QuantityVolume(1000.0,      VolumeUnit.LITRE);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies small decimal precision is maintained.
        /// 0.001 L == 1 mL
        /// </summary>
        [TestMethod]
        public void testEquality_SmallVolumeValue()
        {
            var a = new QuantityVolume(0.001, VolumeUnit.LITRE);
            var b = new QuantityVolume(1.0,   VolumeUnit.MILLILITRE);

            Assert.IsTrue(a.Equals(b));
        }

        // ============================================================
        //                     CONVERSION TESTS
        // ============================================================

        /// <summary>
        /// Verifies Litre to Millilitre conversion.
        /// 1 L = 1000 mL
        /// </summary>
        [TestMethod]
        public void testConversion_LitreToMillilitre()
        {
            var litre  = new QuantityVolume(1.0, VolumeUnit.LITRE);
            var result = litre.ConvertTo(VolumeUnit.MILLILITRE);

            Assert.AreEqual(1000.0,               result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.MILLILITRE, result.Unit);
        }

        /// <summary>
        /// Verifies Millilitre to Litre conversion.
        /// 1000 mL = 1 L
        /// </summary>
        [TestMethod]
        public void testConversion_MillilitreToLitre()
        {
            var ml     = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var result = ml.ConvertTo(VolumeUnit.LITRE);

            Assert.AreEqual(1.0,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Verifies Gallon to Litre conversion.
        /// 1 gallon ≈ 3.78541 L
        /// </summary>
        [TestMethod]
        public void testConversion_GallonToLitre()
        {
            var gallon = new QuantityVolume(1.0, VolumeUnit.GALLON);
            var result = gallon.ConvertTo(VolumeUnit.LITRE);

            // ConvertTo() rounds to 2 decimal places: 3.78541 → 3.79
            Assert.AreEqual(3.79, result.Value, 0.01);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Verifies Litre to Gallon conversion.
        /// 3.78541 L ≈ 1 gallon
        /// </summary>
        [TestMethod]
        public void testConversion_LitreToGallon()
        {
            var litre  = new QuantityVolume(LitresPerGallon, VolumeUnit.LITRE);
            var result = litre.ConvertTo(VolumeUnit.GALLON);

            Assert.AreEqual(1.0,              result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.GALLON, result.Unit);
        }

        /// <summary>
        /// Verifies Millilitre to Gallon conversion.
        /// 1000 mL = 1 L ≈ 0.264172 gallons
        /// </summary>
        [TestMethod]
        public void testConversion_MillilitreToGallon()
        {
            var ml     = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var result = ml.ConvertTo(VolumeUnit.GALLON);

            // ConvertTo() rounds to 2 decimal places: 0.264172 → 0.26
            Assert.AreEqual(0.26, result.Value, 0.01);
            Assert.AreEqual(VolumeUnit.GALLON, result.Unit);
        }

        /// <summary>
        /// Verifies converting to same unit does not change the value.
        /// </summary>
        [TestMethod]
        public void testConversion_SameUnit()
        {
            var litre  = new QuantityVolume(5.0, VolumeUnit.LITRE);
            var result = litre.ConvertTo(VolumeUnit.LITRE);

            Assert.AreEqual(5.0,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Verifies zero conversion across units.
        /// 0 L = 0 mL
        /// </summary>
        [TestMethod]
        public void testConversion_ZeroValue()
        {
            var litre  = new QuantityVolume(0.0, VolumeUnit.LITRE);
            var result = litre.ConvertTo(VolumeUnit.MILLILITRE);

            Assert.AreEqual(0.0,                  result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.MILLILITRE, result.Unit);
        }

        /// <summary>
        /// Verifies negative value conversion preserves sign.
        /// -1 L = -1000 mL
        /// </summary>
        [TestMethod]
        public void testConversion_NegativeValue()
        {
            var litre  = new QuantityVolume(-1.0, VolumeUnit.LITRE);
            var result = litre.ConvertTo(VolumeUnit.MILLILITRE);

            Assert.AreEqual(-1000.0,              result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.MILLILITRE, result.Unit);
        }

        /// <summary>
        /// Verifies round-trip conversion accuracy.
        /// L → mL → L should return original value.
        /// </summary>
        [TestMethod]
        public void testConversion_RoundTrip()
        {
            var litre  = new QuantityVolume(1.5, VolumeUnit.LITRE);
            var inML   = litre.ConvertTo(VolumeUnit.MILLILITRE);
            var back   = inML.ConvertTo(VolumeUnit.LITRE);

            Assert.AreEqual(1.5,            back.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, back.Unit);
        }

        // ============================================================
        //                      ADDITION TESTS
        // ============================================================

        /// <summary>
        /// Same-unit addition: LITRE + LITRE.
        /// </summary>
        [TestMethod]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            var first  = new QuantityVolume(1.0, VolumeUnit.LITRE);
            var second = new QuantityVolume(2.0, VolumeUnit.LITRE);
            var result = first.Add(second);

            Assert.AreEqual(3.0,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Same-unit addition: MILLILITRE + MILLILITRE.
        /// </summary>
        [TestMethod]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            var first  = new QuantityVolume(500.0, VolumeUnit.MILLILITRE);
            var second = new QuantityVolume(500.0, VolumeUnit.MILLILITRE);
            var result = first.Add(second);

            Assert.AreEqual(1000.0,               result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.MILLILITRE, result.Unit);
        }

        /// <summary>
        /// Cross-unit addition: LITRE + MILLILITRE.
        /// Result in first operand's unit (LITRE).
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var result     = litre.Add(millilitre);

            Assert.AreEqual(2.0,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Cross-unit addition: MILLILITRE + LITRE.
        /// Result in first operand's unit (MILLILITRE).
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var result     = millilitre.Add(litre);

            Assert.AreEqual(2000.0,               result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.MILLILITRE, result.Unit);
        }

        /// <summary>
        /// Cross-unit addition: GALLON + LITRE.
        /// 1 GALLON + 3.78541 LITRE = 2 GALLON.
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            var gallon = new QuantityVolume(1.0,           VolumeUnit.GALLON);
            var litre  = new QuantityVolume(LitresPerGallon, VolumeUnit.LITRE);
            var result = gallon.Add(litre);

            Assert.AreEqual(2.0,              result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.GALLON, result.Unit);
        }

        /// <summary>
        /// Explicit target unit: result expressed in LITRE.
        /// </summary>
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var result     = litre.Add(millilitre, VolumeUnit.LITRE);

            Assert.AreEqual(2.0,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Explicit target unit: result expressed in MILLILITRE.
        /// </summary>
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            var result     = litre.Add(millilitre, VolumeUnit.MILLILITRE);

            Assert.AreEqual(2000.0,               result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.MILLILITRE, result.Unit);
        }

        /// <summary>
        /// Explicit target unit: result expressed in GALLON.
        /// 3.78541 L + 3.78541 L = 2 gallons.
        /// </summary>
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            var first  = new QuantityVolume(LitresPerGallon, VolumeUnit.LITRE);
            var second = new QuantityVolume(LitresPerGallon, VolumeUnit.LITRE);
            var result = first.Add(second, VolumeUnit.GALLON);

            Assert.AreEqual(2.0,              result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.GALLON, result.Unit);
        }

        /// <summary>
        /// Commutative property:
        /// A + B == B + A (after conversion to same unit).
        /// </summary>
        [TestMethod]
        public void testAddition_Commutativity()
        {
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);

            var result1 = litre.Add(millilitre);           // result in LITRE
            var result2 = millilitre.Add(litre);           // result in MILLILITRE

            Assert.IsTrue(result1.Equals(result2.ConvertTo(result1.Unit)));
        }

        /// <summary>
        /// Adding zero acts as identity element.
        /// </summary>
        [TestMethod]
        public void testAddition_WithZero()
        {
            var litre = new QuantityVolume(5.0, VolumeUnit.LITRE);
            var zero  = new QuantityVolume(0.0, VolumeUnit.MILLILITRE);
            var result = litre.Add(zero);

            Assert.AreEqual(5.0,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Addition with negative measurements.
        /// 5 L + (-2000 mL) = 3 L
        /// </summary>
        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var litre = new QuantityVolume(5.0,     VolumeUnit.LITRE);
            var neg   = new QuantityVolume(-2000.0, VolumeUnit.MILLILITRE);
            var result = litre.Add(neg);

            Assert.AreEqual(3.0,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Addition with very large numbers.
        /// </summary>
        [TestMethod]
        public void testAddition_LargeValues()
        {
            var first  = new QuantityVolume(1e6, VolumeUnit.LITRE);
            var second = new QuantityVolume(1e6, VolumeUnit.LITRE);
            var result = first.Add(second);

            Assert.AreEqual(2e6,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        /// <summary>
        /// Addition with small values that survive 2-decimal-place rounding.
        /// 0.1 L + 0.2 L = 0.3 L
        /// </summary>
        [TestMethod]
        public void testAddition_SmallValues()
        {
            var first  = new QuantityVolume(0.1, VolumeUnit.LITRE);
            var second = new QuantityVolume(0.2, VolumeUnit.LITRE);
            var result = first.Add(second);

            Assert.AreEqual(0.3,            result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, result.Unit);
        }

        // ============================================================
        //               VOLUMEUNIT ENUM CONSTANT TESTS
        // ============================================================

        /// <summary>
        /// Verifies LITRE conversion factor is 1.0 (base unit).
        /// </summary>
        [TestMethod]
        public void testVolumeUnitEnum_LitreConstant()
        {
            double factor = VolumeUnit.LITRE.GetConversionFactor();

            Assert.AreEqual(1.0, factor, EPSILON);
        }

        /// <summary>
        /// Verifies MILLILITRE conversion factor is 0.001.
        /// </summary>
        [TestMethod]
        public void testVolumeUnitEnum_MillilitreConstant()
        {
            double factor = VolumeUnit.MILLILITRE.GetConversionFactor();

            Assert.AreEqual(0.001, factor, EPSILON);
        }

        /// <summary>
        /// Verifies GALLON conversion factor is ~3.78541.
        /// </summary>
        [TestMethod]
        public void testVolumeUnitEnum_GallonConstant()
        {
            double factor = VolumeUnit.GALLON.GetConversionFactor();

            Assert.AreEqual(LitresPerGallon, factor, EPSILON);
        }

        // ============================================================
        //              CONVERT TO BASE UNIT TESTS
        // ============================================================

        /// <summary>
        /// Verifies LITRE.ConvertToBaseUnit returns same value (already base unit).
        /// </summary>
        [TestMethod]
        public void testConvertToBaseUnit_LitreToLitre()
        {
            double result = VolumeUnit.LITRE.ConvertToBaseUnit(5.0);

            Assert.AreEqual(5.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies MILLILITRE.ConvertToBaseUnit: 1000 mL = 1 L.
        /// </summary>
        [TestMethod]
        public void testConvertToBaseUnit_MillilitreToLitre()
        {
            double result = VolumeUnit.MILLILITRE.ConvertToBaseUnit(1000.0);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies GALLON.ConvertToBaseUnit: 1 gallon ≈ 3.78541 L.
        /// </summary>
        [TestMethod]
        public void testConvertToBaseUnit_GallonToLitre()
        {
            double result = VolumeUnit.GALLON.ConvertToBaseUnit(1.0);

            Assert.AreEqual(LitresPerGallon, result, EPSILON);
        }

        // ============================================================
        //             CONVERT FROM BASE UNIT TESTS
        // ============================================================

        /// <summary>
        /// Verifies LITRE.ConvertFromBaseUnit returns same value.
        /// </summary>
        [TestMethod]
        public void testConvertFromBaseUnit_LitreToLitre()
        {
            double result = VolumeUnit.LITRE.ConvertFromBaseUnit(2.0);

            Assert.AreEqual(2.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies MILLILITRE.ConvertFromBaseUnit: 1 L = 1000 mL.
        /// </summary>
        [TestMethod]
        public void testConvertFromBaseUnit_LitreToMillilitre()
        {
            double result = VolumeUnit.MILLILITRE.ConvertFromBaseUnit(1.0);

            Assert.AreEqual(1000.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies GALLON.ConvertFromBaseUnit: 3.78541 L = 1 gallon.
        /// </summary>
        [TestMethod]
        public void testConvertFromBaseUnit_LitreToGallon()
        {
            double result = VolumeUnit.GALLON.ConvertFromBaseUnit(LitresPerGallon);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        // ============================================================
        //           BACKWARD COMPATIBILITY & ARCHITECTURE TESTS
        // ============================================================

        /// <summary>
        /// Runs representative tests from UC1–UC10 to confirm nothing is broken.
        /// </summary>
        [TestMethod]
        public void testBackwardCompatibility_AllUC1Through10Tests()
        {
            // UC1 — Feet equality
            var feet1 = new Feet(10);
            var feet2 = new Feet(10);
            Assert.AreEqual(feet1, feet2);

            // UC3 — Length equality across units (1 foot == 12 inches)
            var length1 = new QuantityLength(1.0,  LengthEnum.FEET);
            var length2 = new QuantityLength(12.0, LengthEnum.INCH);
            Assert.IsTrue(length1.Equals(length2));

            // UC5 — Static length conversion
            double converted = QuantityLength.Convert(1.0, LengthEnum.FEET, LengthEnum.INCH);
            Assert.AreEqual(12.0, converted, EPSILON);

            // UC6 — Length addition
            var lengthSum = length1.Add(length2);
            Assert.AreEqual(2.0, lengthSum.Value, EPSILON);

            // UC9 — Weight equality (1 kg == 1000 g)
            var kg   = new QuantityWeight(1.0,    WeightUnit.KILOGRAM);
            var gram = new QuantityWeight(1000.0, WeightUnit.GRAM);
            Assert.IsTrue(kg.Equals(gram));

            // UC9 — Weight conversion
            var kgConverted = kg.ConvertTo(WeightUnit.GRAM);
            Assert.AreEqual(1000.0, kgConverted.Value, EPSILON);

            // UC9 — Weight addition with explicit target unit
            var weightSum = kg.Add(gram, WeightUnit.KILOGRAM);
            Assert.AreEqual(2.0, weightSum.Value, EPSILON);

            // UC10 — Generic Quantity<LengthUnitM>
            var gFeet   = new Quantity<LengthUnitM>(1.0,  LengthUnitM.FEET);
            var gInches = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            Assert.IsTrue(gFeet.Equals(gInches));

            // UC10 — Generic Quantity<WeightUnitM>
            var gKg   = new Quantity<WeightUnitM>(1.0,    WeightUnitM.KILOGRAM);
            var gGram = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);
            Assert.IsTrue(gKg.Equals(gGram));
        }

        /// <summary>
        /// Verifies QuantityVolume works identically to QuantityWeight and QuantityLength —
        /// no special-casing required anywhere.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_VolumeOperations_Consistency()
        {
            // Equality
            var litre      = new QuantityVolume(1.0,    VolumeUnit.LITRE);
            var millilitre = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);
            Assert.IsTrue(litre.Equals(millilitre));

            // Conversion
            var converted = litre.ConvertTo(VolumeUnit.MILLILITRE);
            Assert.AreEqual(1000.0,               converted.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.MILLILITRE, converted.Unit);

            // Addition with explicit target
            var sum = litre.Add(millilitre, VolumeUnit.LITRE);
            Assert.AreEqual(2.0,            sum.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
        }

        /// <summary>
        /// Confirms Volume integrates seamlessly — all three categories coexist
        /// and cross-category comparison is always false.
        /// Architecture scales linearly with zero changes to existing code.
        /// </summary>
        [TestMethod]
        public void testScalability_VolumeIntegration()
        {
            var volume = new QuantityVolume(1.0, VolumeUnit.LITRE);
            var length = new QuantityLength(1.0, LengthEnum.FEET);
            var weight = new QuantityWeight(1.0, WeightUnit.KILOGRAM);

            // All three instantiate without errors
            Assert.IsNotNull(volume);
            Assert.IsNotNull(length);
            Assert.IsNotNull(weight);

            // Runtime types are distinct — compiler enforces category isolation
            Assert.AreNotEqual(volume.GetType(), length.GetType());
            Assert.AreNotEqual(volume.GetType(), weight.GetType());
            Assert.AreNotEqual(length.GetType(), weight.GetType());

            // Cross-category equality always false
            Assert.IsFalse(volume.Equals(length));
            Assert.IsFalse(volume.Equals(weight));

            // Volume conversion works correctly (single hop, no accumulated rounding)
            var converted = new QuantityVolume(1.0, VolumeUnit.LITRE)
                .ConvertTo(VolumeUnit.MILLILITRE);
            Assert.AreEqual(1000.0, converted.Value, EPSILON);
        }
    }
}