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
            Feet f1 = new Feet(5);
            Feet f2 = new Feet(5);

            Assert.IsTrue(f1.Equals(f2));
        }

        [Test]
        public void DifferentFeetValues_ShouldReturnFalse()
        {
            Feet f1 = new Feet(5);
            Feet f2 = new Feet(10);

            Assert.IsFalse(f1.Equals(f2));
        }

        [Test]
        public void ZeroFeet_ShouldBeValid()
        {
            Feet f = new Feet(0);

            Assert.AreEqual(0, f.Value);
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
            Feet f = new Feet(5);

            Assert.IsFalse(f.Equals(null));
        }

        [Test]
        public void CompareWithDifferentObject_ShouldReturnFalse()
        {
            Feet f = new Feet(5);

            Assert.IsFalse(f.Equals("5"));
        }
    }
}