using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC4 - Extended Unit Support
    /// Tests Yards and Centimeters added to QuantityLength.
    /// Validates equality, conversions, symmetry,
    /// transitive property and null safety.
    /// </summary>
    [TestClass]
    public class UC4_ExtendedUnitSupportTests
    {
        // ================= 1 =================
        [TestMethod]
        public void testEquality_YardToYard_SameValue()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.YARD);
            var q2 = new QuantityLength(1.0, LengthUnit.YARD);

            Assert.IsTrue(q1.Equals(q2));
        }

        // ================= 2 =================
        [TestMethod]
        public void testEquality_YardToYard_DifferentValue()
        {
            var q1 = new QuantityLength(1.0, LengthUnit.YARD);
            var q2 = new QuantityLength(2.0, LengthUnit.YARD);

            Assert.IsFalse(q1.Equals(q2));
        }

        // ================= 3 =================
        [TestMethod]
        public void testEquality_YardToFeet_EquivalentValue()
        {
            var yard = new QuantityLength(1.0, LengthUnit.YARD);
            var feet = new QuantityLength(3.0, LengthUnit.FEET);

            Assert.IsTrue(yard.Equals(feet));
        }

        // ================= 4 =================
        [TestMethod]
        public void testEquality_FeetToYard_EquivalentValue()
        {
            var feet = new QuantityLength(3.0, LengthUnit.FEET);
            var yard = new QuantityLength(1.0, LengthUnit.YARD);

            Assert.IsTrue(feet.Equals(yard));
        }

        // ================= 5 =================
        [TestMethod]
        public void testEquality_YardToInches_EquivalentValue()
        {
            var yard = new QuantityLength(1.0, LengthUnit.YARD);
            var inches = new QuantityLength(36.0, LengthUnit.INCH);

            Assert.IsTrue(yard.Equals(inches));
        }

        // ================= 6 =================
        [TestMethod]
        public void testEquality_InchesToYard_EquivalentValue()
        {
            var inches = new QuantityLength(36.0, LengthUnit.INCH);
            var yard = new QuantityLength(1.0, LengthUnit.YARD);

            Assert.IsTrue(inches.Equals(yard));
        }

        // ================= 7 =================
        [TestMethod]
        public void testEquality_YardToFeet_NonEquivalentValue()
        {
            var yard = new QuantityLength(1.0, LengthUnit.YARD);
            var feet = new QuantityLength(2.0, LengthUnit.FEET);

            Assert.IsFalse(yard.Equals(feet));
        }

        // ================= 8 =================
        [TestMethod]
        public void testEquality_centimetersToInches_EquivalentValue()
        {
            var cm = new QuantityLength(1.0, LengthUnit.CENTIMETER);
            var inch = new QuantityLength(0.393701, LengthUnit.INCH);

            Assert.IsTrue(cm.Equals(inch));
        }

        // ================= 9 =================
        [TestMethod]
        public void testEquality_centimetersToFeet_NonEquivalentValue()
        {
            var cm = new QuantityLength(1.0, LengthUnit.CENTIMETER);
            var feet = new QuantityLength(1.0, LengthUnit.FEET);

            Assert.IsFalse(cm.Equals(feet));
        }

        // ================= 10 =================
        [TestMethod]
        public void testEquality_MultiUnit_TransitiveProperty()
        {
            var yard = new QuantityLength(1.0, LengthUnit.YARD);
            var feet = new QuantityLength(3.0, LengthUnit.FEET);
            var inches = new QuantityLength(36.0, LengthUnit.INCH);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }

        // ================= 11 =================
        [TestMethod]
        public void testEquality_YardWithNullUnit()
        {
            Assert.Throws<System.ArgumentException>(() =>
            {
                new QuantityLength(1.0, (LengthUnit)(-1));
            });
        }

        // ================= 12 =================
        [TestMethod]
        public void testEquality_YardSameReference()
        {
            var yard = new QuantityLength(2.0, LengthUnit.YARD);

            Assert.IsTrue(yard.Equals(yard));
        }

        // ================= 13 =================
        [TestMethod]
        public void testEquality_YardNullComparison()
        {
            var yard = new QuantityLength(1.0, LengthUnit.YARD);

            Assert.IsFalse(yard.Equals(null));
        }

        // ================= 14 =================
        [TestMethod]
        public void testEquality_CentimetersWithNullUnit()
        {
            Assert.Throws<System.ArgumentException>(() =>
            {
                new QuantityLength(2.0, (LengthUnit)(-1));
            });
        }

        // ================= 15 =================
        [TestMethod]
        public void testEquality_CentimetersSameReference()
        {
            var cm = new QuantityLength(5.0, LengthUnit.CENTIMETER);

            Assert.IsTrue(cm.Equals(cm));
        }

        // ================= 16 =================
        [TestMethod]
        public void testEquality_CentimetersNullComparison()
        {
            var cm = new QuantityLength(1.0, LengthUnit.CENTIMETER);

            Assert.IsFalse(cm.Equals(null));
        }

        // ================= 17 =================
        [TestMethod]
        public void testEquality_AllUnits_ComplexScenario()
        {
            var yard = new QuantityLength(2.0, LengthUnit.YARD);
            var feet = new QuantityLength(6.0, LengthUnit.FEET);
            var inches = new QuantityLength(72.0, LengthUnit.INCH);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }
    }
}
