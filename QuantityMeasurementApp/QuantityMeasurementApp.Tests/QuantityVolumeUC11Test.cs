using System;
using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class QuantityVolumeUC11Tests
    {
        private const double Epsilon     = 1e-4;
        private const double EpsilonWide = 1e-3;

        private static VolumeUnitMeasurable V(VolumeUnit u) => new VolumeUnitMeasurable(u);
        private static LengthUnitMeasurable L(LengthUnit u) => new LengthUnitMeasurable(u);
        private static WeightUnitMeasurable W(WeightUnit u) => new WeightUnitMeasurable(u);

        [Test]
        public void testEquality_LitreToLitre_SameValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_LitreToLitre_DifferentValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(2.0, V(VolumeUnit.Litre));
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void testEquality_LitreToMillilitre_EquivalentValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_MillilitreToLitre_EquivalentValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre));
            var b = new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_LitreToGallon_EquivalentValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0,      V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(0.264172, V(VolumeUnit.Gallon));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_GallonToLitre_EquivalentValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0,     V(VolumeUnit.Gallon));
            var b = new Quantity<VolumeUnitMeasurable>(3.78541, V(VolumeUnit.Litre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_VolumeVsLength_Incompatible()
        {
            var vol = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            var len = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            Assert.That(vol.Equals((object)len), Is.False);
        }

        [Test]
        public void testEquality_VolumeVsWeight_Incompatible()
        {
            var vol = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            var wt  = new Quantity<WeightUnitMeasurable>(1.0, W(WeightUnit.Kilogram));
            Assert.That(vol.Equals((object)wt), Is.False);
        }

        [Test]
        public void testEquality_NullComparison()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void testEquality_SameReference()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            Assert.That(a.Equals(a), Is.True);
        }

        [Test]
        public void testEquality_NullUnit()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Quantity<VolumeUnitMeasurable>(1.0, null!));
        }

        [Test]
        public void testEquality_TransitiveProperty()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre));
            var c = new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre));
            Assert.That(a.Equals(b), Is.True);
            Assert.That(b.Equals(c), Is.True);
            Assert.That(a.Equals(c), Is.True);
        }

        [Test]
        public void testEquality_ZeroValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(0.0, V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(0.0, V(VolumeUnit.Millilitre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_NegativeVolume()
        {
            var a = new Quantity<VolumeUnitMeasurable>(-1.0,    V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(-1000.0, V(VolumeUnit.Millilitre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_LargeVolumeValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1000000.0, V(VolumeUnit.Millilitre));
            var b = new Quantity<VolumeUnitMeasurable>(1000.0,    V(VolumeUnit.Litre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testEquality_SmallVolumeValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(0.001, V(VolumeUnit.Litre));
            var b = new Quantity<VolumeUnitMeasurable>(1.0,   V(VolumeUnit.Millilitre));
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void testConversion_LitreToMillilitre()
        {
            var result = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre)).ConvertTo(V(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(1000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Millilitre));
        }

        [Test]
        public void testConversion_MillilitreToLitre()
        {
            var result = new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre)).ConvertTo(V(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(1.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        [Test]
        public void testConversion_GallonToLitre()
        {
            var result = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Gallon)).ConvertTo(V(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(3.78541).Within(EpsilonWide));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        [Test]
        public void testConversion_LitreToGallon()
        {
            var result = new Quantity<VolumeUnitMeasurable>(3.78541, V(VolumeUnit.Litre)).ConvertTo(V(VolumeUnit.Gallon));
            Assert.That(result.Value, Is.EqualTo(1.0).Within(EpsilonWide));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Gallon));
        }

        [Test]
        public void testConversion_MillilitreToGallon()
        {
            var result = new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre)).ConvertTo(V(VolumeUnit.Gallon));
            Assert.That(result.Value, Is.EqualTo(0.264172).Within(EpsilonWide));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Gallon));
        }

        [Test]
        public void testConversion_SameUnit()
        {
            var result = new Quantity<VolumeUnitMeasurable>(5.0, V(VolumeUnit.Litre)).ConvertTo(V(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_ZeroValue()
        {
            var result = new Quantity<VolumeUnitMeasurable>(0.0, V(VolumeUnit.Litre)).ConvertTo(V(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            var result = new Quantity<VolumeUnitMeasurable>(-1.0, V(VolumeUnit.Litre)).ConvertTo(V(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(-1000.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_RoundTrip()
        {
            var original = new Quantity<VolumeUnitMeasurable>(1.5, V(VolumeUnit.Litre));
            var backToL  = original.ConvertTo(V(VolumeUnit.Millilitre)).ConvertTo(V(VolumeUnit.Litre));
            Assert.That(backToL.Value, Is.EqualTo(original.Value).Within(Epsilon));
        }

        [Test]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(2.0, V(VolumeUnit.Litre)));
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        [Test]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(500.0, V(VolumeUnit.Millilitre)),
                new Quantity<VolumeUnitMeasurable>(500.0, V(VolumeUnit.Millilitre)));
            Assert.That(result.Value, Is.EqualTo(1000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Millilitre));
        }

        [Test]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre)));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        [Test]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre)),
                new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre)));
            Assert.That(result.Value, Is.EqualTo(2000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Millilitre));
        }

        [Test]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(1.0,     V(VolumeUnit.Gallon)),
                new Quantity<VolumeUnitMeasurable>(3.78541, V(VolumeUnit.Litre)));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(EpsilonWide));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Gallon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre)),
                V(VolumeUnit.Litre));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre)),
                V(VolumeUnit.Millilitre));
            Assert.That(result.Value, Is.EqualTo(2000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Millilitre));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(3.78541, V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(3.78541, V(VolumeUnit.Litre)),
                V(VolumeUnit.Gallon));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(EpsilonWide));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Gallon));
        }

        [Test]
        public void testAddition_Commutativity()
        {
            var a  = new Quantity<VolumeUnitMeasurable>(1.0,    V(VolumeUnit.Litre));
            var b  = new Quantity<VolumeUnitMeasurable>(1000.0, V(VolumeUnit.Millilitre));
            var tU = V(VolumeUnit.Litre);
            var sum1 = Quantity<VolumeUnitMeasurable>.Add(a, b, tU);
            var sum2 = Quantity<VolumeUnitMeasurable>.Add(b, a, tU);
            Assert.That(sum1.Value, Is.EqualTo(sum2.Value).Within(Epsilon));
        }

        [Test]
        public void testAddition_WithZero()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(5.0, V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(0.0, V(VolumeUnit.Millilitre)));
            Assert.That(result.Value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(VolumeUnit.Litre));
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(5.0,     V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(-2000.0, V(VolumeUnit.Millilitre)));
            Assert.That(result.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_LargeValues()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(1e6, V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(1e6, V(VolumeUnit.Litre)));
            Assert.That(result.Value, Is.EqualTo(2e6).Within(1.0));
        }

        [Test]
        public void testAddition_SmallValues()
        {
            var result = Quantity<VolumeUnitMeasurable>.Add(
                new Quantity<VolumeUnitMeasurable>(0.001, V(VolumeUnit.Litre)),
                new Quantity<VolumeUnitMeasurable>(0.002, V(VolumeUnit.Litre)));
            Assert.That(result.Value, Is.EqualTo(0.003).Within(Epsilon));
        }

        [Test]
        public void testVolumeUnitEnum_LitreConstant()
        {
            Assert.That(VolumeUnit.Litre.GetConversionFactor(), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testVolumeUnitEnum_MillilitreConstant()
        {
            Assert.That(VolumeUnit.Millilitre.GetConversionFactor(), Is.EqualTo(0.001).Within(Epsilon));
        }

        [Test]
        public void testVolumeUnitEnum_GallonConstant()
        {
            Assert.That(VolumeUnit.Gallon.GetConversionFactor(), Is.EqualTo(3.78541).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_LitreToLitre()
        {
            Assert.That(VolumeUnit.Litre.ConvertToBaseUnit(5.0), Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_MillilitreToLitre()
        {
            Assert.That(VolumeUnit.Millilitre.ConvertToBaseUnit(1000.0), Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_GallonToLitre()
        {
            Assert.That(VolumeUnit.Gallon.ConvertToBaseUnit(1.0), Is.EqualTo(3.78541).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_LitreToLitre()
        {
            Assert.That(VolumeUnit.Litre.ConvertFromBaseUnit(2.0), Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_LitreToMillilitre()
        {
            Assert.That(VolumeUnit.Millilitre.ConvertFromBaseUnit(1.0), Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_LitreToGallon()
        {
            Assert.That(VolumeUnit.Gallon.ConvertFromBaseUnit(3.78541), Is.EqualTo(1.0).Within(EpsilonWide));
        }

        [Test]
        public void testBackwardCompatibility_AllUC1Through10Tests()
        {
            var len1 = new QuantityLength(1.0, LengthUnit.Feet);
            var len2 = new QuantityLength(12.0, LengthUnit.Inch);
            Assert.That(len1.Equals(len2), Is.True);

            var wt1 = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var wt2 = new QuantityWeight(1000.0, WeightUnit.Gram);
            Assert.That(wt1.Equals(wt2), Is.True);
        }

        [Test]
        public void testGenericQuantity_VolumeOperations_Consistency()
        {
            var litre      = V(VolumeUnit.Litre);
            var millilitre = V(VolumeUnit.Millilitre);
            var gallon     = V(VolumeUnit.Gallon);

            Assert.That(litre.GetConversionFactor(),      Is.EqualTo(1.0).Within(Epsilon));
            Assert.That(millilitre.GetConversionFactor(), Is.EqualTo(0.001).Within(Epsilon));
            Assert.That(gallon.GetConversionFactor(),     Is.EqualTo(3.78541).Within(Epsilon));

            Assert.That(litre.ConvertToBaseUnit(5.0),         Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(millilitre.ConvertToBaseUnit(1000.0), Is.EqualTo(1.0).Within(Epsilon));
            Assert.That(gallon.ConvertToBaseUnit(1.0),        Is.EqualTo(3.78541).Within(Epsilon));

            Assert.That(litre.ConvertFromBaseUnit(5.0),      Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(millilitre.ConvertFromBaseUnit(1.0), Is.EqualTo(1000.0).Within(Epsilon));
            Assert.That(gallon.ConvertFromBaseUnit(3.78541), Is.EqualTo(1.0).Within(EpsilonWide));
        }

        [Test]
        public void testScalability_VolumeIntegration()
        {
            var v1 = new Quantity<VolumeUnitMeasurable>(1.0,     V(VolumeUnit.Litre));
            var v2 = new Quantity<VolumeUnitMeasurable>(1000.0,  V(VolumeUnit.Millilitre));
            var v3 = new Quantity<VolumeUnitMeasurable>(3.78541, V(VolumeUnit.Litre));
            var v4 = new Quantity<VolumeUnitMeasurable>(1.0,     V(VolumeUnit.Gallon));

            Assert.That(v1.Equals(v2), Is.True);
            Assert.That(v3.Equals((object)v4), Is.True);

            var converted = v1.ConvertTo(V(VolumeUnit.Millilitre));
            Assert.That(converted.Value, Is.EqualTo(1000.0).Within(Epsilon));

            var sum = Quantity<VolumeUnitMeasurable>.Add(v1, v2, V(VolumeUnit.Litre));
            Assert.That(sum.Value, Is.EqualTo(2.0).Within(Epsilon));

            var len = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            Assert.That(v1.Equals((object)len), Is.False);
        }
    }
}