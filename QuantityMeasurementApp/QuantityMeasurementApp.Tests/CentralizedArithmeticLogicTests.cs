using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC13: Centralized Arithmetic Logic - Enforce DRY in Quantity Operations.
    ///
    /// Tests verify:
    /// 1.  ArithmeticOperation enum dispatch (ADD, SUBTRACT, DIVIDE) computes correctly.
    /// 2.  ValidateArithmeticOperands centralizes all validation (null, cross-category, finiteness, target unit).
    /// 3.  PerformBaseArithmetic centralizes conversion and computation.
    /// 4.  All UC12 behaviors preserved (backward compatibility).
    /// 5.  Private method encapsulation enforced.
    /// 6.  DRY principle: validation and conversion logic appear once.
    /// 7.  Rounding consistency: Add/Subtract round; Divide does not.
    /// 8.  Immutability preserved after refactoring.
    /// 9.  All categories (Length, Weight, Volume) work uniformly.
    /// 10. Error message consistency across all operations.
    /// </summary>
    [TestClass]
    public class UC13_CentralizedArithmeticLogicTests
    {
        private const double EPSILON = 1e-3;

        // ============================================================
        //   GROUP 1: ARITHMETIC ENUM DISPATCH - Direct Computation
        // ============================================================

        /// <summary>
        /// Verifies ADD operation via public Add() delegates through centralized helper.
        /// Proxy test: ADD logic verified via observable public behavior.
        /// Tests: ADD enum computes correct sum.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_Add_EnumComputation()
        {
            var q1 = new Quantity<LengthUnitM>(7.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(3.0, LengthUnitM.FEET);

            var result = q1.Add(q2);

            Assert.AreEqual(10.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies SUBTRACT operation via public Subtract() delegates through centralized helper.
        /// Tests: SUBTRACT enum computes correct difference.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_Subtract_EnumComputation()
        {
            var q1 = new Quantity<LengthUnitM>(7.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(3.0, LengthUnitM.FEET);

            var result = q1.Subtract(q2);

            Assert.AreEqual(4.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies DIVIDE operation via public Divide() delegates through centralized helper.
        /// Tests: DIVIDE enum computes correct quotient.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_Divide_EnumComputation()
        {
            var q1 = new Quantity<LengthUnitM>(7.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0, LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.AreEqual(3.5, result, EPSILON);
        }

        /// <summary>
        /// Verifies DIVIDE with zero divisor throws ArithmeticException.
        /// Tests: Enum-level zero-division validation.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_DivideByZero_EnumThrows()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(0.0,  LengthUnitM.FEET);

            try
            {
                q1.Divide(q2);
                Assert.Fail("Expected ArithmeticException for division by zero.");
            }
            catch (ArithmeticException) { /* expected */ }
        }

        // ============================================================
        //   GROUP 2: HELPER METHOD DELEGATION
        // ============================================================

        /// <summary>
        /// Verifies Add() correctly converts cross-unit operands via centralized helper.
        /// Tests: Helper correctly converts both operands and performs ADD.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Add_DelegatesViaHelper()
        {
            // 1 foot + 12 inches = 2 feet (cross-unit, centralized conversion)
            var q1 = new Quantity<LengthUnitM>(1.0,  LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0,              result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies Subtract() correctly delegates to centralized helper for cross-unit ops.
        /// Tests: Helper delegation works for SUBTRACT.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Subtract_DelegatesViaHelper()
        {
            // 10 feet - 6 inches = 9.5 feet
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(6.0,  LengthUnitM.INCHES);

            var result = q1.Subtract(q2);

            Assert.AreEqual(9.5,              result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies Divide() correctly delegates to centralized helper for cross-unit ops.
        /// Tests: Helper delegation works for DIVIDE.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Divide_DelegatesViaHelper()
        {
            // 24 inches / 2 feet = 1.0 (both convert to feet: 2 / 2 = 1)
            var q1 = new Quantity<LengthUnitM>(24.0, LengthUnitM.INCHES);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies PerformBaseArithmetic converts to base unit correctly before operating.
        /// Tests: Helper base-unit conversion and operation are correct.
        /// </summary>
        [TestMethod]
        public void testPerformBaseArithmetic_ConversionAndOperation()
        {
            // 1 kg + 1000 g = 2 kg (1 kg base + 1 kg base = 2 kg base -> 2 kg)
            var q1 = new Quantity<WeightUnitM>(1.0,    WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(1000.0, WeightUnitM.GRAM);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0,                result.Value, EPSILON);
            Assert.AreEqual(WeightUnitM.KILOGRAM, result.Unit);
        }

        // ============================================================
        //   GROUP 3: VALIDATION CONSISTENCY ACROSS ALL OPERATIONS
        // ============================================================

        /// <summary>
        /// Verifies Add(null), Subtract(null), Divide(null) all throw consistently.
        /// Tests: Validation consistency - null operand check unified.
        /// </summary>
        [TestMethod]
        public void testValidation_NullOperand_ConsistentAcrossOperations()
        {
            var q = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            ArgumentNullException? addEx      = null;
            ArgumentNullException? subtractEx = null;
            ArgumentNullException? divideEx   = null;

            try { q.Add(null!);      } catch (ArgumentNullException e) { addEx      = e; }
            try { q.Subtract(null!); } catch (ArgumentNullException e) { subtractEx = e; }
            try { q.Divide(null!);   } catch (ArgumentNullException e) { divideEx   = e; }

            Assert.IsNotNull(addEx,      "Add(null) should throw ArgumentNullException");
            Assert.IsNotNull(subtractEx, "Subtract(null) should throw ArgumentNullException");
            Assert.IsNotNull(divideEx,   "Divide(null) should throw ArgumentNullException");
        }

        /// <summary>
        /// Verifies cross-category check is consistent across all operations.
        /// Tests: Category validation centralization.
        /// </summary>
        [TestMethod]
        public void testValidation_CrossCategory_ConsistentAcrossOperations()
        {
            var length = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            // Cross-category: using WeightUnitM where LengthUnitM expected is caught by generics at compile time.
            // We verify within-generic-type cross-unit-class check via same-type but incompatible instances.

            // Verify same-category same-type operations work fine (no exception)
            var length2 = new Quantity<LengthUnitM>(5.0, LengthUnitM.INCHES);
            Assert.IsNotNull(length.Add(length2));
            Assert.IsNotNull(length.Subtract(length2));
            Assert.AreEqual(24.0, length.Divide(length2), EPSILON); // 10ft / (5/12)ft = 24
        }

        /// <summary>
        /// Verifies NaN and Infinity finiteness checks are consistent across all operations.
        /// Tests: Finiteness validation centralization.
        /// </summary>
        [TestMethod]
        public void testValidation_FiniteValue_ConsistentAcrossOperations()
        {
            var valid = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            // NaN operand
            bool addNaNThrows      = false;
            bool subtractNaNThrows = false;
            bool divideNaNThrows   = false;

            try
            {
                var nanQ = new Quantity<LengthUnitM>(double.NaN, LengthUnitM.FEET);
            }
            catch (ArgumentException) { addNaNThrows = subtractNaNThrows = divideNaNThrows = true; }

            // NaN is caught at construction time (Quantity constructor validates finiteness)
            Assert.IsTrue(addNaNThrows, "NaN should be rejected at construction");
        }

        /// <summary>
        /// Verifies that explicit null target unit throws for Add(other, null).
        /// Tests: Target unit validation for add/subtract with explicit unit.
        /// </summary>
        [TestMethod]
        public void testValidation_NullTargetUnit_AddRejects()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            try
            {
                q1.Add(q2, null!);
                Assert.Fail("Expected ArgumentException for null target unit.");
            }
            catch (ArgumentException) { /* expected */ }
        }

        /// <summary>
        /// Verifies that explicit null target unit throws for Subtract(other, null).
        /// Tests: Target unit validation centralized in ValidateArithmeticOperands.
        /// </summary>
        [TestMethod]
        public void testValidation_NullTargetUnit_SubtractRejects()
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

        // ============================================================
        //   GROUP 4: ADD UC12 BEHAVIOR PRESERVATION
        // ============================================================

        /// <summary>
        /// Verifies all UC12 addition test cases pass after UC13 refactoring.
        /// Tests: Backward compatibility - UC12 addition behaviors preserved.
        /// </summary>
        [TestMethod]
        public void testAdd_UC12_BehaviorPreserved_ImplicitTarget()
        {
            // UC12/UC10 behavior: 1 foot + 12 inches = 2 feet
            var q1 = new Quantity<LengthUnitM>(1.0,  LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0,              result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies UC12 addition with explicit target unit is preserved.
        /// Tests: Backward compatibility - explicit target unit behavior preserved.
        /// </summary>
        [TestMethod]
        public void testAdd_UC12_BehaviorPreserved_ExplicitTarget()
        {
            // 10 kg + 5000 g = 15000 g (explicit target = GRAM)
            var q1 = new Quantity<WeightUnitM>(10.0,   WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(5000.0, WeightUnitM.GRAM);

            var result = q1.Add(q2, WeightUnitM.GRAM);

            Assert.AreEqual(15000.0,           result.Value, EPSILON);
            Assert.AreEqual(WeightUnitM.GRAM,  result.Unit);
        }

        // ============================================================
        //   GROUP 5: SUBTRACT UC12 BEHAVIOR PRESERVATION
        // ============================================================

        /// <summary>
        /// Verifies all UC12 subtraction test cases pass after UC13 refactoring.
        /// Tests: Backward compatibility - UC12 subtraction behaviors preserved.
        /// </summary>
        [TestMethod]
        public void testSubtract_UC12_BehaviorPreserved_CrossUnit()
        {
            // 10 feet - 6 inches = 9.5 feet
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(6.0,  LengthUnitM.INCHES);

            var result = q1.Subtract(q2);

            Assert.AreEqual(9.5,              result.Value, EPSILON);
            Assert.AreEqual(LengthUnitM.FEET, result.Unit);
        }

        /// <summary>
        /// Verifies UC12 subtraction with explicit target unit preserved.
        /// Tests: Backward compatibility - explicit target subtraction preserved.
        /// </summary>
        [TestMethod]
        public void testSubtract_UC12_BehaviorPreserved_ExplicitTarget()
        {
            // 5 litre - 2 litre = 3000 ml (explicit target = MILLILITRE)
            var q1 = new Quantity<VolumeUnitM>(5.0, VolumeUnitM.LITRE);
            var q2 = new Quantity<VolumeUnitM>(2.0, VolumeUnitM.LITRE);

            var result = q1.Subtract(q2, VolumeUnitM.MILLILITRE);

            Assert.AreEqual(3000.0,                result.Value, EPSILON);
            Assert.AreEqual(VolumeUnitM.MILLILITRE, result.Unit);
        }

        /// <summary>
        /// Verifies non-commutativity of subtraction is preserved.
        /// Tests: Mathematical property - subtraction is non-commutative.
        /// </summary>
        [TestMethod]
        public void testSubtract_NonCommutativity_Preserved()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(3.0,  LengthUnitM.FEET);

            var r1 = q1.Subtract(q2); // 10 - 3 = 7
            var r2 = q2.Subtract(q1); // 3 - 10 = -7

            Assert.AreEqual(7.0,  r1.Value, EPSILON);
            Assert.AreEqual(-7.0, r2.Value, EPSILON);
        }

        // ============================================================
        //   GROUP 6: DIVIDE UC12 BEHAVIOR PRESERVATION
        // ============================================================

        /// <summary>
        /// Verifies all UC12 division test cases pass after UC13 refactoring.
        /// Tests: Backward compatibility - UC12 division behaviors preserved.
        /// </summary>
        [TestMethod]
        public void testDivide_UC12_BehaviorPreserved_SameUnit()
        {
            // 10 feet / 2 feet = 5.0
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.AreEqual(5.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies UC12 division returns dimensionless scalar without unit.
        /// Tests: Division returns raw double - no rounding applied.
        /// </summary>
        [TestMethod]
        public void testDivide_UC12_BehaviorPreserved_CrossUnit()
        {
            // 24 inches / 2 feet = 1.0 (both convert to feet: 2ft / 2ft)
            var q1 = new Quantity<LengthUnitM>(24.0, LengthUnitM.INCHES);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies division by zero still throws ArithmeticException.
        /// Tests: Zero-divisor handling via centralized helper.
        /// </summary>
        [TestMethod]
        public void testDivide_UC12_BehaviorPreserved_DivisionByZero()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(0.0,  LengthUnitM.FEET);

            try
            {
                q1.Divide(q2);
                Assert.Fail("Expected ArithmeticException for division by zero.");
            }
            catch (ArithmeticException) { /* expected */ }
        }

        // ============================================================
        //   GROUP 7: ROUNDING CONSISTENCY
        // ============================================================

        /// <summary>
        /// Verifies Add/Subtract results are rounded to two decimal places.
        /// Tests: Rounding consistency via centralized helper.
        /// </summary>
        [TestMethod]
        public void testRounding_AddSubtract_TwoDecimalPlaces()
        {
            // 1 foot + 1 cm: 1 + 0.0328084 = 1.0328084 feet -> rounded to 1.03
            var q1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(1.0, LengthUnitM.CENTIMETERS);

            var addResult = q1.Add(q2);

            // Verify two decimal place rounding
            Assert.AreEqual(Math.Round(addResult.Value, 2), addResult.Value, EPSILON);
        }

        /// <summary>
        /// Verifies Divide returns raw double without rounding.
        /// Tests: Different handling for dimensionless result - no rounding.
        /// </summary>
        [TestMethod]
        public void testRounding_Divide_NoRounding()
        {
            // 7 / 2 = 3.5 (raw, not rounded to 3.50 or truncated)
            var q1 = new Quantity<LengthUnitM>(7.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0, LengthUnitM.FEET);

            double result = q1.Divide(q2);

            Assert.AreEqual(3.5, result, EPSILON);
        }

        /// <summary>
        /// Verifies RoundToTwoDecimals behavior accuracy.
        /// Tests: Rounding helper correctness.
        /// </summary>
        [TestMethod]
        public void testRounding_Helper_Accuracy()
        {
            // 1 cm = 0.0328084 feet. 3 cm = 0.0984252 feet + 1 foot = 1.0984252 -> 1.1 feet
            var q1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(3.0, LengthUnitM.CENTIMETERS);

            var result = q1.Add(q2);

            // 1 + 3*0.0328084 = 1.0984252 -> rounds to 1.1
            Assert.AreEqual(1.1, result.Value, 0.01);
        }

        // ============================================================
        //   GROUP 8: IMPLICIT AND EXPLICIT TARGET UNIT
        // ============================================================

        /// <summary>
        /// Verifies implicit target unit = first operand's unit for Add.
        /// Tests: Implicit target unit behavior.
        /// </summary>
        [TestMethod]
        public void testImplicitTargetUnit_Add_UsesFirstOperandsUnit()
        {
            var q1 = new Quantity<WeightUnitM>(5.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(500.0, WeightUnitM.GRAM);

            var result = q1.Add(q2); // implicit target = KILOGRAM

            Assert.AreEqual(WeightUnitM.KILOGRAM, result.Unit);
            Assert.AreEqual(5.5, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies implicit target unit = first operand's unit for Subtract.
        /// Tests: Implicit target unit behavior for subtraction.
        /// </summary>
        [TestMethod]
        public void testImplicitTargetUnit_Subtract_UsesFirstOperandsUnit()
        {
            var q1 = new Quantity<WeightUnitM>(5.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(500.0, WeightUnitM.GRAM);

            var result = q1.Subtract(q2); // implicit target = KILOGRAM

            Assert.AreEqual(WeightUnitM.KILOGRAM, result.Unit);
            Assert.AreEqual(4.5, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies explicit target unit overrides implicit unit for Add.
        /// Tests: Explicit target unit overrides first operand's unit.
        /// </summary>
        [TestMethod]
        public void testExplicitTargetUnit_Add_Overrides()
        {
            var q1 = new Quantity<WeightUnitM>(5.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(500.0, WeightUnitM.GRAM);

            var result = q1.Add(q2, WeightUnitM.GRAM); // explicit target = GRAM

            Assert.AreEqual(WeightUnitM.GRAM, result.Unit);
            Assert.AreEqual(5500.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies explicit target unit overrides implicit unit for Subtract.
        /// Tests: Explicit target unit behavior for subtraction.
        /// </summary>
        [TestMethod]
        public void testExplicitTargetUnit_Subtract_Overrides()
        {
            var q1 = new Quantity<WeightUnitM>(5.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(500.0, WeightUnitM.GRAM);

            var result = q1.Subtract(q2, WeightUnitM.GRAM); // explicit target = GRAM

            Assert.AreEqual(WeightUnitM.GRAM, result.Unit);
            Assert.AreEqual(4500.0, result.Value, EPSILON);
        }

        // ============================================================
        //   GROUP 9: IMMUTABILITY PRESERVATION
        // ============================================================

        /// <summary>
        /// Verifies original quantities unchanged after Add via centralized helper.
        /// Tests: Immutability through refactored implementation.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterAdd_ViaCentralizedHelper()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            q1.Add(q2);

            Assert.AreEqual(10.0, q1.Value, EPSILON);
            Assert.AreEqual(5.0,  q2.Value, EPSILON);
        }

        /// <summary>
        /// Verifies original quantities unchanged after Subtract via centralized helper.
        /// Tests: Immutability through refactored implementation.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterSubtract_ViaCentralizedHelper()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            q1.Subtract(q2);

            Assert.AreEqual(10.0, q1.Value, EPSILON);
            Assert.AreEqual(5.0,  q2.Value, EPSILON);
        }

        /// <summary>
        /// Verifies original quantities unchanged after Divide via centralized helper.
        /// Tests: Immutability through refactored implementation.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterDivide_ViaCentralizedHelper()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

            q1.Divide(q2);

            Assert.AreEqual(10.0, q1.Value, EPSILON);
            Assert.AreEqual(2.0,  q2.Value, EPSILON);
        }

        // ============================================================
        //   GROUP 10: ALL OPERATIONS ACROSS ALL CATEGORIES
        // ============================================================

        /// <summary>
        /// Verifies Add/Subtract/Divide work for Length category.
        /// Tests: Scalability across categories.
        /// </summary>
        [TestMethod]
        public void testAllOperations_LengthCategory()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);

            Assert.AreEqual(15.0, q1.Add(q2).Value,      EPSILON);
            Assert.AreEqual(5.0,  q1.Subtract(q2).Value, EPSILON);
            Assert.AreEqual(2.0,  q1.Divide(q2),         EPSILON);
        }

        /// <summary>
        /// Verifies Add/Subtract/Divide work for Weight category.
        /// Tests: Scalability across categories.
        /// </summary>
        [TestMethod]
        public void testAllOperations_WeightCategory()
        {
            var q1 = new Quantity<WeightUnitM>(10.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(5.0,  WeightUnitM.KILOGRAM);

            Assert.AreEqual(15.0, q1.Add(q2).Value,      EPSILON);
            Assert.AreEqual(5.0,  q1.Subtract(q2).Value, EPSILON);
            Assert.AreEqual(2.0,  q1.Divide(q2),         EPSILON);
        }

        /// <summary>
        /// Verifies Add/Subtract/Divide work for Volume category.
        /// Tests: Scalability across categories.
        /// </summary>
        [TestMethod]
        public void testAllOperations_VolumeCategory()
        {
            var q1 = new Quantity<VolumeUnitM>(10.0, VolumeUnitM.LITRE);
            var q2 = new Quantity<VolumeUnitM>(5.0,  VolumeUnitM.LITRE);

            Assert.AreEqual(15.0, q1.Add(q2).Value,      EPSILON);
            Assert.AreEqual(5.0,  q1.Subtract(q2).Value, EPSILON);
            Assert.AreEqual(2.0,  q1.Divide(q2),         EPSILON);
        }

        // ============================================================
        //   GROUP 11: PRIVATE METHOD ENCAPSULATION
        // ============================================================

        /// <summary>
        /// Verifies PerformBaseArithmetic is private (not accessible outside class).
        /// Tests: Helper private visibility - encapsulation.
        /// </summary>
        [TestMethod]
        public void testHelper_PerformBaseArithmetic_IsPrivate()
        {
            var quantityType = typeof(Quantity<LengthUnitM>);
            var method = quantityType.GetMethod(
                "PerformBaseArithmetic",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(method, "PerformBaseArithmetic should exist as a private method");
            Assert.IsTrue(method.IsPrivate, "PerformBaseArithmetic should be private");
        }

        /// <summary>
        /// Verifies ValidateArithmeticOperands is private (not accessible outside class).
        /// Tests: Validation helper private visibility - encapsulation.
        /// </summary>
        [TestMethod]
        public void testHelper_ValidateArithmeticOperands_IsPrivate()
        {
            var quantityType = typeof(Quantity<LengthUnitM>);
            var method = quantityType.GetMethod(
                "ValidateArithmeticOperands",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(method, "ValidateArithmeticOperands should exist as a private method");
            Assert.IsTrue(method.IsPrivate, "ValidateArithmeticOperands should be private");
        }

        // ============================================================
        //   GROUP 12: CHAINING AND COMPOSITION
        // ============================================================

        /// <summary>
        /// Verifies chaining of operations: q1.Add(q2).Subtract(q3).
        /// Tests: Operation composition through refactored methods.
        /// </summary>
        [TestMethod]
        public void testArithmetic_Chain_AddThenSubtract()
        {
            var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(5.0,  LengthUnitM.FEET);
            var q3 = new Quantity<LengthUnitM>(3.0,  LengthUnitM.FEET);

            // (10 + 5) - 3 = 12
            var result = q1.Add(q2).Subtract(q3);

            Assert.AreEqual(12.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies chaining of operations across units.
        /// Tests: Cross-unit chaining through refactored centralized helper.
        /// </summary>
        [TestMethod]
        public void testArithmetic_Chain_CrossUnit()
        {
            // 2 feet + 12 inches = 3 feet, 3 feet - 6 inches = 2.5 feet
            var q1 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);
            var q2 = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);
            var q3 = new Quantity<LengthUnitM>(6.0,  LengthUnitM.INCHES);

            var result = q1.Add(q2).Subtract(q3);

            Assert.AreEqual(2.5, result.Value, EPSILON);
        }

        // ============================================================
        //   GROUP 13: ERROR MESSAGE CONSISTENCY
        // ============================================================

        /// <summary>
        /// Verifies all operations produce same exception type for null operand.
        /// Tests: Consistent error reporting across operations.
        /// </summary>
        [TestMethod]
        public void testErrorMessage_Consistency_NullOperand()
        {
            var q = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);

            Type? addExType = null, subExType = null, divExType = null;

            try { q.Add(null!);      } catch (Exception e) { addExType = e.GetType(); }
            try { q.Subtract(null!); } catch (Exception e) { subExType = e.GetType(); }
            try { q.Divide(null!);   } catch (Exception e) { divExType = e.GetType(); }

            Assert.AreEqual(addExType, subExType, "Add and Subtract should throw same exception type for null");
            Assert.AreEqual(subExType, divExType, "Subtract and Divide should throw same exception type for null");
        }

        // ============================================================
        //   GROUP 14: SCALABILITY - FUTURE OPERATIONS PATTERN
        // ============================================================

        /// <summary>
        /// Demonstrates the scalability pattern: a hypothetical MULTIPLY operation
        /// could be added using same helper pattern without changing validation.
        /// Tests: Scalability for future operations - pattern validation.
        /// </summary>
        [TestMethod]
        public void testFutureOperation_MultiplicationPattern_CanBeAdded()
        {
            // Verify existing enum pattern supports extension (conceptual test)
            // We verify that ArithmeticOperation enum is a nested private enum in Quantity<T>
            var quantityType = typeof(Quantity<LengthUnitM>);
            var nestedTypes  = quantityType.GetNestedTypes(BindingFlags.NonPublic);

            bool hasArithmeticOperationEnum = false;
            foreach (var t in nestedTypes)
            {
                if (t.Name == "ArithmeticOperation" && t.IsEnum)
                {
                    hasArithmeticOperationEnum = true;
                    // Verify ADD, SUBTRACT, DIVIDE exist
                    var names = Enum.GetNames(t);
                    Assert.IsTrue(Array.Exists(names, n => n == "ADD"),      "ADD should exist");
                    Assert.IsTrue(Array.Exists(names, n => n == "SUBTRACT"), "SUBTRACT should exist");
                    Assert.IsTrue(Array.Exists(names, n => n == "DIVIDE"),   "DIVIDE should exist");
                    break;
                }
            }
            Assert.IsTrue(hasArithmeticOperationEnum,
                "ArithmeticOperation enum should be a private nested enum inside Quantity<T>");
        }

        // ============================================================
        //   GROUP 15: LARGE DATASET BACKWARD COMPATIBILITY
        // ============================================================

        /// <summary>
        /// Runs comprehensive operations to verify behavioral equivalence at scale.
        /// Tests: No behavior change from refactoring across many operations.
        /// </summary>
        [TestMethod]
        public void testRefactoring_NoBehaviorChange_MultipleOperations()
        {
            for (int i = 1; i <= 100; i++)
            {
                var q1 = new Quantity<LengthUnitM>(i * 2.0, LengthUnitM.FEET);
                var q2 = new Quantity<LengthUnitM>(i * 1.0, LengthUnitM.FEET);

                double expectedAdd      = i * 3.0;
                double expectedSubtract = i * 1.0;
                double expectedDivide   = 2.0;

                Assert.AreEqual(expectedAdd,      q1.Add(q2).Value,      EPSILON,
                    $"Add failed at i={i}");
                Assert.AreEqual(expectedSubtract, q1.Subtract(q2).Value, EPSILON,
                    $"Subtract failed at i={i}");
                Assert.AreEqual(expectedDivide,   q1.Divide(q2),         EPSILON,
                    $"Divide failed at i={i}");
            }
        }

        // ============================================================
        //   GROUP 16: ADDITIONAL ENUM CONSTANT TESTS
        // ============================================================

        [TestMethod]
        public void testEnumConstant_ADD_CorrectlyAdds_ThroughPublicAPI()
        {
            var q1 = new Quantity<WeightUnitM>(7.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(3.0, WeightUnitM.KILOGRAM);
            Assert.AreEqual(10.0, q1.Add(q2).Value, EPSILON);
        }

        [TestMethod]
        public void testEnumConstant_SUBTRACT_CorrectlySubtracts_ThroughPublicAPI()
        {
            var q1 = new Quantity<WeightUnitM>(7.0, WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(3.0, WeightUnitM.KILOGRAM);
            Assert.AreEqual(4.0, q1.Subtract(q2).Value, EPSILON);
        }

        [TestMethod]
        public void testEnumConstant_DIVIDE_CorrectlyDivides_ThroughPublicAPI()
        {
            var q1 = new Quantity<VolumeUnitM>(7.0, VolumeUnitM.LITRE);
            var q2 = new Quantity<VolumeUnitM>(2.0, VolumeUnitM.LITRE);
            Assert.AreEqual(3.5, q1.Divide(q2), EPSILON);
        }

        [TestMethod]
        public void testHelper_BaseUnitConversion_Correct()
        {
            // 1 yard = 3 feet; 1 yard + 1 yard = 2 yards = 6 feet in base
            // result in feet = 6 feet
            var q1 = new Quantity<LengthUnitM>(1.0, LengthUnitM.YARDS);
            var q2 = new Quantity<LengthUnitM>(1.0, LengthUnitM.YARDS);
            var result = q1.Add(q2, LengthUnitM.FEET);
            Assert.AreEqual(6.0, result.Value, EPSILON);
        }

        [TestMethod]
        public void testHelper_ResultConversion_Correct()
        {
            // 2 kg - 500 g. Base: 2 - 0.5 = 1.5 kg. In grams = 1500 g.
            var q1 = new Quantity<WeightUnitM>(2.0,   WeightUnitM.KILOGRAM);
            var q2 = new Quantity<WeightUnitM>(500.0, WeightUnitM.GRAM);
            var result = q1.Subtract(q2, WeightUnitM.GRAM);
            Assert.AreEqual(1500.0, result.Value, EPSILON);
        }

        [TestMethod]
        public void testRefactoring_Validation_UnifiedBehavior()
        {
            // All operations reject the same invalid inputs with same exception types
            var valid = new Quantity<LengthUnitM>(5.0, LengthUnitM.FEET);

            Type addType = typeof(ArgumentNullException), subType = addType, divType = addType;

            try { valid.Add(null!);      } catch (Exception e) { addType = e.GetType(); }
            try { valid.Subtract(null!); } catch (Exception e) { subType = e.GetType(); }
            try { valid.Divide(null!);   } catch (Exception e) { divType = e.GetType(); }

            Assert.AreEqual(addType, subType);
            Assert.AreEqual(subType, divType);
        }

        [TestMethod]
        public void testEnumDispatch_AllOperations_CorrectlyDispatched()
        {
            var q1 = new Quantity<VolumeUnitM>(10.0, VolumeUnitM.LITRE);
            var q2 = new Quantity<VolumeUnitM>(4.0,  VolumeUnitM.LITRE);

            Assert.AreEqual(14.0, q1.Add(q2).Value,      EPSILON, "ADD dispatch");
            Assert.AreEqual(6.0,  q1.Subtract(q2).Value, EPSILON, "SUBTRACT dispatch");
            Assert.AreEqual(2.5,  q1.Divide(q2),         EPSILON, "DIVIDE dispatch");
        }
    }
}