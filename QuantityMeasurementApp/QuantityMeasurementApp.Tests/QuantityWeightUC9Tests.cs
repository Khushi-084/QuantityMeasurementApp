using System;
using NUnit.Framework;
using QuantityMeasurementApp.Domain;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC9: Weight Measurement Equality, Conversion, and Addition (Kilogram, Gram, Pound).
    /// Tests WeightUnit conversion methods and QuantityWeight behavior.
    /// </summary>
    [TestFixture]
    public class QuantityWeightUC9Tests
    {
        private const double Epsilon = 1e-6;

        // --------------------- WeightUnit Enum Constants ---------------------

        [Test]
        public void testWeightUnitEnum_KilogramConstant()
        {
            Assert.That(WeightUnit.Kilogram.GetConversionFactor(), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testWeightUnitEnum_GramConstant()
        {
            Assert.That(WeightUnit.Gram.GetConversionFactor(), Is.EqualTo(0.001).Within(Epsilon));
        }

        [Test]
        public void testWeightUnitEnum_PoundConstant()
        {
            Assert.That(WeightUnit.Pound.GetConversionFactor(), Is.EqualTo(0.453592).Within(Epsilon));
        }

        // --------------------- ConvertToBaseUnit (→ Kilogram) ---------------------

        [Test]
        public void testConvertToBaseUnit_KilogramToKilogram()
        {
            Assert.That(WeightUnit.Kilogram.ConvertToBaseUnit(5.0), Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_GramToKilogram()
        {
            Assert.That(WeightUnit.Gram.ConvertToBaseUnit(1000.0), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_PoundToKilogram()
        {
            Assert.That(WeightUnit.Pound.ConvertToBaseUnit(2.20462), Is.EqualTo(1.0).Within(1e-5));
        }

        // --------------------- ConvertFromBaseUnit (Kilogram → unit) ---------------------

        [Test]
        public void testConvertFromBaseUnit_KilogramToKilogram()
        {
            Assert.That(WeightUnit.Kilogram.ConvertFromBaseUnit(2.0), Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_KilogramToGram()
        {
            Assert.That(WeightUnit.Gram.ConvertFromBaseUnit(1.0), Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_KilogramToPound()
        {
            Assert.That(WeightUnit.Pound.ConvertFromBaseUnit(1.0), Is.EqualTo(2.20462).Within(1e-5));
        }

        // --------------------- QuantityWeight Equality ---------------------

        [Test]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(2.0, WeightUnit.Kilogram);
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(1000.0, WeightUnit.Gram);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            var a = new QuantityWeight(1000.0, WeightUnit.Gram);
            var b = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_KilogramToPound_EquivalentValue()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(2.20462, WeightUnit.Pound);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_GramToPound_EquivalentValue()
        {
            var a = new QuantityWeight(453.592, WeightUnit.Gram);
            var b = new QuantityWeight(1.0, WeightUnit.Pound);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_WeightVsLength_Incompatible()
        {
            var weight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var length = new QuantityLength(1.0, LengthUnit.Feet);
            Assert.That(weight.Equals(length), Is.False);
        }

        [Test]
        public void testEquality_NullComparison()
        {
            var weight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.That(weight.Equals(null), Is.False);
        }

        [Test]
        public void testEquality_SameReference()
        {
            var weight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.That(weight.Equals(weight), Is.True);
        }

        [Test]
        public void testEquality_NullUnit()
        {
            Assert.Throws<ArgumentException>(() => new QuantityWeight(1.0, (WeightUnit)(-1)));
        }

        [Test]
        public void testEquality_ZeroValue()
        {
            var a = new QuantityWeight(0.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(0.0, WeightUnit.Gram);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_NegativeWeight()
        {
            var a = new QuantityWeight(-1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(-1000.0, WeightUnit.Gram);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_LargeWeightValue()
        {
            var a = new QuantityWeight(100000.0, WeightUnit.Gram);
            var b = new QuantityWeight(100.0, WeightUnit.Kilogram);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_SmallWeightValue()
        {
            var a = new QuantityWeight(0.001, WeightUnit.Kilogram);
            var b = new QuantityWeight(1.0, WeightUnit.Gram);
            Assert.That(a.Equals(b), Is.True);
        }

        // --------------------- QuantityWeight Conversion ---------------------

        [Test]
        public void testConversion_SameUnit()
        {
            var q = new QuantityWeight(5.0, WeightUnit.Kilogram);
            var converted = q.ConvertTo(WeightUnit.Kilogram);
            Assert.That(converted.Value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(converted.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testConversion_KilogramToGram()
        {
            var q = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var converted = q.ConvertTo(WeightUnit.Gram);
            Assert.That(converted.Value, Is.EqualTo(1000.0).Within(Epsilon));
            Assert.That(converted.Unit, Is.EqualTo(WeightUnit.Gram));
        }

        [Test]
        public void testConversion_PoundToKilogram()
        {
            var q = new QuantityWeight(2.20462, WeightUnit.Pound);
            var converted = q.ConvertTo(WeightUnit.Kilogram);
            Assert.That(converted.Value, Is.EqualTo(1.0).Within(1e-5));
            Assert.That(converted.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testConversion_GramToPound()
        {
            var q = new QuantityWeight(500.0, WeightUnit.Gram);
            var converted = q.ConvertTo(WeightUnit.Pound);
            Assert.That(converted.Value, Is.EqualTo(1.10231).Within(1e-5));
            Assert.That(converted.Unit, Is.EqualTo(WeightUnit.Pound));
        }

        [Test]
        public void testConversion_ZeroValue()
        {
            var q = new QuantityWeight(0.0, WeightUnit.Kilogram);
            var converted = q.ConvertTo(WeightUnit.Gram);
            Assert.That(converted.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            var q = new QuantityWeight(-1.0, WeightUnit.Kilogram);
            var converted = q.ConvertTo(WeightUnit.Gram);
            Assert.That(converted.Value, Is.EqualTo(-1000.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_RoundTrip()
        {
            var original = new QuantityWeight(1.5, WeightUnit.Kilogram);
            var toGram = original.ConvertTo(WeightUnit.Gram);
            var backToKg = toGram.ConvertTo(WeightUnit.Kilogram);
            Assert.That(backToKg.Value, Is.EqualTo(original.Value).Within(Epsilon));
        }

        // --------------------- QuantityWeight Addition ---------------------

        [Test]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(2.0, WeightUnit.Kilogram);
            var result = QuantityWeight.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testAddition_CrossUnit_KilogramPlusGram_ImplicitUnit()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(1000.0, WeightUnit.Gram);
            var result = QuantityWeight.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testAddition_CrossUnit_KilogramPlusGram_ExplicitTargetGram()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(1000.0, WeightUnit.Gram);
            var result = QuantityWeight.Add(a, b, WeightUnit.Gram);
            Assert.That(result.Value, Is.EqualTo(2000.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Gram));
        }

        [Test]
        public void testAddition_CrossUnit_PoundPlusKilogram_TargetKilogram()
        {
            var a = new QuantityWeight(2.0, WeightUnit.Pound);
            var b = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var result = QuantityWeight.Add(a, b, WeightUnit.Kilogram);
            // 2 lb ≈ 0.907184 kg, so sum ≈ 1.907184 kg
            Assert.That(result.Value, Is.EqualTo(1.907184).Within(1e-5));
            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testAddition_Commutativity_WithTargetUnit()
        {
            var a = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(1000.0, WeightUnit.Gram);

            var sum1 = QuantityWeight.Add(a, b, WeightUnit.Kilogram);
            var sum2 = QuantityWeight.Add(b, a, WeightUnit.Kilogram);

            Assert.That(sum1.Value, Is.EqualTo(sum2.Value).Within(Epsilon));
            Assert.That(sum1.Unit, Is.EqualTo(sum2.Unit));
        }

        [Test]
        public void testAddition_WithZero()
        {
            var a = new QuantityWeight(5.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(0.0, WeightUnit.Gram);
            var result = QuantityWeight.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            var a = new QuantityWeight(5.0, WeightUnit.Kilogram);
            var b = new QuantityWeight(-2000.0, WeightUnit.Gram);
            var result = QuantityWeight.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testAddition_LargeValues()
        {
            var a = new QuantityWeight(1e5, WeightUnit.Kilogram);
            var b = new QuantityWeight(1e5, WeightUnit.Kilogram);
            var result = QuantityWeight.Add(a, b);
            Assert.That(result.Value, Is.EqualTo(2e5).Within(1.0));
            Assert.That(result.Unit, Is.EqualTo(WeightUnit.Kilogram));
        }
    }
}

