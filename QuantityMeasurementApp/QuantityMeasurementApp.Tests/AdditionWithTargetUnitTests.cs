using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using System;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC7 - Addition with Explicit Target Unit
    /// </summary>
    [TestClass]
    public class UC7_AdditionWithTargetUnitTests
    {
        private const double EPSILON = 0.01;

        // Explicit Target Unit - Feet
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.FEET);

            Assert.AreEqual(2.0, result.ConvertToFeet(), 0.0001);
        }

        // Explicit Target Unit - Inches
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Inches()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.INCH);

            Assert.AreEqual(24.0, result.ConvertToFeet() * 12.0, 0.0001);
        }

        // Explicit Target Unit - Yards
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Yards()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.YARD);

            Assert.AreEqual(0.667, result.ConvertToFeet() / 3.0, EPSILON);
        }

        // Explicit Target Unit - Centimeters
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.INCH);
            var q2 = new QuantityLength(1.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.CENTIMETER);

            double cm = result.ConvertToFeet() / 0.0328084;
            Assert.AreEqual(5.08, cm, EPSILON);
        }

        // Target Same As First Operand
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            var q1 = new QuantityLength(2.0, LengthUnit.YARD);
            var q2 = new QuantityLength(3.0, LengthUnit.FEET);

            var result = q1.Add(q2, LengthUnit.YARD);

            Assert.AreEqual(3.0, result.ConvertToFeet() / 3.0, 0.0001);
        }

        // Target Same As Second Operand
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            var q1 = new QuantityLength(2.0, LengthUnit.YARD);
            var q2 = new QuantityLength(3.0, LengthUnit.FEET);

            var result = q1.Add(q2, LengthUnit.FEET);

            Assert.AreEqual(9.0, result.ConvertToFeet(), 0.0001);
        }

        //Commutativity
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);

            var r1 = q1.Add(q2, LengthUnit.YARD);
            var r2 = q2.Add(q1, LengthUnit.YARD);

            Assert.AreEqual(r1.ConvertToFeet(), r2.ConvertToFeet(), 0.0001);
        }

        // Zero Operand
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            var q1 = new QuantityLength(5.0, LengthUnit.FEET);
            var q2 = new QuantityLength(0.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.YARD);

            Assert.AreEqual(1.667, result.ConvertToFeet() / 3.0, EPSILON);
        }

        // Negative Values
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            var q1 = new QuantityLength(5.0, LengthUnit.FEET);
            var q2 = new QuantityLength(-2.0, LengthUnit.FEET);

            var result = q1.Add(q2, LengthUnit.INCH);

            Assert.AreEqual(36.0, result.ConvertToFeet() * 12.0, 0.0001);
        }

        //Null / Invalid Target Unit
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullTargetUnit()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.FEET);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);

            Assert.Throws<ArgumentException>(() =>
                q1.Add(q2, (LengthUnit)(-1)));
        }

        //  Large → Small Scale
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            var q1 = new QuantityLength(1000.0, LengthUnit.FEET);
            var q2 = new QuantityLength(500.0, LengthUnit.FEET);

            var result = q1.Add(q2, LengthUnit.INCH);

            Assert.AreEqual(18000.0, result.ConvertToFeet() * 12.0, 0.0001);
        }

        //  Small → Large Scale
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            var q1 = new QuantityLength(12.0, LengthUnit.INCH);
            var q2 = new QuantityLength(12.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.YARD);

            Assert.AreEqual(0.667, result.ConvertToFeet() / 3.0, EPSILON);
        }

        //  All Unit Combinations
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_AllUnitCombinations()
        {
            LengthUnit[] units =
            {
                LengthUnit.INCH,
                LengthUnit.FEET,
                LengthUnit.YARD,
                LengthUnit.CENTIMETER
            };

            foreach (var u1 in units)
            {
                foreach (var u2 in units)
                {
                    foreach (var target in units)
                    {
                        var q1 = new QuantityLength(1.0, u1);
                        var q2 = new QuantityLength(1.0, u2);

                        var result = q1.Add(q2, target);

                        Assert.IsNotNull(result);
                    }
                }
            }
        }

        // Precision Tolerance
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_PrecisionTolerance()
        {
            var q1 = new QuantityLength(2.54, LengthUnit.CENTIMETER);
            var q2 = new QuantityLength(1.0, LengthUnit.INCH);

            var result = q1.Add(q2, LengthUnit.CENTIMETER);

            double cm = result.ConvertToFeet() / 0.0328084;

            Assert.AreEqual(5.08, cm, EPSILON);
        }
    }
}
