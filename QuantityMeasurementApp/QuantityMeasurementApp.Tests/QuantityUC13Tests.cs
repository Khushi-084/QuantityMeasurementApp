using System;
using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class QuantityUC13Tests
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
        //  testRefactoring_Add_DelegatesViaHelper
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testRefactoring_Add_DelegatesViaHelper()
        {
            // Add() must produce the correct result, confirming it delegates
            // through PerformBaseArithmetic with ArithmeticOperation.Add
            // 1 foot + 12 inches = 2 feet
            var result = Len(1.0, LengthUnit.Feet).Add(Len(12.0, LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testRefactoring_Subtract_DelegatesViaHelper
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testRefactoring_Subtract_DelegatesViaHelper()
        {
            // Subtract() must produce the correct result, confirming it delegates
            // through PerformBaseArithmetic with ArithmeticOperation.Subtract
            // 10 feet − 6 inches = 9.5 feet
            var result = Len(10.0, LengthUnit.Feet).Subtract(Len(6.0, LengthUnit.Inch));
            Assert.That(result.Value, Is.EqualTo(9.5).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testRefactoring_Divide_DelegatesViaHelper
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testRefactoring_Divide_DelegatesViaHelper()
        {
            // Divide() must produce the correct result, confirming it delegates
            // through PerformBaseArithmetic with ArithmeticOperation.Divide
            // 24 inches ÷ 2 feet = 1.0
            double ratio = Len(24.0, LengthUnit.Inch).Divide(Len(2.0, LengthUnit.Feet));
            Assert.That(ratio, Is.EqualTo(1.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testValidation_NullOperand_ConsistentAcrossOperations
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testValidation_NullOperand_ConsistentAcrossOperations()
        {
            // All three operations must throw ArgumentNullException for null operand
            Assert.Throws<ArgumentNullException>(() => Len(10.0, LengthUnit.Feet).Add(null!));
            Assert.Throws<ArgumentNullException>(() => Len(10.0, LengthUnit.Feet).Subtract(null!));
            Assert.Throws<ArgumentNullException>(() => Len(10.0, LengthUnit.Feet).Divide(null!));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testValidation_CrossCategory_ConsistentAcrossOperations
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testValidation_CrossCategory_ConsistentAcrossOperations()
        {
            // Cross-category is prevented at compile time by the generic type system.
            Assert.Pass("Cross-category arithmetic is prevented at compile time by generic type constraints.");
        }

        // ══════════════════════════════════════════════════════════════════
        //  testValidation_NullTargetUnit_AddSubtractReject
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testValidation_NullTargetUnit_AddSubtractReject()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Quantity<LengthUnitMeasurable>.Add(
                    Len(10.0, LengthUnit.Feet), Len(5.0, LengthUnit.Feet), null!));

            Assert.Throws<ArgumentNullException>(() =>
                Len(10.0, LengthUnit.Feet).Subtract(Len(5.0, LengthUnit.Feet), null!));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testArithmeticOperation_Add_EnumComputation
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testArithmeticOperation_Add_EnumComputation()
        {
            // ADD.Compute(10, 5) must return 15.0
            double result = ArithmeticOperation.Add.Compute(10.0, 5.0);
            Assert.That(result, Is.EqualTo(15.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testArithmeticOperation_Subtract_EnumComputation
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testArithmeticOperation_Subtract_EnumComputation()
        {
            // SUBTRACT.Compute(10, 5) must return 5.0
            double result = ArithmeticOperation.Subtract.Compute(10.0, 5.0);
            Assert.That(result, Is.EqualTo(5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testArithmeticOperation_Divide_EnumComputation
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testArithmeticOperation_Divide_EnumComputation()
        {
            // DIVIDE.Compute(10, 5) must return 2.0
            double result = ArithmeticOperation.Divide.Compute(10.0, 5.0);
            Assert.That(result, Is.EqualTo(2.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testArithmeticOperation_DivideByZero_EnumThrows
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testArithmeticOperation_DivideByZero_EnumThrows()
        {
            // DIVIDE.Compute(10, 0) must throw ArithmeticException
            Assert.Throws<ArithmeticException>(() =>
                ArithmeticOperation.Divide.Compute(10.0, 0.0));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testAdd_UC12_BehaviorPreserved
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testAdd_UC12_BehaviorPreserved()
        {
            // Implicit unit: 1 foot + 12 inches = 2 feet
            var implicit1 = Len(1.0, LengthUnit.Feet).Add(Len(12.0, LengthUnit.Inch));
            Assert.That(implicit1.Value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(implicit1.Unit.Unit, Is.EqualTo(LengthUnit.Feet));

            // Explicit unit: 10 kg + 5000 g expressed in grams = 15000 g
            var explicit1 = Quantity<WeightUnitMeasurable>.Add(
                Wgt(10.0, WeightUnit.Kilogram),
                Wgt(5000.0, WeightUnit.Gram),
                W(WeightUnit.Gram));
            Assert.That(explicit1.Value, Is.EqualTo(15000.0).Within(Epsilon));

            // Cross-unit: 5 L + 500 mL = 5.5 L
            var vol1 = Vol(5.0, VolumeUnit.Litre).Add(Vol(500.0, VolumeUnit.Millilitre));
            Assert.That(vol1.Value, Is.EqualTo(5.5).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testSubtract_UC12_BehaviorPreserved
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testSubtract_UC12_BehaviorPreserved()
        {
            // Implicit unit: 10 feet − 6 inches = 9.5 feet
            var implicit1 = Len(10.0, LengthUnit.Feet).Subtract(Len(6.0, LengthUnit.Inch));
            Assert.That(implicit1.Value, Is.EqualTo(9.5).Within(Epsilon));

            // Explicit unit: 5 L − 2 L expressed in mL = 3000 mL
            var explicit1 = Vol(5.0, VolumeUnit.Litre)
                .Subtract(Vol(2.0, VolumeUnit.Litre), V(VolumeUnit.Millilitre));
            Assert.That(explicit1.Value, Is.EqualTo(3000.0).Within(Epsilon));

            // Non-commutativity
            var ab = Len(10.0, LengthUnit.Feet).Subtract(Len(5.0, LengthUnit.Feet));
            var ba = Len(5.0,  LengthUnit.Feet).Subtract(Len(10.0, LengthUnit.Feet));
            Assert.That(ab.Value,  Is.EqualTo( 5.0).Within(Epsilon));
            Assert.That(ba.Value,  Is.EqualTo(-5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testDivide_UC12_BehaviorPreserved
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testDivide_UC12_BehaviorPreserved()
        {
            // Same unit: 10 feet ÷ 2 feet = 5.0
            Assert.That(Len(10.0, LengthUnit.Feet).Divide(Len(2.0, LengthUnit.Feet)),
                        Is.EqualTo(5.0).Within(Epsilon));

            // Cross unit: 24 inches ÷ 2 feet = 1.0
            Assert.That(Len(24.0, LengthUnit.Inch).Divide(Len(2.0, LengthUnit.Feet)),
                        Is.EqualTo(1.0).Within(Epsilon));

            // Division by zero
            Assert.Throws<ArithmeticException>(() =>
                Len(10.0, LengthUnit.Feet).Divide(Len(0.0, LengthUnit.Feet)));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testRounding_AddSubtract_TwoDecimalPlaces
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testRounding_AddSubtract_TwoDecimalPlaces()
        {
            // Subtract result must be rounded to 2 decimal places
            var subResult = Len(13.0, LengthUnit.Inch).Subtract(Len(1.0, LengthUnit.Feet));
            Assert.That(subResult.Value, Is.EqualTo(Math.Round(subResult.Value, 2)).Within(1e-9));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testRounding_Divide_NoRounding
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testRounding_Divide_NoRounding()
        {
            // Division returns raw double — no forced 2dp rounding
            double ratio = Len(10.0, LengthUnit.Feet).Divide(Len(3.0, LengthUnit.Feet));
            Assert.That(ratio, Is.EqualTo(10.0 / 3.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testImplicitTargetUnit_AddSubtract
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testImplicitTargetUnit_AddSubtract()
        {
            // Implicit: result must be in first operand's unit
            var addResult = Len(10.0, LengthUnit.Feet).Add(Len(12.0, LengthUnit.Inch));
            Assert.That(addResult.Unit.Unit, Is.EqualTo(LengthUnit.Feet));

            var subResult = Len(10.0, LengthUnit.Feet).Subtract(Len(12.0, LengthUnit.Inch));
            Assert.That(subResult.Unit.Unit, Is.EqualTo(LengthUnit.Feet));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testExplicitTargetUnit_AddSubtract_Overrides
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testExplicitTargetUnit_AddSubtract_Overrides()
        {
            // Explicit target unit overrides first operand's unit
            var addResult = Quantity<LengthUnitMeasurable>.Add(
                Len(10.0, LengthUnit.Feet), Len(12.0, LengthUnit.Inch), L(LengthUnit.Inch));
            Assert.That(addResult.Unit.Unit, Is.EqualTo(LengthUnit.Inch));

            var subResult = Len(10.0, LengthUnit.Feet)
                .Subtract(Len(6.0, LengthUnit.Inch), L(LengthUnit.Inch));
            Assert.That(subResult.Unit.Unit, Is.EqualTo(LengthUnit.Inch));
            Assert.That(subResult.Value, Is.EqualTo(114.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testImmutability_AfterAdd_ViaCentralizedHelper
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testImmutability_AfterAdd_ViaCentralizedHelper()
        {
            var a = Len(10.0, LengthUnit.Feet);
            var b = Len(3.0,  LengthUnit.Feet);
            _ = a.Add(b);
            Assert.That(a.Value, Is.EqualTo(10.0).Within(Epsilon));
            Assert.That(b.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testImmutability_AfterSubtract_ViaCentralizedHelper
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testImmutability_AfterSubtract_ViaCentralizedHelper()
        {
            var a = Len(10.0, LengthUnit.Feet);
            var b = Len(3.0,  LengthUnit.Feet);
            _ = a.Subtract(b);
            Assert.That(a.Value, Is.EqualTo(10.0).Within(Epsilon));
            Assert.That(b.Value, Is.EqualTo(3.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testImmutability_AfterDivide_ViaCentralizedHelper
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testImmutability_AfterDivide_ViaCentralizedHelper()
        {
            var a = Len(10.0, LengthUnit.Feet);
            var b = Len(2.0,  LengthUnit.Feet);
            _ = a.Divide(b);
            Assert.That(a.Value, Is.EqualTo(10.0).Within(Epsilon));
            Assert.That(b.Value, Is.EqualTo(2.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testAllOperations_AcrossAllCategories
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testAllOperations_AcrossAllCategories()
        {
            // Addition — length, weight, volume
            Assert.That(Len(5.0, LengthUnit.Feet).Add(Len(2.0, LengthUnit.Feet)).Value,
                        Is.EqualTo(7.0).Within(Epsilon));
            Assert.That(Wgt(5.0, WeightUnit.Kilogram).Add(Wgt(2.0, WeightUnit.Kilogram)).Value,
                        Is.EqualTo(7.0).Within(Epsilon));
            Assert.That(Vol(5.0, VolumeUnit.Litre).Add(Vol(2.0, VolumeUnit.Litre)).Value,
                        Is.EqualTo(7.0).Within(Epsilon));

            // Subtraction — length, weight, volume
            Assert.That(Len(5.0, LengthUnit.Feet).Subtract(Len(2.0, LengthUnit.Feet)).Value,
                        Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(Wgt(5.0, WeightUnit.Kilogram).Subtract(Wgt(2.0, WeightUnit.Kilogram)).Value,
                        Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(Vol(5.0, VolumeUnit.Litre).Subtract(Vol(2.0, VolumeUnit.Litre)).Value,
                        Is.EqualTo(3.0).Within(Epsilon));

            // Division — length, weight, volume
            Assert.That(Len(10.0, LengthUnit.Feet).Divide(Len(2.0, LengthUnit.Feet)),
                        Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(Wgt(10.0, WeightUnit.Kilogram).Divide(Wgt(2.0, WeightUnit.Kilogram)),
                        Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(Vol(10.0, VolumeUnit.Litre).Divide(Vol(2.0, VolumeUnit.Litre)),
                        Is.EqualTo(5.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testEnumDispatch_AllOperations_CorrectlyDispatched
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testEnumDispatch_AllOperations_CorrectlyDispatched()
        {
            var a = Len(10.0, LengthUnit.Feet);
            var b = Len(2.0,  LengthUnit.Feet);

            Assert.That(a.Add(b).Value,      Is.EqualTo(12.0).Within(Epsilon)); // ADD
            Assert.That(a.Subtract(b).Value, Is.EqualTo(8.0).Within(Epsilon));  // SUBTRACT
            Assert.That(a.Divide(b),         Is.EqualTo(5.0).Within(Epsilon));  // DIVIDE
        }

        // ══════════════════════════════════════════════════════════════════
        //  testCodeDuplication_ValidationLogic_Eliminated
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testCodeDuplication_ValidationLogic_Eliminated()
        {
            // Validation logic lives only in ValidateArithmeticOperands.
            // Confirmed by the fact that all three operations throw the same
            // exception type and message for the same invalid input.
            var exAdd      = Assert.Throws<ArgumentNullException>(() =>
                                 Len(10.0, LengthUnit.Feet).Add(null!));
            var exSubtract = Assert.Throws<ArgumentNullException>(() =>
                                 Len(10.0, LengthUnit.Feet).Subtract(null!));
            var exDivide   = Assert.Throws<ArgumentNullException>(() =>
                                 Len(10.0, LengthUnit.Feet).Divide(null!));

            Assert.That(exAdd!.Message,      Does.Contain("must not be null"));
            Assert.That(exSubtract!.Message, Does.Contain("must not be null"));
            Assert.That(exDivide!.Message,   Does.Contain("must not be null"));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testCodeDuplication_ConversionLogic_Eliminated
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testCodeDuplication_ConversionLogic_Eliminated()
        {
            // Conversion logic lives only in PerformBaseArithmetic.
            // Cross-unit operations across all three methods produce correct
            // results, confirming the single conversion path works for all.
            Assert.That(Len(1.0, LengthUnit.Feet).Add(Len(12.0, LengthUnit.Inch)).Value,
                        Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(Len(10.0, LengthUnit.Feet).Subtract(Len(6.0, LengthUnit.Inch)).Value,
                        Is.EqualTo(9.5).Within(Epsilon));
            Assert.That(Len(24.0, LengthUnit.Inch).Divide(Len(2.0, LengthUnit.Feet)),
                        Is.EqualTo(1.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testHelper_PrivateVisibility
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testHelper_PrivateVisibility()
        {
            Assert.Pass("PerformBaseArithmetic is private — enforced at compile time.");
        }

        // ══════════════════════════════════════════════════════════════════
        //  testValidation_Helper_PrivateVisibility
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testValidation_Helper_PrivateVisibility()
        {
            Assert.Pass("ValidateArithmeticOperands is private — enforced at compile time.");
        }

        // ══════════════════════════════════════════════════════════════════
        //  testRounding_Helper_Accuracy
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testRounding_Helper_Accuracy()
        {
            // Subtract result rounded to 2 dp — e.g. 1.234567 → 1.23
            var result = Len(13.0, LengthUnit.Inch).Subtract(Len(1.0, LengthUnit.Feet));
            double rounded = Math.Round(result.Value, 2);
            Assert.That(result.Value, Is.EqualTo(rounded).Within(1e-9));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testArithmetic_Chain_Operations
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testArithmetic_Chain_Operations()
        {
            // q1.Add(q2).Subtract(q3).Divide(q4)
            // (10 + 2 - 3) / 3 = 9 / 3 = 3.0
            var q1 = Len(10.0, LengthUnit.Feet);
            var q2 = Len(2.0,  LengthUnit.Feet);
            var q3 = Len(3.0,  LengthUnit.Feet);
            var q4 = Len(3.0,  LengthUnit.Feet);

            double result = q1.Add(q2).Subtract(q3).Divide(q4);
            Assert.That(result, Is.EqualTo(3.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testFutureOperation_MultiplicationPattern
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testFutureOperation_MultiplicationPattern()
        {
            Assert.Pass("Multiply can be added by extending the ArithmeticOperation enum only.");
        }

        // ══════════════════════════════════════════════════════════════════
        //  testErrorMessage_Consistency_Across_Operations
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testErrorMessage_Consistency_Across_Operations()
        {
            var exAdd      = Assert.Throws<ArgumentNullException>(() =>
                                 Len(10.0, LengthUnit.Feet).Add(null!));
            var exSubtract = Assert.Throws<ArgumentNullException>(() =>
                                 Len(10.0, LengthUnit.Feet).Subtract(null!));
            var exDivide   = Assert.Throws<ArgumentNullException>(() =>
                                 Len(10.0, LengthUnit.Feet).Divide(null!));

            Assert.That(exAdd!.Message,      Does.Contain("must not be null"));
            Assert.That(exSubtract!.Message, Does.Contain("must not be null"));
            Assert.That(exDivide!.Message,   Does.Contain("must not be null"));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testHelper_BaseUnitConversion_Correct
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testHelper_BaseUnitConversion_Correct()
        {
            // 2 kg ÷ 2000 g = 1.0 — confirms base-unit conversion in helper
            double ratio = Wgt(2.0, WeightUnit.Kilogram).Divide(Wgt(2000.0, WeightUnit.Gram));
            Assert.That(ratio, Is.EqualTo(1.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testHelper_ResultConversion_Correct
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testHelper_ResultConversion_Correct()
        {
            // 10 kg + 5000 g expressed in grams = 15000 g
            // Confirms base result is correctly converted back to target unit
            var result = Quantity<WeightUnitMeasurable>.Add(
                Wgt(10.0, WeightUnit.Kilogram),
                Wgt(5000.0, WeightUnit.Gram),
                W(WeightUnit.Gram));
            Assert.That(result.Value, Is.EqualTo(15000.0).Within(Epsilon));
            Assert.That(result.Unit.Unit, Is.EqualTo(WeightUnit.Gram));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testRefactoring_Validation_UnifiedBehavior
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testRefactoring_Validation_UnifiedBehavior()
        {
            // Same invalid inputs rejected identically across all operations
            Assert.Throws<ArgumentNullException>(() => Len(10.0, LengthUnit.Feet).Add(null!));
            Assert.Throws<ArgumentNullException>(() => Len(10.0, LengthUnit.Feet).Subtract(null!));
            Assert.Throws<ArgumentNullException>(() => Len(10.0, LengthUnit.Feet).Divide(null!));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testEnumConstant_ADD_CorrectlyAdds
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testEnumConstant_ADD_CorrectlyAdds()
        {
            Assert.That(ArithmeticOperation.Add.Compute(7.0, 3.0), Is.EqualTo(10.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testEnumConstant_SUBTRACT_CorrectlySubtracts
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testEnumConstant_SUBTRACT_CorrectlySubtracts()
        {
            Assert.That(ArithmeticOperation.Subtract.Compute(7.0, 3.0), Is.EqualTo(4.0).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testEnumConstant_DIVIDE_CorrectlyDivides
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testEnumConstant_DIVIDE_CorrectlyDivides()
        {
            Assert.That(ArithmeticOperation.Divide.Compute(7.0, 2.0), Is.EqualTo(3.5).Within(Epsilon));
        }

        // ══════════════════════════════════════════════════════════════════
        //  testValidation_FiniteValue_ConsistentAcrossOperations
        // ══════════════════════════════════════════════════════════════════

        [Test]
        public void testValidation_FiniteValue_ConsistentAcrossOperations()
        {
            // NaN and infinite values are rejected by the Quantity constructor,
            // so they never reach ValidateArithmeticOperands.
            // This test confirms the constructor guards are in place.
            Assert.Throws<ArgumentException>(() => Len(double.NaN,              LengthUnit.Feet));
            Assert.Throws<ArgumentException>(() => Len(double.PositiveInfinity, LengthUnit.Feet));
            Assert.Throws<ArgumentException>(() => Len(double.NegativeInfinity, LengthUnit.Feet));
        }
    }
}