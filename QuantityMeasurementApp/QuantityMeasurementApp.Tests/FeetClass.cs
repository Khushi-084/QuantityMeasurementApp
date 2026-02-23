using NUnit.Framework;
using QuantityMeasurementApp.DomainLayer;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class FeetTests
    {
        [Test]
        public void SameFeetValues_ShouldReturnTrue()
        {
            Feet feetNumber1 = new Feet(5);
            Feet feetNumber2 = new Feet(5);

            Assert.IsTrue(feetNumber1.Equals(feetNumber2));
        }

        [Test]
        public void DifferentFeetValues_ShouldReturnFalse()
        {
            Feet feetNumber1 = new Feet(5);
            Feet feetNumber2 = new Feet(10);

            Assert.IsFalse(feetNumber1.Equals(feetNumber2));
        }

        [Test]
        public void ZeroFeet_ShouldBeValid()
        {
            Feet feetNumber = new Feet(0);

            Assert.AreEqual(0, feetNumber.Value);
        }

        [Test]
        public void NegativeFeet_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(-1));
        }

        [Test]
        public void VeryLargeValue_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(1000001));
        }

        [Test]
        public void NaNValue_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(double.NaN));
        }

        [Test]
        public void InfinityValue_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(double.PositiveInfinity));
        }

        [Test]
        public void CompareWithNull_ShouldReturnFalse()
        {
            Feet feetNumber = new Feet(5);

            Assert.IsFalse(feetNumber.Equals(null));
        }

        [Test]
        public void CompareWithDifferentObject_ShouldReturnFalse()
        {
            Feet feetNumber = new Feet(5);

            Assert.IsFalse(feetNumber.Equals("5"));
        }
    }
}