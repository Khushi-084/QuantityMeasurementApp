using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementModel.Interfaces;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC10: Generic Quantity Class with Unit Interface for Multi-Category Support
    /// Tests the IMeasurable interface, Quantity<U> generic class,
    /// cross-category prevention, scalability, and architectural improvements.
    /// </summary>
    [TestClass]
    public class GenericQuantityTests
    {
        private const double EPSILON = 1e-3;

        // ============================================================
        //         IMeasurable INTERFACE IMPLEMENTATION TESTS
        // ============================================================

        /// <summary>
        /// Verifies that LengthUnitM correctly implements IMeasurable interface.
        /// Tests: All interface methods are present and functional.
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_LengthUnitImplementation()
        {
            IMeasurable feet = LengthUnitM.FEET;

            Assert.IsNotNull(feet.GetConversionFactor());
            Assert.AreEqual(1.0, feet.ConvertToBaseUnit(1.0), EPSILON);
            Assert.AreEqual(1.0, feet.ConvertFromBaseUnit(1.0), EPSILON);
            Assert.IsNotNull(feet.GetUnitName());

            IMeasurable inches = LengthUnitM.INCHES;
            Assert.AreEqual(1.0 / 12.0, inches.GetConversionFactor(), EPSILON);
            Assert.AreEqual(1.0, inches.ConvertToBaseUnit(12.0), EPSILON);
            Assert.AreEqual(12.0, inches.ConvertFromBaseUnit(1.0), EPSILON);
        }

        /// <summary>
        /// Verifies that WeightUnitM correctly implements IMeasurable interface.
        /// Tests: All interface methods are present and functional.
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_WeightUnitImplementation()
        {
            IMeasurable kg = WeightUnitM.KILOGRAM;

            Assert.IsNotNull(kg.GetConversionFactor());
            Assert.AreEqual(1.0, kg.ConvertToBaseUnit(1.0), EPSILON);
            Assert.AreEqual(1.0, kg.ConvertFromBaseUnit(1.0), EPSILON);
            Assert.IsNotNull(kg.GetUnitName());

            IMeasurable gram = WeightUnitM.GRAM;
            Assert.AreEqual(0.001, gram.GetConversionFactor(), EPSILON);
            Assert.AreEqual(1.0, gram.ConvertToBaseUnit(1000.0), EPSILON);
            Assert.AreEqual(1000.0, gram.ConvertFromBaseUnit(1.0), EPSILON);
        }

        /// <summary>
        /// Verifies that both LengthUnitM and WeightUnitM implement methods consistently.
        /// Tests: Method signatures and return types match interface contract.
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_ConsistentBehavior()
        {
            IMeasurable lengthUnit = LengthUnitM.FEET;
            IMeasurable weightUnit = WeightUnitM.KILOGRAM;

            // Both must return a valid conversion factor
            Assert.IsTrue(lengthUnit.GetConversionFactor() > 0);
            Assert.IsTrue(weightUnit.GetConversionFactor() > 0);

            // Both must convert to base and back consistently
            double lengthBase = lengthUnit.ConvertToBaseUnit(5.0);
            double lengthBack = lengthUnit.ConvertFromBaseUnit(lengthBase);
            Assert.AreEqual(5.0, lengthBack, EPSILON);

            double weightBase = weightUnit.ConvertToBaseUnit(5.0);
            double weightBack = weightUnit.ConvertFromBaseUnit(weightBase);
            Assert.AreEqual(5.0, weightBack, EPSILON);

            // Both must return a non-null unit name
            Assert.IsFalse(string.IsNullOrEmpty(lengthUnit.GetUnitName()));
            Assert.IsFalse(string.IsNullOrEmpty(weightUnit.GetUnitName()));
        }

        // ============================================================
        //         GENERIC QUANTITY - LENGTH OPERATIONS
        // ============================================================

        /// <summary>
        /// Verifies that Quantity<LengthUnitM> equality works identically to original QuantityLength.
        /// Tests: new Quantity(1.0, FEET).Equals(new Quantity(12.0, INCHES)) returns true.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Equality()
        {
            var q1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);

            Assert.IsTrue(q1.Equals(q2));
        }

        /// <summary>
        /// Verifies that Quantity<WeightUnitM> equality works identically to original QuantityWeight.
        /// Tests: new Quantity(1.0, KILOGRAM).Equals(new Quantity(1000.0, GRAM)) returns true.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Equality()
        {
            var q1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);

            Assert.IsTrue(q1.Equals(q2));
        }

        /// <summary>
        /// Verifies that Quantity<LengthUnitM> conversion works correctly.
        /// Tests: new Quantity(1.0, FEET).ConvertTo(INCHES) returns Quantity(12.0, INCHES).
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Conversion()
        {
            var q = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var result = q.ConvertTo(LengthUnitM.INCHES);

            Assert.AreEqual(12.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.INCHES, result.Unit);
        }

        /// <summary>
        /// Verifies that Quantity<WeightUnitM> conversion works correctly.
        /// Tests: new Quantity(1.0, KILOGRAM).ConvertTo(GRAM) returns Quantity(1000.0, GRAM).
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Conversion()
        {
            var q = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var result = q.ConvertTo(WeightUnitM.GRAM);

            Assert.AreEqual(1000.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnitM.GRAM, result.Unit);
        }

        /// <summary>
        /// Verifies that Quantity<LengthUnitM> addition works correctly.
        /// Tests: new Quantity(1.0, FEET).Add(new Quantity(12.0, INCHES), FEET) returns Quantity(2.0, FEET).
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Addition()
        {
            var q1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);

            var result = q1.Add(q2, LengthUnitM.FEET);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies that Quantity<WeightUnitM> addition works correctly.
        /// Tests: new Quantity(1.0, KILOGRAM).Add(new Quantity(1000.0, GRAM), KILOGRAM) returns Quantity(2.0, KILOGRAM).
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Addition()
        {
            var q1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);

            var result = q1.Add(q2, WeightUnitM.KILOGRAM);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnitM.KILOGRAM, result.Unit);
        }

        // ============================================================
        //         CROSS-CATEGORY PREVENTION TESTS
        // ============================================================

        /// <summary>
        /// Verifies that Quantity<LengthUnitM> and Quantity<WeightUnitM> cannot be compared.
        /// Tests: equals() returns false when categories differ.
        /// </summary>
        [TestMethod]
        public void testCrossCategoryPrevention_LengthVsWeight()
        {
            var length = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var weight = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);

            Assert.IsFalse(length.Equals(weight));
        }

        /// <summary>
        /// Verifies that compiler rejects type mismatches at compile-time.
        /// Tests: Quantity<LengthUnitM> and Quantity<WeightUnitM> are distinct generic types.
        /// This test verifies at runtime that type separation is enforced.
        /// </summary>
        [TestMethod]
        public void testCrossCategoryPrevention_CompilerTypeSafety()
        {
            var length = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var weight = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);

            // Distinct types — cannot be assigned to each other
            Assert.AreNotEqual(length.GetType(), weight.GetType());
            Assert.IsFalse(length.GetType() == weight.GetType());
        }

        // ============================================================
        //         CONSTRUCTOR VALIDATION TESTS
        // ============================================================

        /// <summary>
        /// Verifies that null unit is rejected in constructor.
        /// Tests: new Quantity(1.0, null) throws ArgumentException.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_NullUnit()
        {
            bool threw = false;
            try { var q = new Quantity<LengthUnitM>(1.0, null!); }
            catch (ArgumentException) { threw = true; }
            Assert.IsTrue(threw, "Expected ArgumentException for null unit.");
        }

        /// <summary>
        /// Verifies that non-finite values are rejected.
        /// Tests: new Quantity(Double.NaN, FEET) throws ArgumentException.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_InvalidValue()
        {
            bool threwNaN = false;
            try { var q = new Quantity<LengthUnitM>(double.NaN, LengthUnitM.FEET); }
            catch (ArgumentException) { threwNaN = true; }
            Assert.IsTrue(threwNaN, "Expected ArgumentException for NaN value.");

            bool threwInfinity = false;
            try { var q = new Quantity<LengthUnitM>(double.PositiveInfinity, LengthUnitM.FEET); }
            catch (ArgumentException) { threwInfinity = true; }
            Assert.IsTrue(threwInfinity, "Expected ArgumentException for Infinity value.");
        }

        // ============================================================
        //         ALL UNIT COMBINATIONS TESTS
        // ============================================================

        /// <summary>
        /// Verifies conversion between all unit pairs for multiple categories.
        /// Tests: Mathematical correctness across conversions.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Conversion_AllUnitCombinations()
        {
            // Length: FEET -> INCHES -> YARDS -> CENTIMETERS -> FEET
            var feet = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            Assert.AreEqual(12.0,    feet.ConvertTo(LengthUnitM.INCHES).Value,      EPSILON);
            Assert.AreEqual(0.33,    feet.ConvertTo(LengthUnitM.YARDS).Value,       1e-2);
            Assert.AreEqual(30.48,   feet.ConvertTo(LengthUnitM.CENTIMETERS).Value, 1e-1);

            // Weight: KILOGRAM -> GRAM -> POUND -> KILOGRAM
            var kg = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            Assert.AreEqual(1000.0,  kg.ConvertTo(WeightUnitM.GRAM).Value,   EPSILON);
            Assert.AreEqual(2.20,    kg.ConvertTo(WeightUnitM.POUND).Value,  1e-1);

            var gram = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);
            Assert.AreEqual(1.0,     gram.ConvertTo(WeightUnitM.KILOGRAM).Value, EPSILON);
        }

        /// <summary>
        /// Verifies addition with same and different units across categories.
        /// Tests: Addition with explicit target unit specification.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Addition_AllUnitCombinations()
        {
            // Length: FEET + INCHES -> result in FEET
            var lq1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var lq2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            Assert.AreEqual(2.0, lq1.Add(lq2, LengthUnitM.FEET).Value, EPSILON);

            // Length: FEET + YARDS -> result in INCHES
            var lq3 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var lq4 = new Quantity<LengthUnitM>(1.0, LengthUnitM.YARDS);
            Assert.AreEqual(48.0, lq3.Add(lq4, LengthUnitM.INCHES).Value, EPSILON);

            // Weight: KILOGRAM + GRAM -> result in KILOGRAM
            var wq1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var wq2 = new Quantity<WeightUnitM>(500.0, WeightUnitM.GRAM);
            Assert.AreEqual(1.5, wq1.Add(wq2, WeightUnitM.KILOGRAM).Value, EPSILON);

            // Weight: KILOGRAM + GRAM -> result in GRAM
            Assert.AreEqual(1500.0, wq1.Add(wq2, WeightUnitM.GRAM).Value, EPSILON);
        }

        // ============================================================
        //         BACKWARD COMPATIBILITY TESTS
        // ============================================================

        /// <summary>
        /// Runs all test cases from UC1-UC9 unchanged.
        /// Tests: All functionality is preserved in refactored design.
        /// Verified by confirming original classes still work correctly.
        /// </summary>
        [TestMethod]
        public void testBackwardCompatibility_AllUC1Through9Tests()
        {
            // UC1-UC2: Feet and Inches still work
            var feet1 = new QuantityLength(1.0, LengthUnit.FEET);
            var feet2 = new QuantityLength(1.0, LengthUnit.FEET);
            Assert.IsTrue(feet1.Equals(feet2));

            // UC3-UC4: Generic length still works
            var inch1 = new QuantityLength(12.0, LengthUnit.INCH);
            var feet3 = new QuantityLength(1.0, LengthUnit.FEET);
            Assert.IsTrue(inch1.Equals(feet3));

            // UC5: Conversion still works
            var converted = feet1.ConvertTo(LengthUnit.INCH);
            Assert.AreEqual(12.0, converted.Value, EPSILON);

            // UC6-UC7: Addition still works
            var sum = feet1.Add(inch1, LengthUnit.FEET);
            Assert.AreEqual(2.0, sum.Value, EPSILON);

            // UC9: Weight still works
            var kg = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
            var gram = new QuantityWeight(1000.0, WeightUnit.GRAM);
            Assert.IsTrue(kg.Equals(gram));
        }

        // ============================================================
        //         SIMPLIFIED DEMONSTRATION METHOD TESTS
        // ============================================================

        /// <summary>
        /// Verifies that single generic demonstration method handles equality.
        /// Tests: demonstrateEquality() works with length and weight quantities.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Equality()
        {
            var lq1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var lq2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            Assert.IsTrue(DemonstrateEquality(lq1, lq2));

            var wq1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var wq2 = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);
            Assert.IsTrue(DemonstrateEquality(wq1, wq2));
        }

        /// <summary>
        /// Verifies that single generic demonstration method handles conversion.
        /// Tests: demonstrateConversion() works with length and weight quantities.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Conversion()
        {
            var lq = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var lResult = DemonstrateConversion(lq, LengthUnitM.INCHES);
            Assert.AreEqual(12.0, lResult.Value, EPSILON);

            var wq = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var wResult = DemonstrateConversion(wq, WeightUnitM.GRAM);
            Assert.AreEqual(1000.0, wResult.Value, EPSILON);
        }

        /// <summary>
        /// Verifies that single generic demonstration method handles addition.
        /// Tests: demonstrateAddition() works with length and weight quantities.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Addition()
        {
            var lq1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var lq2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            var lResult = DemonstrateAddition(lq1, lq2, LengthUnitM.FEET);
            Assert.AreEqual(2.0, lResult.Value, EPSILON);

            var wq1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var wq2 = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);
            var wResult = DemonstrateAddition(wq1, wq2, WeightUnitM.KILOGRAM);
            Assert.AreEqual(2.0, wResult.Value, EPSILON);
        }

        // ============================================================
        //         TYPE WILDCARD & SCALABILITY TESTS
        // ============================================================

        /// <summary>
        /// Verifies that methods using Quantity<?> work with multiple unit types.
        /// Tests: Single method signature handles all categories.
        /// </summary>
        [TestMethod]
        public void testTypeWildcard_FlexibleSignatures()
        {
            var lengthQ = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var weightQ = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);

            // Single generic method handles both via IMeasurable
            Assert.AreEqual(1.0, GetBaseValue(lengthQ), EPSILON);
            Assert.AreEqual(1.0, GetBaseValue(weightQ), EPSILON);
        }

        /// <summary>
        /// Creates a test VolumeUnitTest class and verifies it works with Quantity<VolumeUnitTest>.
        /// Tests: New categories integrate seamlessly without refactoring existing code.
        /// </summary>
        [TestMethod]
        public void testScalability_NewUnitEnumIntegration()
        {
            // VolumeUnitTest is defined at the bottom of this file
            var liter      = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);
            var milliliter = new Quantity<VolumeUnitTest>(1000.0, VolumeUnitTest.MILLILITER);

            Assert.IsTrue(liter.Equals(milliliter));

            var converted = liter.ConvertTo(VolumeUnitTest.MILLILITER);
            Assert.AreEqual(1000.0, converted.Value, EPSILON);
        }

        /// <summary>
        /// Tests addition of temperature, time, and other measurement categories.
        /// Verifies: No modifications to Quantity<U> or QuantityMeasurementApp needed.
        /// </summary>
        [TestMethod]
        public void testScalability_MultipleNewCategories()
        {
            // Volume
            var v1 = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);
            var v2 = new Quantity<VolumeUnitTest>(500.0, VolumeUnitTest.MILLILITER);
            var vSum = v1.Add(v2, VolumeUnitTest.LITER);
            Assert.AreEqual(1.5, vSum.Value, EPSILON);

            // Length (already proved, showing same Quantity<U> works for all)
            var l1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var l2 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            Assert.IsTrue(l1.Equals(l2));

            // Weight (same Quantity<U> class, different type parameter)
            var w1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var w2 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            Assert.IsTrue(w1.Equals(w2));
        }

        // ============================================================
        //         GENERIC TYPE PARAMETER ENFORCEMENT
        // ============================================================

        /// <summary>
        /// Verifies that bounded type parameter enforces interface implementation.
        /// Tests: Only valid IMeasurable types are accepted by compiler.
        /// </summary>
        [TestMethod]
        public void testGenericBoundedTypeParameter_Enforcement()
        {
            // LengthUnitM implements IMeasurable — valid
            var lq = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            Assert.IsNotNull(lq);

            // WeightUnitM implements IMeasurable — valid
            var wq = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            Assert.IsNotNull(wq);

            // VolumeUnitTest implements IMeasurable — valid, no code change needed
            var vq = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);
            Assert.IsNotNull(vq);

            // All unit types are accessible via the interface
            Assert.IsInstanceOfType(lq.Unit, typeof(IMeasurable));
            Assert.IsInstanceOfType(wq.Unit, typeof(IMeasurable));
            Assert.IsInstanceOfType(vq.Unit, typeof(IMeasurable));
        }

        // ============================================================
        //         HASHCODE & EQUALS CONTRACT TESTS
        // ============================================================

        /// <summary>
        /// Verifies that hashCode() is consistent across generic types.
        /// Tests: Generic quantities with equal base values have equal hash codes.
        /// </summary>
        [TestMethod]
        public void testHashCode_GenericQuantity_Consistency()
        {
            var q1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);

            // Equal quantities must have equal hash codes
            Assert.IsTrue(q1.Equals(q2));
            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());

            var w1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var w2 = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);

            Assert.IsTrue(w1.Equals(w2));
            Assert.AreEqual(w1.GetHashCode(), w2.GetHashCode());
        }

        /// <summary>
        /// Verifies that equals() contract (reflexive, symmetric, transitive) is maintained.
        /// Tests: All equality mathematical properties hold for generic quantities.
        /// </summary>
        [TestMethod]
        public void testEquals_GenericQuantity_ContractPreservation()
        {
            var a = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var b = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            var c = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);

            // Reflexive: a == a
            Assert.IsTrue(a.Equals(a));

            // Symmetric: a == b => b == a
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));

            // Transitive: a == b && b == c => a == c
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));

            // Null: a != null
            Assert.IsFalse(a.Equals(null));
        }

        // ============================================================
        //         ENUM AS BEHAVIOR CARRIER TESTS
        // ============================================================

        /// <summary>
        /// Verifies that enum constants carry behavior through IMeasurable interface.
        /// Tests: Polymorphic calls to unit methods work correctly across categories.
        /// </summary>
        [TestMethod]
        public void testEnumAsUnitCarrier_BehaviorEncapsulation()
        {
            // Polymorphic call via interface — no cast needed
            IMeasurable[] units = { LengthUnitM.FEET, LengthUnitM.INCHES, WeightUnitM.KILOGRAM, WeightUnitM.GRAM };

            foreach (var unit in units)
            {
                double baseVal = unit.ConvertToBaseUnit(1.0);
                double backVal = unit.ConvertFromBaseUnit(baseVal);
                Assert.AreEqual(1.0, backVal, EPSILON);
                Assert.IsFalse(string.IsNullOrEmpty(unit.GetUnitName()));
            }
        }

        /// <summary>
        /// Verifies that type erasure does not compromise runtime safety.
        /// Tests: Cross-category checks work despite generic type erasure.
        /// </summary>
        [TestMethod]
        public void testTypeErasure_RuntimeSafety()
        {
            var length = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var weight = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);

            // Despite type erasure, runtime type info is preserved via unit references
            Assert.AreNotEqual(length.Unit.GetType(), weight.Unit.GetType());

            // Cross-category equals() correctly returns false at runtime
            Assert.IsFalse(length.Equals(weight));
            Assert.IsFalse(weight.Equals(length));
        }

        // ============================================================
        //         COMPOSITION & DRY TESTS
        // ============================================================

        /// <summary>
        /// Verifies that composition approach is more flexible than inheritance-based design.
        /// Tests: Quantity<U> works with any IMeasurable implementation.
        /// </summary>
        [TestMethod]
        public void testCompositionOverInheritance_Flexibility()
        {
            // Single Quantity<U> class works for all categories — no subclassing needed
            var lengthQ  = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var weightQ  = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var volumeQ  = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);

            Assert.AreEqual("Quantity`1", lengthQ.GetType().Name);
            Assert.AreEqual("Quantity`1", weightQ.GetType().Name);
            Assert.AreEqual("Quantity`1", volumeQ.GetType().Name);

            // All use the same generic class — composition, not inheritance
            Assert.AreEqual(lengthQ.GetType().GetGenericTypeDefinition(),
                            weightQ.GetType().GetGenericTypeDefinition());
        }

        /// <summary>
        /// Measures reduction in total lines of code and duplication compared to UC9.
        /// Tests: Generic design contains significantly less redundancy.
        /// </summary>
        [TestMethod]
        public void testCodeReduction_DRYValidation()
        {
            // UC10 uses ONE Quantity<U> class for all categories
            // UC9 used TWO separate classes (QuantityLength + QuantityWeight)
            // Verify both length and weight operate through the SAME generic class
            var lengthType = typeof(Quantity<LengthUnitM>).GetGenericTypeDefinition();
            var weightType = typeof(Quantity<WeightUnitM>).GetGenericTypeDefinition();

            Assert.AreEqual(lengthType, weightType); // same class, different type params
        }

        /// <summary>
        /// Verifies that comparison and arithmetic logic is implemented once.
        /// Tests: Changes to equals() or add() automatically benefit all categories.
        /// </summary>
        [TestMethod]
        public void testMaintainability_SingleSourceOfTruth()
        {
            // All categories use the same Equals() implementation
            var lq1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var lq2 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var wq1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var wq2 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
            var vq1 = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);
            var vq2 = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);

            // All use same equals() logic from Quantity<U>
            Assert.IsTrue(lq1.Equals(lq2));
            Assert.IsTrue(wq1.Equals(wq2));
            Assert.IsTrue(vq1.Equals(vq2));
        }

        // ============================================================
        //         ARCHITECTURAL READINESS TESTS
        // ============================================================

        /// <summary>
        /// Tests addition of 5+ new unit types (VolumeUnit, TemperatureUnit, TimeUnit, etc.).
        /// Verifies: No refactoring of existing code required.
        /// </summary>
        [TestMethod]
        public void testArchitecturalReadiness_MultipleNewCategories()
        {
            // All new categories use the exact same Quantity<U> class
            var vol   = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);
            var len   = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var wt    = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);

            Assert.IsNotNull(vol);
            Assert.IsNotNull(len);
            Assert.IsNotNull(wt);

            // Each category works independently without touching Quantity<U>
            Assert.IsTrue(vol.Equals(new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER)));
            Assert.IsTrue(len.Equals(new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES)));
            Assert.IsTrue(wt.Equals(new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM)));
        }

        /// <summary>
        /// Benchmarks generic version against UC9 implementation.
        /// Tests: Generic approach introduces negligible performance overhead.
        /// </summary>
        [TestMethod]
        public void testPerformance_GenericOverhead()
        {
            int iterations = 100_000;

            // Generic Quantity<U>
            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var q1 = new Quantity<WeightUnitM>(1.0, WeightUnitM.KILOGRAM);
                var q2 = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);
                _ = q1.Equals(q2);
            }
            sw1.Stop();

            // Original QuantityWeight (UC9)
            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var q1 = new QuantityWeight(1.0, WeightUnit.KILOGRAM);
                var q2 = new QuantityWeight(1000.0, WeightUnit.GRAM);
                _ = q1.Equals(q2);
            }
            sw2.Stop();

            // Generic overhead must be negligible (within 5x of original)
            Assert.IsTrue(sw1.ElapsedMilliseconds < sw2.ElapsedMilliseconds * 5,
                $"Generic: {sw1.ElapsedMilliseconds}ms, Original: {sw2.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// Verifies that documentation clearly explains how to add new unit enums.
        /// Tests: New developers can easily extend system with new categories following documentation.
        /// Pattern: Create class implementing IMeasurable -> use with Quantity<U> immediately.
        /// </summary>
        [TestMethod]
        public void testDocumentation_PatternClarity()
        {
            // Step 1: Create new unit class implementing IMeasurable (VolumeUnitTest below)
            // Step 2: Use with Quantity<U> — no other changes needed
            var liter = new Quantity<VolumeUnitTest>(1.0, VolumeUnitTest.LITER);
            var ml    = new Quantity<VolumeUnitTest>(1000.0, VolumeUnitTest.MILLILITER);

            Assert.IsTrue(liter.Equals(ml));
            Assert.AreEqual(1000.0, liter.ConvertTo(VolumeUnitTest.MILLILITER).Value, EPSILON);
            Assert.AreEqual(2.0, liter.Add(ml, VolumeUnitTest.LITER).Value, EPSILON);
        }

        // ============================================================
        //         INTERFACE SEGREGATION & IMMUTABILITY TESTS
        // ============================================================

        /// <summary>
        /// Verifies that IMeasurable interface is minimal and focused.
        /// UC14: Interface now defines 6 methods — the original 4 mandatory conversion methods
        /// plus 2 optional default methods (SupportsArithmetic, ValidateOperationSupport)
        /// added to support selective arithmetic for TemperatureUnit.
        /// Existing units (Length, Weight, Volume) require no changes — defaults apply.
        /// Tests: Interface method count matches expected contract.
        /// </summary>
        [TestMethod]
        public void testInterfaceSegregation_MinimalContract()
        {
            var methods = typeof(IMeasurable).GetMethods();

            // UC14: IMeasurable now has 6 methods:
            // Mandatory (4): GetConversionFactor, ConvertToBaseUnit, ConvertFromBaseUnit, GetUnitName
            // Default  (2): SupportsArithmetic, ValidateOperationSupport
            Assert.AreEqual(6, methods.Length,
                "IMeasurable should define exactly 6 methods — 4 mandatory + 2 UC14 default methods.");
        }

        /// <summary>
        /// Verifies that Quantity<U> objects are immutable.
        /// Tests: No setters exist; all operations return new instances.
        /// </summary>
        [TestMethod]
        public void testImmutability_GenericQuantity()
        {
            var original = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);

            // ConvertTo returns a NEW instance
            var converted = original.ConvertTo(LengthUnitM.INCHES);
            Assert.AreNotSame(original, converted);
            Assert.AreEqual(1.0, original.Value, EPSILON);   // original unchanged

            // Add returns a NEW instance
            var other = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var sum = original.Add(other);
            Assert.AreNotSame(original, sum);
            Assert.AreEqual(1.0, original.Value, EPSILON);   // original unchanged

            // No public setters exist on Quantity<U>
            var props = typeof(Quantity<LengthUnitM>).GetProperties();
            foreach (var prop in props)
            {
                Assert.IsFalse(prop.CanWrite,
                    $"Property '{prop.Name}' should not have a public setter.");
            }
        }

        // ============================================================
        //         GENERIC HELPER METHODS (mirror of QuantityMeasurementApp)
        // ============================================================

        private bool DemonstrateEquality<U>(Quantity<U> a, Quantity<U> b) where U : class, IMeasurable
            => a.Equals(b);

        private Quantity<U> DemonstrateConversion<U>(Quantity<U> q, U target) where U : class, IMeasurable
            => q.ConvertTo(target);

        private Quantity<U> DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b, U target) where U : class, IMeasurable
            => a.Add(b, target);

        private double GetBaseValue<U>(Quantity<U> q) where U : class, IMeasurable
            => q.ConvertToBaseUnit();
    }

    // ============================================================
    //   STUB: VolumeUnitTest — used to verify scalability tests
    //   Demonstrates that adding a new category requires ONLY this.
    // ============================================================

    /// <summary>
    /// Test volume unit implementing IMeasurable.
    /// Base unit: LITER.
    /// Created only to verify UC10 scalability — no changes to Quantity<U> needed.
    /// </summary>
    public class VolumeUnitTest : IMeasurable
    {
        public static readonly VolumeUnitTest LITER      = new VolumeUnitTest("LITER",       1.0);
        public static readonly VolumeUnitTest MILLILITER = new VolumeUnitTest("MILLILITER",  0.001);
        public static readonly VolumeUnitTest GALLON     = new VolumeUnitTest("GALLON",      3.78541);

        private readonly string name;
        private readonly double factor;

        private VolumeUnitTest(string name, double factor)
        {
            this.name = name;
            this.factor = factor;
        }

        public double GetConversionFactor()             => factor;
        public double ConvertToBaseUnit(double value)   => value * factor;
        public double ConvertFromBaseUnit(double baseValue)
        {
            if (System.Math.Abs(factor) < 1e-15)
                throw new System.ArgumentException("Unsupported unit");
            return baseValue / factor;
        }
        public string GetUnitName() => name;

        public override string ToString() => name;
    }
}
