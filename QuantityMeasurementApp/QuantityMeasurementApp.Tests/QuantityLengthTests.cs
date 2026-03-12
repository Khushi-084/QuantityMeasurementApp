using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using System;

namespace QuantityMeasurementApp.Tests
{
    // UC3 - Generic Quantity Length Tests
    // Validates DRY implementation + cross-unit equality

    [TestClass]
    public class QuantityLengthTests
    {
        private QuantityLength oneFoot = null!;
        private QuantityLength twelveInches = null!;
        private QuantityLength twoFeet = null!;
        private QuantityLength oneInch = null!;

        // Runs before each test
        [TestInitialize]
        public void Setup()
        {
            oneFoot = new QuantityLength(1.0, LengthUnit.FEET);
            twelveInches = new QuantityLength(12.0, LengthUnit.INCH);
            twoFeet = new QuantityLength(2.0, LengthUnit.FEET);
            oneInch = new QuantityLength(1.0, LengthUnit.INCH);
        }

        // ---------- SAME UNIT ----------

        [TestMethod]
        public void Feet_To_Feet_SameValue_ReturnsTrue()
        {
            var another = new QuantityLength(1.0, LengthUnit.FEET);
            Assert.IsTrue(oneFoot.Equals(another));
        }

        [TestMethod]
        public void Inch_To_Inch_SameValue_ReturnsTrue()
        {
            var another = new QuantityLength(1.0, LengthUnit.INCH);
            Assert.IsTrue(oneInch.Equals(another));
        }

        // ---------- CROSS UNIT ----------

        [TestMethod]
        public void Feet_To_Inch_Equivalent_ReturnsTrue()
        {
            Assert.IsTrue(oneFoot.Equals(twelveInches));
        }

        [TestMethod]
        public void Inch_To_Feet_Equivalent_ReturnsTrue()
        {
            Assert.IsTrue(twelveInches.Equals(oneFoot));
        }

        // ---------- DIFFERENT VALUES ----------

        [TestMethod]
        public void Different_Feet_ReturnsFalse()
        {
            Assert.IsFalse(oneFoot.Equals(twoFeet));
        }

        [TestMethod]
        public void Different_Inches_ReturnsFalse()
        {
            var twoInches = new QuantityLength(2.0, LengthUnit.INCH);
            Assert.IsFalse(oneInch.Equals(twoInches));
        }

        // ---------- EQUALITY CONTRACT ----------

        [TestMethod]
        public void SameReference_ReturnsTrue()
        {
            Assert.IsTrue(oneFoot.Equals(oneFoot));
        }

        [TestMethod]
        public void NullComparison_ReturnsFalse()
        {
            Assert.IsFalse(oneFoot.Equals(null));
        }

        [TestMethod]
        public void SymmetricProperty()
        {
            Assert.IsTrue(oneFoot.Equals(twelveInches));
            Assert.IsTrue(twelveInches.Equals(oneFoot));
        }

        [TestMethod]
        public void TransitiveProperty()
        {
            var anotherFoot = new QuantityLength(1.0, LengthUnit.FEET);

            Assert.IsTrue(oneFoot.Equals(twelveInches));
            Assert.IsTrue(twelveInches.Equals(anotherFoot));
            Assert.IsTrue(oneFoot.Equals(anotherFoot));
        }

        [TestMethod]
        public void ConsistencyProperty()
        {
            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(oneFoot.Equals(twelveInches));
            }
        }

        // ---------- INVALID UNIT ----------

        [TestMethod]
        public void InvalidUnit_ShouldThrowException()
        {
            bool exceptionThrown = false;

            try
            {
                var invalid = (LengthUnit)999;
                new QuantityLength(1.0, invalid);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}
