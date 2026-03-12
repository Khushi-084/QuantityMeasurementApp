using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
namespace QuantityMeasurementApp.Tests
{
    // UC1 - Feet Equality Tests
    // Validates equality contract, edge cases and robustness
    [TestClass]
    public class FeetEqualityTests
    {
        // ================= GLOBAL TEST OBJECTS =================

        // Common reusable objects for tests
        private Feet oneFootMeasurement;
        private Feet anotherOneFootMeasurement;
        private Feet twoFeetMeasurement;
        private Feet negativeTwoFeetMeasurement;
        private Feet zeroFeetMeasurement;
        private Feet largeFeetMeasurement;

        // Runs before EACH test method
        [TestInitialize]
        public void Setup()
        {
            oneFootMeasurement = new Feet(1.0);
            anotherOneFootMeasurement = new Feet(1.0);
            twoFeetMeasurement = new Feet(2.0);
            negativeTwoFeetMeasurement = new Feet(-2.0);
            zeroFeetMeasurement = new Feet(0.0);
            largeFeetMeasurement = new Feet(1_000_000.0);
        }

        // ================= BASIC EQUALITY TESTS =================

        [TestMethod]
        public void FeetEquality_SameValue_ReturnsTrue()
        {
            Assert.IsTrue(oneFootMeasurement.Equals(anotherOneFootMeasurement));
        }

        [TestMethod]
        public void FeetEquality_DifferentValue_ReturnsFalse()
        {
            Assert.IsFalse(oneFootMeasurement.Equals(twoFeetMeasurement));
        }

        [TestMethod]
        public void FeetEquality_NullComparison_ReturnsFalse()
        {
            Assert.IsFalse(oneFootMeasurement.Equals(null));
        }

        [TestMethod]
        public void FeetEquality_DifferentClass_ReturnsFalse()
        {
            Assert.IsFalse(oneFootMeasurement.Equals("invalid input"));
        }

        [TestMethod]
        public void FeetEquality_SameReference_ReturnsTrue()
        {
            Assert.IsTrue(oneFootMeasurement.Equals(oneFootMeasurement));
        }

        // ================= EDGE CASE TESTS =================

        [TestMethod]
        public void FeetEquality_NegativeValues_ReturnsTrue()
        {
            var anotherNegativeMeasurement = new Feet(-2.0);

            Assert.IsTrue(
                negativeTwoFeetMeasurement.Equals(anotherNegativeMeasurement));
        }

        [TestMethod]
        public void FeetEquality_ZeroValues_ReturnsTrue()
        {
            var anotherZeroMeasurement = new Feet(0.0);

            Assert.IsTrue(
                zeroFeetMeasurement.Equals(anotherZeroMeasurement));
        }

        [TestMethod]
        public void FeetEquality_LargeValues_ReturnsTrue()
        {
            var anotherLargeMeasurement = new Feet(1_000_000.0);

            Assert.IsTrue(
                largeFeetMeasurement.Equals(anotherLargeMeasurement));
        }

        [TestMethod]
        public void FeetEquality_SmallDecimalDifference_ReturnsTrue()
        {
            var firstMeasurement = new Feet(1.0000001);
            var secondMeasurement = new Feet(1.0000002);

            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
        }

        [TestMethod]
        public void FeetEquality_IntegerAndDoubleValue_ReturnsTrue()
        {
            var integerMeasurement = new Feet(1);
            var doubleMeasurement = new Feet(1.0);

            Assert.IsTrue(integerMeasurement.Equals(doubleMeasurement));
        }

        [TestMethod]
        public void FeetEquality_EmptyStringComparison_ReturnsFalse()
        {
            Assert.IsFalse(oneFootMeasurement.Equals(""));
        }

        // ================= EQUALITY CONTRACT TESTS =================

        [TestMethod]
        public void FeetEquality_SymmetricProperty()
        {
            var firstMeasurement = new Feet(2.0);
            var secondMeasurement = new Feet(2.0);

            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
            Assert.IsTrue(secondMeasurement.Equals(firstMeasurement));
        }

        [TestMethod]
        public void FeetEquality_TransitiveProperty()
        {
            var measurementA = new Feet(3.0);
            var measurementB = new Feet(3.0);
            var measurementC = new Feet(3.0);

            Assert.IsTrue(measurementA.Equals(measurementB));
            Assert.IsTrue(measurementB.Equals(measurementC));
            Assert.IsTrue(measurementA.Equals(measurementC));
        }

        [TestMethod]
        public void FeetEquality_ConsistencyProperty()
        {
            var firstMeasurement = new Feet(4.0);
            var secondMeasurement = new Feet(4.0);

            for (int iteration = 0; iteration < 5; iteration++)
            {
                Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
            }
        }
    }
}
