using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementApp.Tests
{
    // =====================================================
    // UC2 - Inches Equality Tests
    // Validates equality contract for Inches measurements
    // =====================================================

    [TestClass]
    public class InchesEqualityTests
    {
        // ================= GLOBAL TEST OBJECTS =================

        private Inches oneInchMeasurement;
        private Inches anotherOneInchMeasurement;
        private Inches twoInchMeasurement;
        private Inches zeroInchMeasurement;
        private Inches largeInchMeasurement;

        // Runs before EACH test
        [TestInitialize]
        public void Setup()
        {
            oneInchMeasurement = new Inches(1.0);
            anotherOneInchMeasurement = new Inches(1.0);
            twoInchMeasurement = new Inches(2.0);
            zeroInchMeasurement = new Inches(0.0);
            largeInchMeasurement = new Inches(1_000_000.0);
        }

        // ================= BASIC EQUALITY TESTS =================

        [TestMethod]
        public void InchesEquality_SameValue_ReturnsTrue()
        {
            Assert.IsTrue(oneInchMeasurement.Equals(anotherOneInchMeasurement));
        }

        [TestMethod]
        public void InchesEquality_DifferentValue_ReturnsFalse()
        {
            Assert.IsFalse(oneInchMeasurement.Equals(twoInchMeasurement));
        }

        [TestMethod]
        public void InchesEquality_NullComparison_ReturnsFalse()
        {
            Assert.IsFalse(oneInchMeasurement.Equals(null));
        }

        [TestMethod]
        public void InchesEquality_DifferentClass_ReturnsFalse()
        {
            Assert.IsFalse(oneInchMeasurement.Equals("invalid"));
        }

        [TestMethod]
        public void InchesEquality_SameReference_ReturnsTrue()
        {
            Assert.IsTrue(oneInchMeasurement.Equals(oneInchMeasurement));
        }

        // ================= EDGE CASE TESTS =================

        [TestMethod]
        public void InchesEquality_ZeroValues_ReturnsTrue()
        {
            var anotherZero = new Inches(0.0);

            Assert.IsTrue(zeroInchMeasurement.Equals(anotherZero));
        }

        [TestMethod]
        public void InchesEquality_LargeValues_ReturnsTrue()
        {
            var anotherLarge = new Inches(1_000_000.0);

            Assert.IsTrue(largeInchMeasurement.Equals(anotherLarge));
        }

        [TestMethod]
        public void InchesEquality_SmallDecimalDifference_ReturnsTrue()
        {
            var first = new Inches(1.0000001);
            var second = new Inches(1.0000002);

            Assert.IsTrue(first.Equals(second));
        }

        // ================= EQUALITY CONTRACT TESTS =================

        [TestMethod]
        public void InchesEquality_SymmetricProperty()
        {
            var a = new Inches(5.0);
            var b = new Inches(5.0);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));
        }

        [TestMethod]
        public void InchesEquality_ConsistencyProperty()
        {
            var a = new Inches(3.0);
            var b = new Inches(3.0);

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(a.Equals(b));
            }
        }
    }
}
