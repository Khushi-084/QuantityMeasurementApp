using System;
using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class QuantityUC12Tests
    {
        private const double Epsilon = 1e-4;

        private static LengthUnitMeasurable L(LengthUnit u) => new LengthUnitMeasurable(u);
        private static WeightUnitMeasurable W(WeightUnit u) => new WeightUnitMeasurable(u);
        private static VolumeUnitMeasurable V(VolumeUnit u) => new VolumeUnitMeasurable(u);

        private static Quantity<LengthUnitMeasurable> Len(double v, LengthUnit u)
            => new Quantity<LengthUnitMeasurable>(v, L(u));
        private static Quantity<WeightUnitMeasurable> Wgt(double v, WeightUnit u)
            => new Quantity<WeightUnitMeasurable>(v, W(u));
        private static Quantity<VolumeUnitMeasurable> Vol(double v, VolumeUnit u)
            => new Quantity<VolumeUnitMeasurable>(v, V(u));

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_SameUnit_FeetMinusFeet
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            var result = Len(10.0, LengthUnit.Feet).Subtract(Len(5.0, LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_SameUnit_LitreMinusLitre
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            var result = Vol(10.0, VolumeUnit.Litre).Subtract(Vol(3.0, VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(7.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_CrossUnit_FeetMinusInches
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_CrossUnit_FeetMinusInches()
        {
            // 10 feet − 6 inches = 9.5 feet
            var result = Len(10.0, LengthUnit.Feet).Subtract(Len(6.0, LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(9.5).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_CrossUnit_InchesMinusFeet
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_CrossUnit_InchesMinusFeet()
        {
            // 120 inches − 5 feet = 60 inches
            var result = Len(120.0, LengthUnit.Inch).Subtract(Len(5.0, LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(60.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_ExplicitTargetUnit_Feet
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Feet()
        {
            // 10 feet − 6 inches expressed in feet = 9.5 feet
            var result = Len(10.0, LengthUnit.Feet)
                .Subtract(Len(6.0, LengthUnit.Inch), L(LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(9.5).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_ExplicitTargetUnit_Inches
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            // 10 feet − 6 inches expressed in inches = 114 inches
            var result = Len(10.0, LengthUnit.Feet)
                .Subtract(Len(6.0, LengthUnit.Inch), L(LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(114.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_ExplicitTargetUnit_Millilitre
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            // 5 L − 2 L expressed in millilitre = 3000 mL
            var result = Vol(5.0, VolumeUnit.Litre)
                .Subtract(Vol(2.0, VolumeUnit.Litre), V(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(3000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Millilitre));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_ResultingInNegative
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_ResultingInNegative()
        {
            var result = Len(5.0, LengthUnit.Feet).Subtract(Len(10.0, LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(-5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_ResultingInZero
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_ResultingInZero()
        {
            // 10 feet − 120 inches = 0 feet
            var result = Len(10.0, LengthUnit.Feet).Subtract(Len(120.0, LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_WithZeroOperand
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_WithZeroOperand()
        {
            // 5 feet − 0 inches = 5 feet
            var result = Len(5.0, LengthUnit.Feet).Subtract(Len(0.0, LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_WithNegativeValues
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_WithNegativeValues()
        {
            // 5 feet − (−2 feet) = 7 feet
            var result = Len(5.0, LengthUnit.Feet).Subtract(Len(-2.0, LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(7.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_NonCommutative
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_NonCommutative()
        {
            var ab = Len(10.0, LengthUnit.Feet).Subtract(Len(5.0, LengthUnit.Feet));
            var ba = Len(5.0,  LengthUnit.Feet).Subtract(Len(10.0, LengthUnit.Feet));
            Assert.That(ab.Value, Is.EqualTo( 5.0).Within(Epsilon));
            Assert.That(ba.Value, Is.EqualTo(-5.0).Within(Epsilon));
            Assert.That(ab.Value, Is.Not.EqualTo(ba.Value));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_WithLargeValues
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_WithLargeValues()
        {
            var result = Wgt(1_000_000.0, WeightUnit.Kilogram)
                .Subtract(Wgt(500_000.0, WeightUnit.Kilogram));
            Assert.That(result.Value, Is.EqualTo(500_000.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_WithSmallValues
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_WithSmallValues()
        {
            var result = Len(0.001, LengthUnit.Feet).Subtract(Len(0.0005, LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(0.0005).Within(1e-3));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_NullOperand
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_NullOperand()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Len(10.0, LengthUnit.Feet).Subtract(null!));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_NullTargetUnit
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_NullTargetUnit()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Len(10.0, LengthUnit.Feet).Subtract(Len(5.0, LengthUnit.Feet), null!));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_CrossCategory
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_CrossCategory()
        {
            // Cross-category is prevented at compile time by the generic type system.
            // Quantity<LengthUnitMeasurable>.Subtract(Quantity<WeightUnitMeasurable>) won't compile.
            // This test documents that the type system enforces category safety.
            Assert.Pass("Cross-category subtraction is prevented at compile time by generic type constraints.");
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_AllMeasurementCategories
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_AllMeasurementCategories()
        {
            Assert.That(Len(5.0, LengthUnit.Feet).Subtract(Len(2.0, LengthUnit.Feet)).Value,
                        Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(Wgt(5.0, WeightUnit.Kilogram).Subtract(Wgt(2.0, WeightUnit.Kilogram)).Value,
                        Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(Vol(5.0, VolumeUnit.Litre).Subtract(Vol(2.0, VolumeUnit.Litre)).Value,
                        Is.EqualTo(3.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_ChainedOperations
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_ChainedOperations()
        {
            // 10 ft − 2 ft − 1 ft = 7 ft
            var result = Len(10.0, LengthUnit.Feet)
                .Subtract(Len(2.0, LengthUnit.Feet))
                .Subtract(Len(1.0, LengthUnit.Feet));
            Assert.That(result.Value, Is.EqualTo(7.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_SameUnit_FeetDividedByFeet
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            double ratio = Len(10.0, LengthUnit.Feet).Divide(Len(2.0, LengthUnit.Feet));
            Assert.That(ratio, Is.EqualTo(5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_SameUnit_LitreDividedByLitre
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_SameUnit_LitreDividedByLitre()
        {
            double ratio = Vol(10.0, VolumeUnit.Litre).Divide(Vol(5.0, VolumeUnit.Litre));
            Assert.That(ratio, Is.EqualTo(2.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_CrossUnit_FeetDividedByInches
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_CrossUnit_FeetDividedByInches()
        {
            // 24 inches ÷ 2 feet = 1.0
            double ratio = Len(24.0, LengthUnit.Inch).Divide(Len(2.0, LengthUnit.Feet));
            Assert.That(ratio, Is.EqualTo(1.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_CrossUnit_KilogramDividedByGram
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_CrossUnit_KilogramDividedByGram()
        {
            // 2 kg ÷ 2000 g = 1.0
            double ratio = Wgt(2.0, WeightUnit.Kilogram).Divide(Wgt(2000.0, WeightUnit.Gram));
            Assert.That(ratio, Is.EqualTo(1.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_RatioGreaterThanOne
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_RatioGreaterThanOne()
        {
            double ratio = Len(10.0, LengthUnit.Feet).Divide(Len(2.0, LengthUnit.Feet));
            Assert.That(ratio, Is.GreaterThan(1.0));
            Assert.That(ratio, Is.EqualTo(5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_RatioLessThanOne
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_RatioLessThanOne()
        {
            double ratio = Vol(5.0, VolumeUnit.Litre).Divide(Vol(10.0, VolumeUnit.Litre));
            Assert.That(ratio, Is.LessThan(1.0));
            Assert.That(ratio, Is.EqualTo(0.5).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_RatioEqualToOne
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_RatioEqualToOne()
        {
            double ratio = Len(10.0, LengthUnit.Feet).Divide(Len(10.0, LengthUnit.Feet));
            Assert.That(ratio, Is.EqualTo(1.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_NonCommutative
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_NonCommutative()
        {
            double ab = Len(10.0, LengthUnit.Feet).Divide(Len(5.0, LengthUnit.Feet));
            double ba = Len(5.0,  LengthUnit.Feet).Divide(Len(10.0, LengthUnit.Feet));
            Assert.That(ab, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(ba, Is.EqualTo(0.5).Within(Epsilon));
            Assert.That(ab, Is.Not.EqualTo(ba));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_ByZero
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_ByZero()
        {
            Assert.Throws<ArithmeticException>(() =>
                Len(10.0, LengthUnit.Feet).Divide(Len(0.0, LengthUnit.Feet)));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_WithLargeRatio
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_WithLargeRatio()
        {
            double ratio = Wgt(1_000_000.0, WeightUnit.Kilogram)
                .Divide(Wgt(1.0, WeightUnit.Kilogram));
            Assert.That(ratio, Is.EqualTo(1_000_000.0).Within(1.0));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_WithSmallRatio
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_WithSmallRatio()
        {
            double ratio = Wgt(1.0, WeightUnit.Kilogram)
                .Divide(Wgt(1_000_000.0, WeightUnit.Kilogram));
            Assert.That(ratio, Is.EqualTo(1e-6).Within(1e-10));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_NullOperand
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_NullOperand()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Len(10.0, LengthUnit.Feet).Divide(null!));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_CrossCategory
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_CrossCategory()
        {
            // Cross-category is prevented at compile time by the generic type system.
            // Quantity<LengthUnitMeasurable>.Divide(Quantity<WeightUnitMeasurable>) won't compile.
            // This test documents that the type system enforces category safety.
            Assert.Pass("Cross-category division is prevented at compile time by generic type constraints.");
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_AllMeasurementCategories
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_AllMeasurementCategories()
        {
            Assert.That(Len(10.0, LengthUnit.Feet).Divide(Len(2.0, LengthUnit.Feet)),
                        Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(Wgt(10.0, WeightUnit.Kilogram).Divide(Wgt(2.0, WeightUnit.Kilogram)),
                        Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(Vol(10.0, VolumeUnit.Litre).Divide(Vol(2.0, VolumeUnit.Litre)),
                        Is.EqualTo(5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_Associativity
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_Associativity()
        {
            // (12 ÷ 6) ÷ 2 = 1.0   vs   12 ÷ (6 ÷ 2) = 4.0  — non-associative
            var a = Len(12.0, LengthUnit.Feet);
            var b = Len(6.0,  LengthUnit.Feet);
            var c = Len(2.0,  LengthUnit.Feet);

            double lhs = Len(a.Divide(b), LengthUnit.Feet).Divide(c);
            double rhs = a.Divide(Len(b.Divide(c), LengthUnit.Feet));

            Assert.That(lhs, Is.EqualTo(1.0).Within(Epsilon));
            Assert.That(rhs, Is.EqualTo(4.0).Within(Epsilon));
            Assert.That(lhs, Is.Not.EqualTo(rhs));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtractionAndDivision_Integration
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtractionAndDivision_Integration()
        {
            // A.subtract(B).divide(C) is valid — operations coexist
            // (10 ft − 4 ft) ÷ 3 ft = 6 ÷ 3 = 2.0
            var diff  = Len(10.0, LengthUnit.Feet).Subtract(Len(4.0, LengthUnit.Feet));
            double ratio = diff.Divide(Len(3.0, LengthUnit.Feet));
            Assert.That(ratio, Is.EqualTo(2.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtractionAddition_Inverse
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtractionAddition_Inverse()
        {
            // A.add(B).subtract(B) ≈ A
            var a = Len(7.5, LengthUnit.Feet);
            var b = Len(2.5, LengthUnit.Feet);
            var result = a.Add(b).Subtract(b);
            Assert.That(result.Value, Is.EqualTo(a.Value).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_Immutability
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_Immutability()
        {
            var a = Len(10.0, LengthUnit.Feet);
            var b = Len(3.0,  LengthUnit.Feet);
            _ = a.Subtract(b);
            Assert.That(a.Value, Is.EqualTo(10.0).Within(Epsilon));
            Assert.That(b.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_Immutability
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_Immutability()
        {
            var a = Len(10.0, LengthUnit.Feet);
            var b = Len(2.0,  LengthUnit.Feet);
            _ = a.Divide(b);
            Assert.That(a.Value, Is.EqualTo(10.0).Within(Epsilon));
            Assert.That(b.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtraction_PrecisionAndRounding
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtraction_PrecisionAndRounding()
        {
            // Result must be rounded to exactly 2 decimal places
            var result = Len(13.0, LengthUnit.Inch).Subtract(Len(1.0, LengthUnit.Feet));
            double rounded = Math.Round(result.Value, 2);
            Assert.That(result.Value, Is.EqualTo(rounded).Within(1e-9));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivision_PrecisionHandling
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivision_PrecisionHandling()
        {
            // Division returns raw double with no forced rounding
            double ratio = Len(10.0, LengthUnit.Feet).Divide(Len(3.0, LengthUnit.Feet));
            Assert.That(ratio, Is.EqualTo(10.0 / 3.0).Within(Epsilon));
        }
    }
}