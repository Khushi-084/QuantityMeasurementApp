using NUnit.Framework;
using QuantityMeasurementApp.DomainLayer;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class InchesTests
    {
        // Same value comparison
        [Test]
        public void testEquality_SameValue_ShouldReturnTrue()
        {
            Inches inchesNumber1 = new Inches(10);
            Inches inchesNumber2 = new Inches(10);

            Assert.IsTrue(inchesNumber1.Equals(inchesNumber2));
        }

        // Different value comparison
        [Test]
        public void testEquality_DifferentValue_ShouldReturnFalse()
        {
            Inches inchesNumber1 = new Inches(10);
            Inches inchesNumber2 = new Inches(12);

            Assert.IsFalse(inchesNumber1.Equals(inchesNumber2));
        }

        //  Zero boundary test
        [Test]
        public void testEquality_ZeroValue_ShouldBeValid()
        {
            Inches inches = new Inches(0);

            Assert.AreEqual(0, inches.Value);
        }

        //  Negative input should throw exception
        [Test]
        public void testEquality_NegativeValue_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Inches(-5));
        }

        //  Very large value should throw exception
        [Test]
        public void testEquality_VeryLargeValue_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Inches(1000001));
        }

        //  NaN value should throw exception
        [Test]
        public void testEquality_NaN_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Inches(double.NaN));
        }

        //  Infinity value should throw exception
        [Test]
        public void testEquality_Infinity_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Inches(double.PositiveInfinity));
        }

        //  Null comparison
        [Test]
        public void testEquality_NullComparison_ShouldReturnFalse()
        {
            Inches inches = new Inches(5);

            Assert.IsFalse(inches.Equals(null));
        }

        //  Same reference comparison
        [Test]
        public void testEquality_SameReference_ShouldReturnTrue()
        {
            Inches inches = new Inches(5);

            Assert.IsTrue(inches.Equals(inches));
        }

        //  Different object type comparison
        [Test]
        public void testEquality_DifferentObjectType_ShouldReturnFalse()
        {
            Inches inches = new Inches(5);

            Assert.IsFalse(inches.Equals("5"));
        }
    }
}