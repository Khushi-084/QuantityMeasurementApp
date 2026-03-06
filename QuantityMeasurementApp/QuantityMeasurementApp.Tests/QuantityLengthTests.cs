using NUnit.Framework;
using QuantityMeasurementApp.Domain;   // For QuantityLength
using QuantityMeasurementApp.ServiceLayer;  // For LengthUnit and QuantityLengthService

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class QuantityLengthTests
    {
        // ----------------- Feet Tests -----------------
        [Test]
        public void testEquality_FeetToFeet_SameValue()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.Feet);
            var q2 = new QuantityLength(1.0, LengthUnit.Feet);

            Assert.That(q1.Equals(q2), Is.True);
        }

        [Test]
        public void testEquality_FeetToFeet_DifferentValue()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.Feet);
            var q2 = new QuantityLength(2.0, LengthUnit.Feet);

            Assert.That(q1.Equals(q2), Is.False);
        }

        // ----------------- Inch Tests -----------------
        [Test]
        public void testEquality_InchToInch_SameValue()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.Inch);
            var q2 = new QuantityLength(1.0, LengthUnit.Inch);

            Assert.That(q1.Equals(q2), Is.True);
        }

        [Test]
        public void testEquality_InchToInch_DifferentValue()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.Inch);
            var q2 = new QuantityLength(2.0, LengthUnit.Inch);

            Assert.That(q1.Equals(q2), Is.False);
        }

        // ----------------- Feet ↔ Inch Conversion -----------------
        [Test]
        public void testEquality_FeetToInch_EquivalentValue()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.Feet);
            var q2 = new QuantityLength(12.0, LengthUnit.Inch);

            Assert.That(q1.Equals(q2), Is.True);
        }

        [Test]
        public void testEquality_InchToFeet_EquivalentValue()
        {
            var q1 = new QuantityLength(12.0, LengthUnit.Inch);
            var q2 = new QuantityLength(1.0, LengthUnit.Feet);

            Assert.That(q1.Equals(q2), Is.True);
        }

        // ----------------- Invalid / Null Unit -----------------
        [Test]
        public void testEquality_InvalidUnit()
        {
            Assert.Throws<System.ArgumentException>(() =>
            {
                // Assuming QuantityLength constructor validates supported units
                var q = new QuantityLength(1.0, (LengthUnit)999);
            });
        }

        [Test]
        public void testEquality_NullUnit()
        {
            QuantityLength? q1 = null;
            var q2 = new QuantityLength(1.0, LengthUnit.Feet);

            Assert.IsFalse(q2.Equals(q1));
        }

        // ----------------- Reference / Null Checks -----------------
        [Test]
        public void testEquality_SameReference()
        {
            var q = new QuantityLength(1.0, LengthUnit.Feet);
            Assert.IsTrue(q.Equals(q));
        }

        [Test]
        public void testEquality_NullComparison()
        {
            var q = new QuantityLength(1.0, LengthUnit.Feet);
            Assert.IsFalse(q.Equals(null));
        }
    }
}