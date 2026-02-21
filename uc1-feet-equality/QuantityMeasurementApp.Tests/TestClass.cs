using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using System;

namespace QuantityMeasurementApp.Tests
{
    public class FeetTests
    {
        // Global variables for reuse in test methods
        private Feet numberOne;
        private Feet numberTwo;

        //  Runs before each test
        [SetUp]
        public void Setup()
        {
            numberOne = new Feet(5);
            numberTwo = new Feet(5);
        }

        // Test: Same values should be equal
        [Test]
        public void GivenTwoFeetObjects_WithSameValue_ShouldReturnTrue()
        {
            Assert.AreEqual(numberOne, numberTwo);
        }

        // Test: Different values should not be equal
        [Test]
        public void GivenTwoFeetObjects_WithDifferentValue_ShouldReturnFalse()
        {
            numberTwo = new Feet(6);

            Assert.AreNotEqual(numberOne, numberTwo);
        }

        // Test: Comparing with null should return false
        [Test]
        public void GivenFeetObject_WhenComparedWithNull_ShouldReturnFalse()
        {
            Assert.False(numberOne.Equals(null));
        }

        //  Test: Same reference should return true
        [Test]
        public void GivenSameReference_ShouldReturnTrue()
        {
            Assert.True(numberOne.Equals(numberOne));
        }

        // Test: Small floating-point difference within tolerance
        [Test]
        public void GivenTwoFeetObjects_WithSmallDifferenceWithinTolerance_ShouldReturnTrue()
        {
            numberOne = new Feet(5.00001);
            numberTwo = new Feet(5.00002);

            Assert.AreEqual(numberOne, numberTwo);
        }

        // Test: Difference greater than tolerance should return false
        [Test]
        public void GivenTwoFeetObjects_WithDifferenceGreaterThanTolerance_ShouldReturnFalse()
        {
            numberOne = new Feet(5.0001);
            numberTwo = new Feet(5.001);

            Assert.AreNotEqual(numberOne, numberTwo);
        }

        // Test: Comparing with different object type
        [Test]
        public void GivenFeetObject_WhenComparedWithDifferentType_ShouldReturnFalse()
        {
            object obj = 5;

            Assert.False(numberOne.Equals(obj));
        }

        // Test: Equal objects must have same hashcode
        [Test]
        public void GivenTwoEqualFeetObjects_ShouldHaveSameHashCode()
        {
            Assert.AreEqual(numberOne.GetHashCode(), numberTwo.GetHashCode());
        }

        //  Exception Test: Negative value should throw exception
        [Test]
        public void GivenNegativeFeetValue_WhenCreatingObject_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(-5));
        }

        //  Exception Test: Very large value should throw exception
        [Test]
        public void GivenVeryLargeFeetValue_WhenCreatingObject_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(20000));
        }

        // Test: Zero value should be valid
        [Test]
        public void GivenZeroFeetValues_ShouldReturnTrue()
        {
            numberOne = new Feet(0);
            numberTwo = new Feet(0);

            Assert.AreEqual(numberOne, numberTwo);
        }
    }
}