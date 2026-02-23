using NUnit.Framework;
using QuantityMeasurementApp.Domain;
using QuantityMeasurementApp.ServiceLayer;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class QuantityLengthUC4Tests
    {
        // --------------------- Yard Tests ---------------------

        [Test]
        public void testEquality_YardToYard_SameValue()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength b = new QuantityLength(1.0, LengthUnit.Yard);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void testEquality_YardToYard_DifferentValue()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength b = new QuantityLength(2.0, LengthUnit.Yard);
            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void testEquality_YardToFeet_EquivalentValue()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength b = new QuantityLength(3.0, LengthUnit.Feet);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void testEquality_FeetToYard_EquivalentValue()
        {
            QuantityLength a = new QuantityLength(3.0, LengthUnit.Feet);
            QuantityLength b = new QuantityLength(1.0, LengthUnit.Yard);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void testEquality_YardToInches_EquivalentValue()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength b = new QuantityLength(36.0, LengthUnit.Inch);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void testEquality_InchesToYard_EquivalentValue()
        {
            QuantityLength a = new QuantityLength(36.0, LengthUnit.Inch);
            QuantityLength b = new QuantityLength(1.0, LengthUnit.Yard);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void testEquality_YardToFeet_NonEquivalentValue()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength b = new QuantityLength(2.0, LengthUnit.Feet);
            Assert.IsFalse(a.Equals(b));
        }

        // --------------------- Centimeter Tests ---------------------

        [Test]
        public void testEquality_centimetersToInches_EquivalentValue()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Centimeter);
            QuantityLength b = new QuantityLength(0.393701, LengthUnit.Inch);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void testEquality_centimetersToFeet_NonEquivalentValue()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Centimeter);
            QuantityLength b = new QuantityLength(1.0, LengthUnit.Feet);
            Assert.IsFalse(a.Equals(b));
        }

        // --------------------- Transitive Property ---------------------

        [Test]
        public void testEquality_MultiUnit_TransitiveProperty()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength b = new QuantityLength(3.0, LengthUnit.Feet);
            QuantityLength c = new QuantityLength(36.0, LengthUnit.Inch);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        // --------------------- Null / Reference Tests ---------------------

        [Test]
        public void testEquality_YardWithNullUnit()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            QuantityLength? b = null;
            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void testEquality_YardSameReference()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Yard);
            Assert.IsTrue(a.Equals(a));
        }

        [Test]
        public void testEquality_YardNullComparison()
        {
            QuantityLength? a = null;
            QuantityLength b = new QuantityLength(1.0, LengthUnit.Yard);
            Assert.IsFalse(b.Equals(a));
        }

        [Test]
        public void testEquality_CentimetersWithNullUnit()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Centimeter);
            QuantityLength? b = null;
            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void testEquality_CentimetersSameReference()
        {
            QuantityLength a = new QuantityLength(1.0, LengthUnit.Centimeter);
            Assert.IsTrue(a.Equals(a));
        }

        [Test]
        public void testEquality_CentimetersNullComparison()
        {
            QuantityLength? a = null;
            QuantityLength b = new QuantityLength(1.0, LengthUnit.Centimeter);
            Assert.IsFalse(b.Equals(a));
        }

        // --------------------- Complex Scenario ---------------------

        [Test]
        public void testEquality_AllUnits_ComplexScenario()
        {
            QuantityLength a = new QuantityLength(2.0, LengthUnit.Yard);
            QuantityLength b = new QuantityLength(6.0, LengthUnit.Feet);
            QuantityLength c = new QuantityLength(72.0, LengthUnit.Inch);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        // --------------------- UC4 Service Tests ---------------------

        [Test]
        public void YardAndCentimeter_EqualValues_ShouldReturnTrue_Service()
        {
            bool result = QuantityLengthService.Compare(1.0, LengthUnit.Yard, 91.44, LengthUnit.Centimeter);
            Assert.IsTrue(result);
        }

        [Test]
        public void YardAndCentimeter_NotEqualValues_ShouldReturnFalse_Service()
        {
            bool result = QuantityLengthService.Compare(1.0, LengthUnit.Yard, 90, LengthUnit.Centimeter);
            Assert.IsFalse(result);
        }
    }
}