using System;
using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.Interface;
using static QuantityMeasurementApp.Domain.TemperatureUnit;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class QuantityUC14Tests
    {
        // ── Helpers ───────────────────────────────────────────────────────────
        private static TemperatureUnitMeasurable Temp(TemperatureUnit u) => new TemperatureUnitMeasurable(u);
        private static LengthUnitMeasurable      L(LengthUnit u)         => new LengthUnitMeasurable(u);
        private static WeightUnitMeasurable      W(WeightUnit u)         => new WeightUnitMeasurable(u);
        private static VolumeUnitMeasurable      V(VolumeUnit u)         => new VolumeUnitMeasurable(u);

        private static Quantity<TemperatureUnitMeasurable> T(double value, TemperatureUnit unit)
            => new Quantity<TemperatureUnitMeasurable>(value, Temp(unit));

        private const double Eps = 1e-2;

        // ═════════════════════════════════════════════════════════════════════
        // EQUALITY
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
            => Assert.That(T(0.0, Celsius).Equals(T(0.0, Celsius)), Is.True);

        [Test]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
            => Assert.That(T(32.0, Fahrenheit).Equals(T(32.0, Fahrenheit)), Is.True);

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
            => Assert.That(T(0.0, Celsius).Equals(T(32.0, Fahrenheit)), Is.True);

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
            => Assert.That(T(100.0, Celsius).Equals(T(212.0, Fahrenheit)), Is.True);

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
            => Assert.That(T(-40.0, Celsius).Equals(T(-40.0, Fahrenheit)), Is.True);

        [Test]
        public void testTemperatureEquality_SymmetricProperty()
        {
            var a = T(0.0, Celsius);
            var b = T(32.0, Fahrenheit);
            Assert.That(a.Equals(b), Is.True);
            Assert.That(b.Equals(a), Is.True);
        }

        [Test]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            var q = T(100.0, Celsius);
            Assert.That(q.Equals(q), Is.True);
        }

        // ═════════════════════════════════════════════════════════════════════
        // CONVERSION
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
        {
            // 50°C → 122°F,  -20°C → -4°F,  100°C → 212°F
            Assert.That(T(50.0,   Celsius).ConvertTo(Temp(Fahrenheit)).Value, Is.EqualTo(122.0).Within(Eps));
            Assert.That(T(-20.0,  Celsius).ConvertTo(Temp(Fahrenheit)).Value, Is.EqualTo(-4.0).Within(Eps));
            Assert.That(T(100.0,  Celsius).ConvertTo(Temp(Fahrenheit)).Value, Is.EqualTo(212.0).Within(Eps));
        }

        [Test]
        public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
        {
            // 122°F → 50°C,  -4°F → -20°C,  212°F → 100°C
            Assert.That(T(122.0, Fahrenheit).ConvertTo(Temp(Celsius)).Value, Is.EqualTo(50.0).Within(Eps));
            Assert.That(T(-4.0,  Fahrenheit).ConvertTo(Temp(Celsius)).Value, Is.EqualTo(-20.0).Within(Eps));
            Assert.That(T(212.0, Fahrenheit).ConvertTo(Temp(Celsius)).Value, Is.EqualTo(100.0).Within(Eps));
        }

        [Test]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            double original = 75.0;
            var toF   = T(original, Celsius).ConvertTo(Temp(Fahrenheit));
            var backC = toF.ConvertTo(Temp(Celsius));
            Assert.That(backC.Value, Is.EqualTo(original).Within(Eps));
        }

        [Test]
        public void testTemperatureConversion_SameUnit()
        {
            var result = T(50.0, Celsius).ConvertTo(Temp(Celsius));
            Assert.That(result.Value, Is.EqualTo(50.0).Within(Eps));
        }

        [Test]
        public void testTemperatureConversion_ZeroValue()
        {
            // 0°C → 32°F
            var result = T(0.0, Celsius).ConvertTo(Temp(Fahrenheit));
            Assert.That(result.Value, Is.EqualTo(32.0).Within(Eps));
        }

        [Test]
        public void testTemperatureConversion_NegativeValues()
        {
            // -20°C → -4°F
            var result = T(-20.0, Celsius).ConvertTo(Temp(Fahrenheit));
            Assert.That(result.Value, Is.EqualTo(-4.0).Within(Eps));
        }

        [Test]
        public void testTemperatureConversion_LargeValues()
        {
            // 1000°C → 1832°F
            var result = T(1000.0, Celsius).ConvertTo(Temp(Fahrenheit));
            Assert.That(result.Value, Is.EqualTo(1832.0).Within(Eps));
        }

        // ═════════════════════════════════════════════════════════════════════
        // UNSUPPORTED OPERATIONS
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureUnsupportedOperation_Add()
            => Assert.Throws<NotSupportedException>(() => T(100.0, Celsius).Add(T(50.0, Celsius)));

        [Test]
        public void testTemperatureUnsupportedOperation_Subtract()
            => Assert.Throws<NotSupportedException>(() => T(100.0, Celsius).Subtract(T(50.0, Celsius)));

        [Test]
        public void testTemperatureUnsupportedOperation_Divide()
            => Assert.Throws<NotSupportedException>(() => T(100.0, Celsius).Divide(T(50.0, Celsius)));

        [Test]
        public void testTemperatureUnsupportedOperation_ErrorMessage()
        {
            var ex = Assert.Throws<NotSupportedException>(() => T(100.0, Celsius).Add(T(50.0, Celsius)));
            Assert.That(ex!.Message, Does.Contain("Add"));
            Assert.That(ex!.Message, Does.Contain("Temperature"));
        }

        // ═════════════════════════════════════════════════════════════════════
        // CROSS-CATEGORY PREVENTION
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureVsLengthIncompatibility()
        {
            var temp   = T(100.0, Celsius);
            var length = new Quantity<LengthUnitMeasurable>(100.0, L(LengthUnit.Feet));
            Assert.That(temp.Equals(length), Is.False);
        }

        [Test]
        public void testTemperatureVsWeightIncompatibility()
        {
            var temp   = T(50.0, Celsius);
            var weight = new Quantity<WeightUnitMeasurable>(50.0, W(WeightUnit.Kilogram));
            Assert.That(temp.Equals(weight), Is.False);
        }

        [Test]
        public void testTemperatureVsVolumeIncompatibility()
        {
            var temp   = T(25.0, Celsius);
            var volume = new Quantity<VolumeUnitMeasurable>(25.0, V(VolumeUnit.Litre));
            Assert.That(temp.Equals(volume), Is.False);
        }

        // ═════════════════════════════════════════════════════════════════════
        // OPERATION SUPPORT METHODS
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testOperationSupportMethods_TemperatureUnitAddition()
            => Assert.That(Temp(Celsius).SupportsArithmeticOp(), Is.False);

        [Test]
        public void testOperationSupportMethods_TemperatureUnitDivision()
            => Assert.That(Temp(Fahrenheit).SupportsArithmeticOp(), Is.False);

        [Test]
        public void testOperationSupportMethods_LengthUnitAddition()
            => Assert.That(((IMeasurable)L(LengthUnit.Feet)).SupportsArithmeticOp(), Is.True);

        [Test]
        public void testOperationSupportMethods_WeightUnitDivision()
            => Assert.That(((IMeasurable)W(WeightUnit.Kilogram)).SupportsArithmeticOp(), Is.True);

        // ═════════════════════════════════════════════════════════════════════
        // INTERFACE EVOLUTION & BACKWARD COMPATIBILITY
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testIMeasurableInterface_Evolution_BackwardCompatible()
        {
            // Existing units work without any modification — no breaking changes
            var feet = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            var inch = new Quantity<LengthUnitMeasurable>(12.0, L(LengthUnit.Inch));
            Assert.That(feet.Equals(inch), Is.True);

            var kg  = new Quantity<WeightUnitMeasurable>(1.0, W(WeightUnit.Kilogram));
            var gm  = new Quantity<WeightUnitMeasurable>(1000.0, W(WeightUnit.Gram));
            Assert.That(kg.Equals(gm), Is.True);
        }

        [Test]
        public void testTemperatureDefaultMethodInheritance()
        {
            // Non-temperature units inherit default true from IMeasurable
            Assert.That(((IMeasurable)L(LengthUnit.Feet)).SupportsArithmeticOp(),     Is.True);
            Assert.That(((IMeasurable)W(WeightUnit.Kilogram)).SupportsArithmeticOp(), Is.True);
            Assert.That(((IMeasurable)V(VolumeUnit.Litre)).SupportsArithmeticOp(),    Is.True);
        }

        // ═════════════════════════════════════════════════════════════════════
        // TEMPERATURE UNIT STRUCTURE
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureUnit_NonLinearConversion()
        {
            // Verify temperature uses formula-based conversion, not simple multiplication
            // If it were linear: 100°C * factor = 212°F  → factor = 2.12
            // But 0°C should then = 0°F, which it doesn't (0°C = 32°F)
            // This proves non-linear (offset) conversion is in use
            Assert.That(T(0.0, Celsius).ConvertTo(Temp(Fahrenheit)).Value,   Is.EqualTo(32.0).Within(Eps));
            Assert.That(T(100.0, Celsius).ConvertTo(Temp(Fahrenheit)).Value, Is.EqualTo(212.0).Within(Eps));
        }

        [Test]
        public void testTemperatureUnit_AllConstants()
        {
            Assert.That(Temp(Celsius),    Is.Not.Null);
            Assert.That(Temp(Fahrenheit), Is.Not.Null);
            Assert.That(Temp(Kelvin),     Is.Not.Null);
        }

        [Test]
        public void testTemperatureUnit_NameMethod()
        {
            Assert.That(Temp(Celsius).GetUnitName(),    Is.EqualTo("CELSIUS"));
            Assert.That(Temp(Fahrenheit).GetUnitName(), Is.EqualTo("FAHRENHEIT"));
            Assert.That(Temp(Kelvin).GetUnitName(),     Is.EqualTo("KELVIN"));
        }

        [Test]
        public void testTemperatureUnit_ConversionFactor()
            => Assert.That(Temp(Celsius).GetConversionFactor(), Is.EqualTo(1.0));

        // ═════════════════════════════════════════════════════════════════════
        // NULL & INEQUALITY
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureNullUnitValidation()
            => Assert.Throws<ArgumentNullException>(() =>
                new Quantity<TemperatureUnitMeasurable>(100.0, null!));

        [Test]
        public void testTemperatureNullOperandValidation_InComparison()
            => Assert.That(T(100.0, Celsius).Equals(null), Is.False);

        [Test]
        public void testTemperatureDifferentValuesInequality()
            => Assert.That(T(50.0, Celsius).Equals(T(100.0, Celsius)), Is.False);

        // ═════════════════════════════════════════════════════════════════════
        // BACKWARD COMPATIBILITY UC1–UC13
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureBackwardCompatibility_UC1_Through_UC13()
        {
            // Length add
            var lenA = new Quantity<LengthUnitMeasurable>(1.0, L(LengthUnit.Feet));
            var lenB = new Quantity<LengthUnitMeasurable>(12.0, L(LengthUnit.Inch));
            Assert.That(lenA.Add(lenB).Value, Is.EqualTo(2.0).Within(Eps));

            // Weight subtract
            var wA = new Quantity<WeightUnitMeasurable>(1.0, W(WeightUnit.Kilogram));
            var wB = new Quantity<WeightUnitMeasurable>(500.0, W(WeightUnit.Gram));
            Assert.That(wA.Subtract(wB).Value, Is.EqualTo(0.5).Within(Eps));

            // Volume divide
            var vA = new Quantity<VolumeUnitMeasurable>(2.0, V(VolumeUnit.Litre));
            var vB = new Quantity<VolumeUnitMeasurable>(1.0, V(VolumeUnit.Litre));
            Assert.That(vA.Divide(vB), Is.EqualTo(2.0).Within(Eps));
        }

        // ═════════════════════════════════════════════════════════════════════
        // PRECISION & EDGE CASES
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureConversionPrecision_Epsilon()
        {
            // 50°C = 122°F within epsilon tolerance
            var result = T(50.0, Celsius).ConvertTo(Temp(Fahrenheit));
            Assert.That(result.Value, Is.EqualTo(122.0).Within(Eps));
        }

        [Test]
        public void testTemperatureConversionEdgeCase_VerySmallDifference()
        {
            // Values very close to each other should still be equal within epsilon
            Assert.That(T(99.999, Celsius).Equals(T(211.9982, Fahrenheit)), Is.True);
        }

        // ═════════════════════════════════════════════════════════════════════
        // INTERFACE & GENERIC INTEGRATION
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureEnumImplementsIMeasurable()
            => Assert.That(Temp(Celsius), Is.InstanceOf<IMeasurable>());

        [Test]
        public void testTemperatureCrossUnitAdditionAttempt()
        {
            // Adding different temperature units also throws NotSupportedException
            Assert.Throws<NotSupportedException>(() =>
                T(100.0, Celsius).Add(T(212.0, Fahrenheit)));
        }

        [Test]
        public void testTemperatureValidateOperationSupport_MethodBehavior()
        {
            // Direct call to ValidateOperationSupport throws NotSupportedException
            var ex = Assert.Throws<NotSupportedException>(() =>
                Temp(Celsius).ValidateOperationSupport("addition"));
            Assert.That(ex!.Message, Does.Contain("addition"));
        }

        [Test]
        public void testTemperatureIntegrationWithGenericQuantity()
        {
            // Quantity<TemperatureUnitMeasurable> works seamlessly with the generic class
            var t1     = T(0.0, Celsius);
            var t2     = T(32.0, Fahrenheit);
            var result = t1.ConvertTo(Temp(Fahrenheit));
            Assert.That(t1.Equals(t2),      Is.True);
            Assert.That(result.Value,        Is.EqualTo(32.0).Within(Eps));
            Assert.That(result.Unit.Unit,    Is.EqualTo(Fahrenheit));
        }

        // ═════════════════════════════════════════════════════════════════════
        // EXTRA TEST CASES
        // ═════════════════════════════════════════════════════════════════════

        [Test]
        public void testTemperatureConversion_KelvinToFahrenheit_273Point15K_Gives32F()
        {
            // 273.15 K = 0°C = 32°F
            var result = T(273.15, Kelvin).ConvertTo(Temp(Fahrenheit));
            Assert.That(result.Value, Is.EqualTo(32.0).Within(Eps));
        }

        [Test]
        public void testTemperatureEquality_KelvinToCelsius_AbsoluteZero()
        {
            // 0 K = -273.15°C
            Assert.That(T(0.0, Kelvin).Equals(T(-273.15, Celsius)), Is.True);
        }

        [Test]
        public void testTemperatureConversion_TargetUnitStoredCorrectly()
        {
            // After conversion, unit of result must be the target unit
            var result = T(100.0, Celsius).ConvertTo(Temp(Kelvin));
            Assert.That(result.Unit.Unit, Is.EqualTo(Kelvin));
            Assert.That(result.Value, Is.EqualTo(373.15).Within(Eps));
        }

        [Test]
        public void testTemperatureUnsupportedOperation_AddWithDifferentUnits()
        {
            // Add across different temperature units also throws NotSupportedException
            Assert.Throws<NotSupportedException>(() =>
                T(100.0, Celsius).Add(T(373.15, Kelvin)));
        }

        [Test]
        public void testTemperatureEquality_TransitiveProperty()
        {
            // If A == B and B == C then A == C
            var a = T(0.0, Celsius);
            var b = T(32.0, Fahrenheit);
            var c = T(273.15, Kelvin);
            Assert.That(a.Equals(b), Is.True);
            Assert.That(b.Equals(c), Is.True);
            Assert.That(a.Equals(c), Is.True);
        }

        [Test]
        public void testTemperatureConversion_RoundTrip_KelvinToCelsiusAndBack()
        {
            double original = 300.0;
            var toC   = T(original, Kelvin).ConvertTo(Temp(Celsius));
            var backK = toC.ConvertTo(Temp(Kelvin));
            Assert.That(backK.Value, Is.EqualTo(original).Within(Eps));
        }
    }
}