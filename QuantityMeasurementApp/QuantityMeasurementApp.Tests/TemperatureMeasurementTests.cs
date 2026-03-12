using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementModel.Interfaces;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC14: Temperature Measurement with Selective Arithmetic Support.
    ///
    /// Tests verify:
    ///  1.  Temperature equality within and across units (Celsius, Fahrenheit, Kelvin).
    ///  2.  Temperature conversion accuracy using non-linear formulas.
    ///  3.  Unsupported arithmetic (add/subtract/divide) throws NotSupportedException.
    ///  4.  Cross-category prevention (temperature vs length/weight/volume).
    ///  5.  IMeasurable interface evolution — existing units unaffected.
    ///  6.  SupportsArithmetic() and ValidateOperationSupport() methods work correctly.
    ///  7.  Null handling and edge cases (absolute zero, -40 equal point).
    ///  8.  All UC1–UC13 behaviors preserved (backward compatibility).
    /// </summary>
    [TestClass]
    public class TemperatureMeasurementTests
    {
        private const double EPSILON = 0.01;

        // ============================================================
        //   GROUP 1: EQUALITY — WITHIN SAME UNIT
        // ============================================================

        /// <summary>
        /// Verifies two Celsius quantities with the same value are equal.
        /// Tests: Baseline same-unit equality for temperature.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            var t1 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            Assert.IsTrue(t1.Equals(t2));
        }

        /// <summary>
        /// Verifies two Fahrenheit quantities with the same value are equal.
        /// Tests: Same-unit equality for Fahrenheit.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            var t1 = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);
            var t2 = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

            Assert.IsTrue(t1.Equals(t2));
        }

        /// <summary>
        /// Verifies two Kelvin quantities with the same value are equal.
        /// Tests: Same-unit equality for Kelvin.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_KelvinToKelvin_SameValue()
        {
            var t1 = new Quantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);
            var t2 = new Quantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);

            Assert.IsTrue(t1.Equals(t2));
        }

        // ============================================================
        //   GROUP 2: EQUALITY — CROSS UNIT
        // ============================================================

        /// <summary>
        /// Verifies 0°C equals 32°F after Kelvin normalization.
        /// Tests: Cross-unit equality Celsius to Fahrenheit.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            var celsius    = new Quantity<TemperatureUnit>(0.0,  TemperatureUnit.CELSIUS);
            var fahrenheit = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        /// <summary>
        /// Verifies 100°C equals 212°F.
        /// Tests: Cross-unit equality at boiling point.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            var celsius    = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var fahrenheit = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.FAHRENHEIT);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        /// <summary>
        /// Verifies -40°C equals -40°F (the unique equal point of the two scales).
        /// Tests: Edge case where Celsius and Fahrenheit intersect.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            var celsius    = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);
            var fahrenheit = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.FAHRENHEIT);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        /// <summary>
        /// Verifies 0°C equals 273.15 K.
        /// Tests: Cross-unit equality Celsius to Kelvin.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToKelvin_0Celsius27315Kelvin()
        {
            var celsius = new Quantity<TemperatureUnit>(0.0,    TemperatureUnit.CELSIUS);
            var kelvin  = new Quantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);

            Assert.IsTrue(celsius.Equals(kelvin));
        }

        /// <summary>
        /// Verifies symmetric property: if A equals B then B equals A.
        /// Tests: Bidirectional equality for temperature.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_SymmetricProperty()
        {
            var celsius    = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var fahrenheit = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.FAHRENHEIT);

            Assert.IsTrue(celsius.Equals(fahrenheit));
            Assert.IsTrue(fahrenheit.Equals(celsius));
        }

        /// <summary>
        /// Verifies reflexive property: a temperature equals itself.
        /// Tests: Same-reference equality.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            var t = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            Assert.IsTrue(t.Equals(t));
        }

        /// <summary>
        /// Verifies two different temperature values are not equal.
        /// Tests: Inequality for different temperature values.
        /// </summary>
        [TestMethod]
        public void testTemperatureDifferentValuesInequality()
        {
            var t1 = new Quantity<TemperatureUnit>(50.0,  TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            Assert.IsFalse(t1.Equals(t2));
        }

        // ============================================================
        //   GROUP 3: CONVERSION ACCURACY
        // ============================================================

        /// <summary>
        /// Verifies 100°C converts to 212°F.
        /// Tests: Celsius to Fahrenheit conversion formula.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_CelsiusToFahrenheit_100C_212F()
        {
            var celsius = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var result  = celsius.ConvertTo(TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(212.0, result.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.FAHRENHEIT, result.Unit);
        }

        /// <summary>
        /// Verifies 32°F converts to 0°C.
        /// Tests: Fahrenheit to Celsius conversion formula.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_FahrenheitToCelsius_32F_0C()
        {
            var fahrenheit = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);
            var result     = fahrenheit.ConvertTo(TemperatureUnit.CELSIUS);

            Assert.AreEqual(0.0, result.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.CELSIUS, result.Unit);
        }

        /// <summary>
        /// Verifies 0°C converts to 273.15 K.
        /// Tests: Celsius to Kelvin conversion formula.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_CelsiusToKelvin_0C_27315K()
        {
            var celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var result  = celsius.ConvertTo(TemperatureUnit.KELVIN);

            Assert.AreEqual(273.15, result.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.KELVIN, result.Unit);
        }

        /// <summary>
        /// Verifies 273.15 K converts to 0°C.
        /// Tests: Kelvin to Celsius conversion formula.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_KelvinToCelsius_27315K_0C()
        {
            var kelvin = new Quantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);
            var result = kelvin.ConvertTo(TemperatureUnit.CELSIUS);

            Assert.AreEqual(0.0, result.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.CELSIUS, result.Unit);
        }

        /// <summary>
        /// Verifies converting to the same unit returns the same value.
        /// Tests: Identity conversion (same source and target).
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_SameUnit_ReturnsSameValue()
        {
            var celsius = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);
            var result  = celsius.ConvertTo(TemperatureUnit.CELSIUS);

            Assert.AreEqual(50.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies a round-trip conversion preserves the original value.
        /// Tests: Convert C->F->C returns original.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            var original  = new Quantity<TemperatureUnit>(37.0, TemperatureUnit.CELSIUS);
            var toF       = original.ConvertTo(TemperatureUnit.FAHRENHEIT);
            var backToC   = toF.ConvertTo(TemperatureUnit.CELSIUS);

            Assert.AreEqual(original.Value, backToC.Value, EPSILON);
        }

        /// <summary>
        /// Verifies negative temperature conversions are accurate.
        /// Tests: -20°C = -4°F.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_NegativeValues()
        {
            var celsius = new Quantity<TemperatureUnit>(-20.0, TemperatureUnit.CELSIUS);
            var result  = celsius.ConvertTo(TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(-4.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies large temperature values convert accurately.
        /// Tests: 1000°C = 1832°F.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_LargeValues()
        {
            var celsius = new Quantity<TemperatureUnit>(1000.0, TemperatureUnit.CELSIUS);
            var result  = celsius.ConvertTo(TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(1832.0, result.Value, EPSILON);
        }

        // ============================================================
        //   GROUP 4: UNSUPPORTED ARITHMETIC OPERATIONS
        // ============================================================

        /// <summary>
        /// Verifies Add() throws NotSupportedException for temperature.
        /// Tests: Arithmetic not supported for absolute temperature values.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_Add()
        {
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0,  TemperatureUnit.CELSIUS);

            try
            {
                t1.Add(t2);
                Assert.Fail("Expected NotSupportedException for temperature Add.");
            }
            catch (NotSupportedException) { /* expected */ }
        }

        /// <summary>
        /// Verifies Subtract() throws NotSupportedException for temperature.
        /// Tests: Subtraction not supported for absolute temperature values.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_Subtract()
        {
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0,  TemperatureUnit.CELSIUS);

            try
            {
                t1.Subtract(t2);
                Assert.Fail("Expected NotSupportedException for temperature Subtract.");
            }
            catch (NotSupportedException) { /* expected */ }
        }

        /// <summary>
        /// Verifies Divide() throws NotSupportedException for temperature.
        /// Tests: Division not supported for absolute temperature values.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_Divide()
        {
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0,  TemperatureUnit.CELSIUS);

            try
            {
                t1.Divide(t2);
                Assert.Fail("Expected NotSupportedException for temperature Divide.");
            }
            catch (NotSupportedException) { /* expected */ }
        }

        /// <summary>
        /// Verifies the exception message clearly explains why the operation is unsupported.
        /// Tests: Error message clarity and informativeness.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_ErrorMessage_IsInformative()
        {
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0,  TemperatureUnit.CELSIUS);

            try
            {
                t1.Add(t2);
                Assert.Fail("Expected NotSupportedException.");
            }
            catch (NotSupportedException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Temperature"),
                    "Error message should mention 'Temperature'.");
                Assert.IsTrue(ex.Message.Length > 20,
                    "Error message should be informative, not a one-word error.");
            }
        }

        // ============================================================
        //   GROUP 5: CROSS-CATEGORY PREVENTION
        // ============================================================

        /// <summary>
        /// Verifies temperature does not equal a length quantity.
        /// Tests: Cross-category prevention — temperature vs length.
        /// </summary>
        [TestMethod]
        public void testTemperatureVsLengthIncompatibility()
        {
            var temp   = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var length = new Quantity<LengthUnitM>(100.0, LengthUnitM.FEET);

            Assert.IsFalse(temp.Equals(length));
        }

        /// <summary>
        /// Verifies temperature does not equal a weight quantity.
        /// Tests: Cross-category prevention — temperature vs weight.
        /// </summary>
        [TestMethod]
        public void testTemperatureVsWeightIncompatibility()
        {
            var temp   = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);
            var weight = new Quantity<WeightUnitM>(50.0, WeightUnitM.KILOGRAM);

            Assert.IsFalse(temp.Equals(weight));
        }

        /// <summary>
        /// Verifies temperature does not equal a volume quantity.
        /// Tests: Cross-category prevention — temperature vs volume.
        /// </summary>
        [TestMethod]
        public void testTemperatureVsVolumeIncompatibility()
        {
            var temp   = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.CELSIUS);
            var volume = new Quantity<VolumeUnitM>(25.0, VolumeUnitM.LITRE);

            Assert.IsFalse(temp.Equals(volume));
        }

        // ============================================================
        //   GROUP 6: OPERATION SUPPORT METHODS
        // ============================================================

        /// <summary>
        /// Verifies TemperatureUnit.CELSIUS.SupportsArithmetic() returns false.
        /// Tests: Capability-based design — temperature does not support arithmetic.
        /// </summary>
        [TestMethod]
        public void testOperationSupportMethods_TemperatureUnit_SupportsArithmetic_ReturnsFalse()
        {
            Assert.IsFalse(TemperatureUnit.CELSIUS.SupportsArithmetic());
            Assert.IsFalse(TemperatureUnit.FAHRENHEIT.SupportsArithmetic());
            Assert.IsFalse(TemperatureUnit.KELVIN.SupportsArithmetic());
        }

        /// <summary>
        /// Verifies LengthUnitM inherits SupportsArithmetic() default returning true.
        /// Tests: Backward compatibility — existing units unaffected by IMeasurable changes.
        /// </summary>
        [TestMethod]
        public void testOperationSupportMethods_LengthUnit_SupportsArithmetic_ReturnsTrue()
        {
            Assert.IsTrue(((IMeasurable)LengthUnitM.FEET).SupportsArithmetic());
            Assert.IsTrue(((IMeasurable)LengthUnitM.INCHES).SupportsArithmetic());
        }

        /// <summary>
        /// Verifies WeightUnitM inherits SupportsArithmetic() default returning true.
        /// Tests: Backward compatibility — weight unaffected by IMeasurable changes.
        /// </summary>
        [TestMethod]
        public void testOperationSupportMethods_WeightUnit_SupportsArithmetic_ReturnsTrue()
        {
            Assert.IsTrue(((IMeasurable)WeightUnitM.KILOGRAM).SupportsArithmetic());
        }

        /// <summary>
        /// Verifies VolumeUnitM inherits SupportsArithmetic() default returning true.
        /// Tests: Backward compatibility — volume unaffected by IMeasurable changes.
        /// </summary>
        [TestMethod]
        public void testOperationSupportMethods_VolumeUnit_SupportsArithmetic_ReturnsTrue()
        {
            Assert.IsTrue(((IMeasurable)VolumeUnitM.LITRE).SupportsArithmetic());
        }

        // ============================================================
        //   GROUP 7: IMeasurable INTERFACE EVOLUTION
        // ============================================================

        /// <summary>
        /// Verifies TemperatureUnit implements IMeasurable correctly.
        /// Tests: Interface contract satisfied for temperature.
        /// </summary>
        [TestMethod]
        public void testTemperatureEnumImplementsIMeasurable()
        {
            IMeasurable unit = TemperatureUnit.CELSIUS;

            Assert.IsNotNull(unit.GetUnitName());
            Assert.IsNotNull(unit.ConvertToBaseUnit(0.0));
            Assert.IsNotNull(unit.ConvertFromBaseUnit(273.15));
        }

        /// <summary>
        /// Verifies existing units work without modification after UC14 IMeasurable changes.
        /// Tests: IMeasurable interface evolution is backward compatible.
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_Evolution_BackwardCompatible()
        {
            // All existing units should still work exactly as before
            var feet = new Quantity<LengthUnitM>(1.0,  LengthUnitM.FEET);
            var inch = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            var sum  = feet.Add(inch);

            Assert.AreEqual(2.0, sum.Value, EPSILON);

            var kg = new Quantity<WeightUnitM>(1.0,    WeightUnitM.KILOGRAM);
            var g  = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);
            Assert.IsTrue(kg.Equals(g));
        }

        /// <summary>
        /// Verifies ValidateOperationSupport throws NotSupportedException directly when called on temperature.
        /// Tests: ValidateOperationSupport() method behavior for temperature.
        /// </summary>
        [TestMethod]
        public void testTemperatureValidateOperationSupport_ThrowsNotSupportedException()
        {
            try
            {
                TemperatureUnit.CELSIUS.ValidateOperationSupport("addition");
                Assert.Fail("Expected NotSupportedException.");
            }
            catch (NotSupportedException) { /* expected */ }
        }

        /// <summary>
        /// Verifies non-temperature units do NOT throw from ValidateOperationSupport (default no-op).
        /// Tests: Default no-op behavior for existing units.
        /// </summary>
        [TestMethod]
        public void testTemperatureDefaultMethodInheritance_NonTemperatureUnits_NoThrow()
        {
            // Should not throw — default no-op implementation
            ((IMeasurable)LengthUnitM.FEET).ValidateOperationSupport("addition");
            ((IMeasurable)WeightUnitM.KILOGRAM).ValidateOperationSupport("division");
            ((IMeasurable)VolumeUnitM.LITRE).ValidateOperationSupport("subtraction");
        }

        // ============================================================
        //   GROUP 8: NULL AND VALIDATION HANDLING
        // ============================================================

        /// <summary>
        /// Verifies null unit throws ArgumentException.
        /// Tests: Null unit validation for temperature.
        /// </summary>
        [TestMethod]
        public void testTemperatureNullUnitValidation_ThrowsArgumentException()
        {
            try
            {
                var t = new Quantity<TemperatureUnit>(100.0, null!);
                Assert.Fail("Expected ArgumentException for null unit.");
            }
            catch (ArgumentException) { /* expected */ }
        }

        /// <summary>
        /// Verifies equals(null) returns false for temperature.
        /// Tests: Null comparison returns false.
        /// </summary>
        [TestMethod]
        public void testTemperatureNullOperandValidation_InComparison_ReturnsFalse()
        {
            var t = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            Assert.IsFalse(t.Equals(null));
        }

        // ============================================================
        //   GROUP 9: TEMPERATURE UNIT STRUCTURE
        // ============================================================

        /// <summary>
        /// Verifies all three temperature constants are accessible.
        /// Tests: TemperatureUnit enum constants exist.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnit_AllConstants_Accessible()
        {
            Assert.IsNotNull(TemperatureUnit.CELSIUS);
            Assert.IsNotNull(TemperatureUnit.FAHRENHEIT);
            Assert.IsNotNull(TemperatureUnit.KELVIN);
        }

        /// <summary>
        /// Verifies GetUnitName() returns correct names for each temperature constant.
        /// Tests: Unit name method returns expected values.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnit_GetUnitName_ReturnsCorrectNames()
        {
            Assert.AreEqual("CELSIUS",    TemperatureUnit.CELSIUS.GetUnitName());
            Assert.AreEqual("FAHRENHEIT", TemperatureUnit.FAHRENHEIT.GetUnitName());
            Assert.AreEqual("KELVIN",     TemperatureUnit.KELVIN.GetUnitName());
        }

        /// <summary>
        /// Verifies temperature conversions use non-linear formulas, not simple multiplication.
        /// Tests: Non-linear conversion logic (offset formulas).
        /// </summary>
        [TestMethod]
        public void testTemperatureUnit_NonLinearConversion_NotSimpleMultiplication()
        {
            // If it were linear: 2 * C->F factor * 0°C would still be 0°F, but 0°C = 32°F
            double kelvin = TemperatureUnit.CELSIUS.ConvertToBaseUnit(0.0);
            Assert.AreEqual(273.15, kelvin, EPSILON, "0°C should convert to 273.15 K, not 0 K");
        }

        /// <summary>
        /// Verifies integration with Quantity&lt;TemperatureUnit&gt; generic class.
        /// Tests: Generic Quantity works seamlessly with TemperatureUnit.
        /// </summary>
        [TestMethod]
        public void testTemperatureIntegrationWithGenericQuantity()
        {
            var temp = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            Assert.AreEqual(100.0,              temp.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.CELSIUS, temp.Unit);
            Assert.IsTrue(temp.ToString().Contains("100"));
        }

        // ============================================================
        //   GROUP 10: BACKWARD COMPATIBILITY — UC1 THROUGH UC13
        // ============================================================

        /// <summary>
        /// Verifies all UC10-UC13 operations still work after UC14 changes.
        /// Tests: No regression from IMeasurable refactoring.
        /// </summary>
        [TestMethod]
        public void testTemperatureBackwardCompatibility_UC10_Through_UC13_Unaffected()
        {
            // UC10: Generic equality
            var f1 = new Quantity<LengthUnitM>(1.0,  LengthUnitM.FEET);
            var i12 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            Assert.IsTrue(f1.Equals(i12));

            // UC10: Generic conversion
            var converted = f1.ConvertTo(LengthUnitM.INCHES);
            Assert.AreEqual(12.0, converted.Value, EPSILON);

            // UC10/UC13: Generic addition
            var sum = f1.Add(i12);
            Assert.AreEqual(2.0, sum.Value, EPSILON);

            // UC12/UC13: Subtraction
            var f10 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var i6  = new Quantity<LengthUnitM>(6.0,  LengthUnitM.INCHES);
            var diff = f10.Subtract(i6);
            Assert.AreEqual(9.5, diff.Value, EPSILON);

            // UC12/UC13: Division
            var f2 = new Quantity<LengthUnitM>(2.0, LengthUnitM.FEET);
            Assert.AreEqual(5.0, f10.Divide(f2), EPSILON);
        }

        /// <summary>
        /// Verifies adding two temperature units in Fahrenheit throws as expected.
        /// Tests: Cross-unit arithmetic attempt still blocked.
        /// </summary>
        [TestMethod]
        public void testTemperatureCrossUnitAdditionAttempt_StillBlocked()
        {
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0,  TemperatureUnit.FAHRENHEIT);

            try
            {
                t1.Add(t2);
                Assert.Fail("Expected NotSupportedException.");
            }
            catch (NotSupportedException) { /* expected — temperature doesn't support arithmetic */ }
        }
    }
}
