using System;
using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC10: Tests for the generic Quantity&lt;T&gt; class and IMeasurable interface.
    /// Verifies backward compatibility with UC1–UC9 and the new generic design.
    /// </summary>
    [TestFixture]
    public class QuantityUC10Tests
    {
        private const double Epsilon = 1e-4;

        // ── Helpers to avoid repetition ──────────────────────────────────
        private static LengthUnitMeasurable L(LengthUnit u) => new LengthUnitMeasurable(u);
        private static WeightUnitMeasurable W(WeightUnit u) => new WeightUnitMeasurable(u);

        // ─────────────────── IMeasurable — LengthUnit ───────────────────

        [Test]
        public void testIMeasurable_LengthUnit_GetConversionFactor_Feet()
        {
            IMeasurable unit = L(LengthUnit.Feet);
            Assert.That(unit.GetConversionFactor(), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_LengthUnit_GetConversionFactor_Inch()
        {
            IMeasurable unit = L(LengthUnit.Inch);
            Assert.That(unit.GetConversionFactor(), Is.EqualTo(1.0 / 12.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_LengthUnit_GetConversionFactor_Yard()
        {
            IMeasurable unit = L(LengthUnit.Yard);
            Assert.That(unit.GetConversionFactor(), Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_LengthUnit_ConvertToBaseUnit_InchToFeet()
        {
            IMeasurable unit = L(LengthUnit.Inch);
            Assert.That(unit.ConvertToBaseUnit(12.0), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_LengthUnit_ConvertFromBaseUnit_FeetToInch()
        {
            IMeasurable unit = L(LengthUnit.Inch);
            Assert.That(unit.ConvertFromBaseUnit(1.0), Is.EqualTo(12.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_LengthUnit_GetUnitName()
        {
            IMeasurable unit = L(LengthUnit.Feet);
            Assert.That(unit.GetUnitName(), Is.EqualTo("FEET"));
        }

        // ─────────────────── IMeasurable — WeightUnit ───────────────────

        [Test]
        public void testIMeasurable_WeightUnit_GetConversionFactor_Kilogram()
        {
            IMeasurable unit = W(WeightUnit.Kilogram);
            Assert.That(unit.GetConversionFactor(), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_WeightUnit_GetConversionFactor_Gram()
        {
            IMeasurable unit = W(WeightUnit.Gram);
            Assert.That(unit.GetConversionFactor(), Is.EqualTo(0.001).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_WeightUnit_ConvertToBaseUnit_GramToKilogram()
        {
            IMeasurable unit = W(WeightUnit.Gram);
            Assert.That(unit.ConvertToBaseUnit(1000.0), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_WeightUnit_ConvertFromBaseUnit_KilogramToGram()
        {
            IMeasurable unit = W(WeightUnit.Gram);
            Assert.That(unit.ConvertFromBaseUnit(1.0), Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testIMeasurable_WeightUnit_GetUnitName()
        {
            IMeasurable unit = W(WeightUnit.Kilogram);
            Assert.That(unit.GetUnitName(), Is.EqualTo("KILOGRAM"));
        }

        // ────────────────── Quantity<T> Constructor Validation ───────────

        [Test]
        public void testQuantity_Constructor_NullUnit_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Quantity<LengthUnitMeasurable>(1.0, null!));
        }

        [Test]
        public void testQuantity_Constructor_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new Quantity<LengthUnitMeasurable>(double.NaN, L(LengthUnit.Feet)));
        }

        [Test]
        public void testQuantity_Constructor_Infinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new Quantity<WeightUnitMeasurable>(double.PositiveInfinity, W(WeightUnit.Kilogram)));
        }

        // ──────────────── Quantity<LengthUnitMeasurable> Equality ───────

        [Test]
        public void testGenericQuantity_Length_Equality_FeetToFeet_SameValue()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            var b = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testGenericQuantity_Length_Equality_FeetToInch_Equivalent()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0,  L(LengthUnit.Feet));
            var b = new Quantity<LengthUnitMeasurable>(12.0, L(LengthUnit.Inch));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testGenericQuantity_Length_Equality_YardToFeet_Equivalent()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Yard));
            var b = new Quantity<LengthUnitMeasurable>(3.0, L(LengthUnit.Feet));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testGenericQuantity_Length_Equality_DifferentValues_False()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            var b = new Quantity<LengthUnitMeasurable>(2.0, L(LengthUnit.Feet));
            Assert.That(a.Equals(b), Is.False);
        }

        // ──────────────── Quantity<WeightUnitMeasurable> Equality ───────

        [Test]
        public void testGenericQuantity_Weight_Equality_KilogramToKilogram_SameValue()
        {
            var a = new Quantity<WeightUnitMeasurable>(1.0, W(WeightUnit.Kilogram));
            var b = new Quantity<WeightUnitMeasurable>(1.0, W(WeightUnit.Kilogram));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testGenericQuantity_Weight_Equality_KilogramToGram_Equivalent()
        {
            var a = new Quantity<WeightUnitMeasurable>(1.0,    W(WeightUnit.Kilogram));
            var b = new Quantity<WeightUnitMeasurable>(1000.0, W(WeightUnit.Gram));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testGenericQuantity_Weight_Equality_GramToPound_Equivalent()
        {
            var a = new Quantity<WeightUnitMeasurable>(453.592, W(WeightUnit.Gram));
            var b = new Quantity<WeightUnitMeasurable>(1.0,     W(WeightUnit.Pound));
            Assert.That(a.Equals(b), Is.True);
        }

        // ──────────────── Cross-Category Prevention ─────────────────────

        [Test]
        public void testCrossCategory_LengthVsWeight_EqualityReturnsFalse()
        {
            var length = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            var weight = new Quantity<WeightUnitMeasurable>(1.0, W(WeightUnit.Kilogram));
            Assert.That(length.Equals((object)weight), Is.False);
        }

        [Test]
        public void testCrossCategory_NullComparison_ReturnsFalse()
        {
            var q = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            Assert.That(q.Equals(null), Is.False);
        }

        [Test]
        public void testCrossCategory_SameReference_ReturnsTrue()
        {
            var q = new Quantity<WeightUnitMeasurable>(5.0, W(WeightUnit.Gram));
            Assert.That(q.Equals(q), Is.True);
        }

        // ──────────────── Conversion ─────────────────────────────────────

        [Test]
        public void testGenericQuantity_Length_ConvertTo_FeetToInch()
        {
            var q      = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            var result = q.ConvertTo(L(LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(12.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testGenericQuantity_Weight_ConvertTo_KilogramToGram()
        {
            var q      = new Quantity<WeightUnitMeasurable>(1.0, W(WeightUnit.Kilogram));
            var result = q.ConvertTo(W(WeightUnit.Gram));
            Assert.That(result.Value, Is.EqualTo(1000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(WeightUnit.Gram));
        }

        [Test]
        public void testGenericQuantity_Conversion_RoundTrip_Length()
        {
            var original = new Quantity<LengthUnitMeasurable>(1.5, L(LengthUnit.Feet));
            var toInch   = original.ConvertTo(L(LengthUnit.Inch));
            var backToFt = toInch.ConvertTo(L(LengthUnit.Feet));
            Assert.That(backToFt.Value, Is.EqualTo(original.Value).Within(Epsilon));
        }

        [Test]
        public void testGenericQuantity_Conversion_ZeroValue()
        {
            var q      = new Quantity<WeightUnitMeasurable>(0.0, W(WeightUnit.Kilogram));
            var result = q.ConvertTo(W(WeightUnit.Gram));
            Assert.That(result.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testGenericQuantity_ConvertTo_NullTarget_Throws()
        {
            var q = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            Assert.Throws<ArgumentNullException>(() => q.ConvertTo(null!));
        }

        // ──────────────── Addition ────────────────────────────────────────

        [Test]
        public void testGenericQuantity_Length_Add_FeetPlusInch_ResultInFeet()
        {
            var a      = new Quantity<LengthUnitMeasurable>(1.0,  L(LengthUnit.Feet));
            var b      = new Quantity<LengthUnitMeasurable>(12.0, L(LengthUnit.Inch));
            var result = Quantity<LengthUnitMeasurable>.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        [Test]
        public void testGenericQuantity_Length_Add_WithExplicitTargetUnit()
        {
            var a      = new Quantity<LengthUnitMeasurable>(1.0,  L(LengthUnit.Feet));
            var b      = new Quantity<LengthUnitMeasurable>(12.0, L(LengthUnit.Inch));
            var result = Quantity<LengthUnitMeasurable>.Add(a, b, L(LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(24.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Inch));
        }

        [Test]
        public void testGenericQuantity_Weight_Add_KilogramPlusGram_ResultInKilogram()
        {
            var a      = new Quantity<WeightUnitMeasurable>(1.0,    W(WeightUnit.Kilogram));
            var b      = new Quantity<WeightUnitMeasurable>(1000.0, W(WeightUnit.Gram));
            var result = Quantity<WeightUnitMeasurable>.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testGenericQuantity_Weight_Add_WithExplicitTargetUnit()
        {
            var a      = new Quantity<WeightUnitMeasurable>(1.0,    W(WeightUnit.Kilogram));
            var b      = new Quantity<WeightUnitMeasurable>(1000.0, W(WeightUnit.Gram));
            var result = Quantity<WeightUnitMeasurable>.Add(a, b, W(WeightUnit.Gram));
            Assert.That(result.Value, Is.EqualTo(2000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(WeightUnit.Gram));
        }

        [Test]
        public void testGenericQuantity_Add_WithZero()
        {
            var a      = new Quantity<LengthUnitMeasurable>(5.0, L(LengthUnit.Feet));
            var b      = new Quantity<LengthUnitMeasurable>(0.0, L(LengthUnit.Inch));
            var result = Quantity<LengthUnitMeasurable>.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testGenericQuantity_Add_NullOperand_Throws()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            Assert.Throws<ArgumentNullException>(() => Quantity<LengthUnitMeasurable>.Add(a, null!));
        }

        [Test]
        public void testGenericQuantity_Add_Commutativity_WithTargetUnit()
        {
            var a  = new Quantity<WeightUnitMeasurable>(1.0,    W(WeightUnit.Kilogram));
            var b  = new Quantity<WeightUnitMeasurable>(1000.0, W(WeightUnit.Gram));
            var kg = W(WeightUnit.Kilogram);
            var sum1 = Quantity<WeightUnitMeasurable>.Add(a, b, kg);
            var sum2 = Quantity<WeightUnitMeasurable>.Add(b, a, kg);
            Assert.That(sum1.Value, Is.EqualTo(sum2.Value).Within(Epsilon));
        }

        // ──────────────── Immutability ────────────────────────────────────

        [Test]
        public void testGenericQuantity_Immutability_OriginalsUnchanged()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            var b = new Quantity<LengthUnitMeasurable>(2.0, L(LengthUnit.Feet));
            _ = Quantity<LengthUnitMeasurable>.Add(a, b);
            Assert.That(a.Value, Is.EqualTo(1.0));
            Assert.That(b.Value, Is.EqualTo(2.0));
        }

        // ──────────────── HashCode Consistency ───────────────────────────

        [Test]
        public void testGenericQuantity_HashCode_EqualObjects_SameHash()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0,  L(LengthUnit.Feet));
            var b = new Quantity<LengthUnitMeasurable>(12.0, L(LengthUnit.Inch));
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        // ──────────────── Scalability: New Category ──────────────────────

        [Test]
        public void testScalability_NewVolumeCategory_WorksWithGenericQuantity()
        {
            var litre      = new TestVolumeUnit(1.0,   "LITRE");
            var millilitre = new TestVolumeUnit(0.001, "MILLILITRE");
            var a = new Quantity<TestVolumeUnit>(1.0,    litre);
            var b = new Quantity<TestVolumeUnit>(1000.0, millilitre);
            Assert.That(a.Equals(b), Is.True);
        }

        // ──────────────── ToString ────────────────────────────────────────

        [Test]
        public void testGenericQuantity_ToString_ContainsValueAndUnit()
        {
            var q = new Quantity<LengthUnitMeasurable>(3.0, L(LengthUnit.Feet));
            string s = q.ToString();
            Assert.That(s, Does.Contain("3").And.Contain("FEET"));
        }

        // ──── Helper stub for scalability test ────────────────────────────

        private sealed class TestVolumeUnit : IMeasurable
        {
            private readonly double _factor;
            private readonly string _name;
            public TestVolumeUnit(double factor, string name) { _factor = factor; _name = name; }
            public double GetConversionFactor()         => _factor;
            public double ConvertToBaseUnit(double v)   => v * _factor;
            public double ConvertFromBaseUnit(double b) => b / _factor;
            public string GetUnitName()                 => _name;
        }
    }
}