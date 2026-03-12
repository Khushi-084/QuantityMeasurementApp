using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel;
using QuantityMeasurementRepository;
using QuantityMeasurementBusinessLayer;

namespace QuantityMeasurementApp.Tests
{
    // ════════════════════════════════════════════════════════════════════════
    // Mock helpers for layer-isolation tests
    // ════════════════════════════════════════════════════════════════════════

    internal class InMemoryRepository : IQuantityMeasurementRepository
    {
        private readonly List<QuantityMeasurementEntity> _store = new();

        public void Save(QuantityMeasurementEntity entity) => _store.Add(entity);

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
            => _store.AsReadOnly();

        public void Clear() => _store.Clear();

        public int Count => _store.Count;
    }

    internal class MockService : IQuantityMeasurementService
    {
        public QuantityDTO? LastQ1      { get; private set; }
        public QuantityDTO? LastQ2      { get; private set; }
        public bool         ShouldThrow { get; set; } = false;

        public QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2)
        {
            LastQ1 = q1; LastQ2 = q2;
            if (ShouldThrow) throw new QuantityMeasurementException("Mock error");
            return new QuantityDTO(1, "EQUAL", "RESULT");
        }

        public QuantityDTO Convert(QuantityDTO q1, QuantityDTO target)
        {
            LastQ1 = q1; LastQ2 = target;
            if (ShouldThrow) throw new QuantityMeasurementException("Mock error");
            return new QuantityDTO(12, "INCHES", "LENGTH");
        }

        public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2)
        {
            LastQ1 = q1; LastQ2 = q2;
            if (ShouldThrow) throw new QuantityMeasurementException("Mock error");
            return new QuantityDTO(3, "FEET", "LENGTH");
        }

        public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2)
        {
            LastQ1 = q1; LastQ2 = q2;
            if (ShouldThrow) throw new QuantityMeasurementException("Mock error");
            return new QuantityDTO(1, "FEET", "LENGTH");
        }

        public QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2)
        {
            LastQ1 = q1; LastQ2 = q2;
            if (ShouldThrow) throw new QuantityMeasurementException("Mock error");
            return new QuantityDTO(2, "RATIO", "SCALAR");
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // UC15 — N-Tier Architecture Tests
    // ════════════════════════════════════════════════════════════════════════

    [TestClass]
    public class UC15_NTierArchitectureTests
    {
        private static IQuantityMeasurementService CreateService()
            => new QuantityMeasurementServiceImpl(new InMemoryRepository());

        private static (IQuantityMeasurementService svc,
                        IQuantityMeasurementRepository repo)
            CreateServiceWithRepo()
        {
            var repo = new InMemoryRepository();
            return (new QuantityMeasurementServiceImpl(repo), repo);
        }

        // ════════════════════════════════════════════════════════════════════
        // 1. ENTITY LAYER TESTS
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testQuantityEntity_SingleOperandConstruction()
        {
            var operand = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var result  = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var entity = new QuantityMeasurementEntity("CONVERT", operand, result);

            Assert.AreEqual("CONVERT", entity.OperationType);
            Assert.AreEqual(operand,   entity.Operand1);
            Assert.AreEqual(result,    entity.Result);
            Assert.IsNull(entity.Operand2);
            Assert.IsFalse(entity.HasError);
        }

        [TestMethod]
        public void testQuantityEntity_BinaryOperandConstruction()
        {
            var op1    = new QuantityDTO(1.0, "FEET", "LENGTH");
            var op2    = new QuantityDTO(2.0, "FEET", "LENGTH");
            var result = new QuantityDTO(3.0, "FEET", "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", op1, op2, result);

            Assert.AreEqual("ADD",  entity.OperationType);
            Assert.AreEqual(op1,    entity.Operand1);
            Assert.AreEqual(op2,    entity.Operand2);
            Assert.AreEqual(result, entity.Result);
            Assert.IsFalse(entity.HasError);
        }

        [TestMethod]
        public void testQuantityEntity_ErrorConstruction()
        {
            var op1 = new QuantityDTO(1.0, "CELSIUS", "TEMPERATURE");
            var op2 = new QuantityDTO(2.0, "CELSIUS", "TEMPERATURE");

            var entity = new QuantityMeasurementEntity(
                "ADD", op1, op2, "Temperature does not support Add.");

            Assert.IsTrue(entity.HasError);
            Assert.AreEqual("Temperature does not support Add.", entity.ErrorMessage);
            Assert.IsNull(entity.Result);
        }

        [TestMethod]
        public void testQuantityEntity_ToString_Success()
        {
            var op1    = new QuantityDTO(1.0, "FEET", "LENGTH");
            var op2    = new QuantityDTO(2.0, "FEET", "LENGTH");
            var result = new QuantityDTO(3.0, "FEET", "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", op1, op2, result);
            string text = entity.ToString();

            Assert.Contains("ADD",  text);
            Assert.Contains("FEET", text);
            Assert.Contains("3",    text);
        }

        [TestMethod]
        public void testQuantityEntity_ToString_Error()
        {
            var op1 = new QuantityDTO(1.0, "CELSIUS", "TEMPERATURE");
            var op2 = new QuantityDTO(2.0, "CELSIUS", "TEMPERATURE");

            var entity = new QuantityMeasurementEntity(
                "ADD", op1, op2, "Temperature does not support Add.");

            string text = entity.ToString();

            Assert.Contains("Error",                              text);
            Assert.Contains("Temperature does not support Add.", text);
        }

        [TestMethod]
        public void testEntity_Immutability()
        {
            var op1    = new QuantityDTO(1.0, "FEET", "LENGTH");
            var op2    = new QuantityDTO(2.0, "FEET", "LENGTH");
            var result = new QuantityDTO(3.0, "FEET", "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", op1, op2, result);

            Assert.AreEqual(op1,    entity.Operand1);
            Assert.AreEqual(op2,    entity.Operand2);
            Assert.AreEqual(result, entity.Result);
            Assert.AreEqual("ADD",  entity.OperationType);
        }

        [TestMethod]
        public void testEntity_OperationType_Tracking()
        {
            var op     = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var result = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var convertEntity = new QuantityMeasurementEntity("CONVERT", op, result);
            var errorEntity   = new QuantityMeasurementEntity("ADD", op, op, "err");

            Assert.AreEqual("CONVERT", convertEntity.OperationType);
            Assert.AreEqual("ADD",     errorEntity.OperationType);
        }

        // ════════════════════════════════════════════════════════════════════
        // 2. SERVICE LAYER TESTS
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testService_CompareEquality_SameUnit_Success()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2  = new QuantityDTO(1.0, "FEET", "LENGTH");

            var result = svc.Compare(q1, q2);

            Assert.AreEqual(1.0,     result.Value);
            Assert.AreEqual("EQUAL", result.UnitName);
        }

        [TestMethod]
        public void testService_CompareEquality_DifferentUnit_Success()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var q2  = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var result = svc.Compare(q1, q2);

            Assert.AreEqual(1.0, result.Value, "1 foot should equal 12 inches");
        }

        [TestMethod]
        public void testService_CompareEquality_DifferentValues_NotEqual()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var q2  = new QuantityDTO(5.0, "INCHES", "LENGTH");

            var result = svc.Compare(q1, q2);

            Assert.AreEqual(0.0, result.Value, "1 foot should NOT equal 5 inches");
        }

        [TestMethod]
        public void testService_CompareEquality_CrossCategory_Error()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var q2  = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            Assert.ThrowsExactly<QuantityMeasurementException>(() => svc.Compare(q1, q2));
        }

        [TestMethod]
        public void testService_Convert_Success()
        {
            var svc    = CreateService();
            var q1     = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var target = new QuantityDTO(0,   "INCHES", "LENGTH");

            var result = svc.Convert(q1, target);

            Assert.AreEqual(12.0,     result.Value, 0.01);
            Assert.AreEqual("INCHES", result.UnitName);
        }

        [TestMethod]
        public void testService_Add_Success()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2  = new QuantityDTO(2.0, "FEET", "LENGTH");

            var result = svc.Add(q1, q2);

            Assert.AreEqual(3.0,    result.Value, 0.01);
            Assert.AreEqual("FEET", result.UnitName);
        }

        [TestMethod]
        public void testService_Add_UnsupportedOperation_Error()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var q2  = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            Assert.ThrowsExactly<QuantityMeasurementException>(() => svc.Add(q1, q2));
        }

        [TestMethod]
        public void testService_Subtract_Success()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(3.0, "FEET", "LENGTH");
            var q2  = new QuantityDTO(1.0, "FEET", "LENGTH");

            var result = svc.Subtract(q1, q2);

            Assert.AreEqual(2.0,    result.Value, 0.01);
            Assert.AreEqual("FEET", result.UnitName);
        }

        [TestMethod]
        public void testService_Divide_Success()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(4.0, "FEET", "LENGTH");
            var q2  = new QuantityDTO(2.0, "FEET", "LENGTH");

            var result = svc.Divide(q1, q2);

            Assert.AreEqual(2.0,      result.Value, 0.01);
            Assert.AreEqual("SCALAR", result.Category);
        }

        [TestMethod]
        public void testService_Divide_ByZero_Error()
        {
            var svc = CreateService();
            var q1  = new QuantityDTO(4.0, "FEET", "LENGTH");
            var q2  = new QuantityDTO(0.0, "FEET", "LENGTH");

            Assert.ThrowsExactly<QuantityMeasurementException>(() => svc.Divide(q1, q2));
        }

        [TestMethod]
        public void testService_NullEntity_Rejection()
        {
            var svc = CreateService();
            var arg = new QuantityDTO(1.0, "FEET", "LENGTH");

            Assert.ThrowsExactly<QuantityMeasurementException>(() => svc.Compare(null!, arg));
        }

        [TestMethod]
        public void testService_AllMeasurementCategories()
        {
            var svc = CreateService();

            var lr = svc.Add(new QuantityDTO(1.0, "FEET",     "LENGTH"),
                             new QuantityDTO(1.0, "FEET",     "LENGTH"));
            Assert.AreEqual("LENGTH", lr.Category);

            var wr = svc.Add(new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                             new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"));
            Assert.AreEqual("WEIGHT", wr.Category);

            var vr = svc.Add(new QuantityDTO(1.0, "LITRE",    "VOLUME"),
                             new QuantityDTO(1.0, "LITRE",    "VOLUME"));
            Assert.AreEqual("VOLUME", vr.Category);

            var tr = svc.Compare(new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE"),
                                 new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE"));
            Assert.AreEqual("RESULT", tr.Category);
        }

        [TestMethod]
        public void testService_ValidationConsistency()
        {
            var svc = CreateService();
            var len = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var wt  = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            int errorCount = 0;

            try { svc.Compare(len, wt);  } catch (QuantityMeasurementException) { errorCount++; }
            try { svc.Add(len, wt);      } catch (QuantityMeasurementException) { errorCount++; }
            try { svc.Subtract(len, wt); } catch (QuantityMeasurementException) { errorCount++; }
            try { svc.Divide(len, wt);   } catch (QuantityMeasurementException) { errorCount++; }

            Assert.AreEqual(4, errorCount,
                "All 4 operations must reject cross-category input consistently");
        }

        [TestMethod]
        public void testService_ExceptionHandling_AllOperations()
        {
            var svc = CreateService();
            var c1  = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var c2  = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            int exCount = 0;

            try { svc.Add(c1, c2);      } catch (QuantityMeasurementException) { exCount++; }
            try { svc.Subtract(c1, c2); } catch (QuantityMeasurementException) { exCount++; }
            try { svc.Divide(c1, c2);   } catch (QuantityMeasurementException) { exCount++; }

            Assert.AreEqual(3, exCount,
                "Temperature arithmetic must throw QuantityMeasurementException for all 3 ops");
        }

        [TestMethod]
        public void testService_AllUnitImplementations()
        {
            var svc = CreateService();

            svc.Convert(new QuantityDTO(1,   "FEET",        "LENGTH"),
                        new QuantityDTO(0,   "INCHES",      "LENGTH"));
            svc.Convert(new QuantityDTO(1,   "YARDS",       "LENGTH"),
                        new QuantityDTO(0,   "CENTIMETERS", "LENGTH"));
            svc.Convert(new QuantityDTO(1,   "KILOGRAM",    "WEIGHT"),
                        new QuantityDTO(0,   "GRAM",        "WEIGHT"));
            svc.Convert(new QuantityDTO(1,   "POUND",       "WEIGHT"),
                        new QuantityDTO(0,   "KILOGRAM",    "WEIGHT"));
            svc.Convert(new QuantityDTO(1,   "LITRE",       "VOLUME"),
                        new QuantityDTO(0,   "MILLILITRE",  "VOLUME"));
            svc.Convert(new QuantityDTO(1,   "GALLON",      "VOLUME"),
                        new QuantityDTO(0,   "LITRE",       "VOLUME"));
            svc.Convert(new QuantityDTO(100, "CELSIUS",     "TEMPERATURE"),
                        new QuantityDTO(0,   "FAHRENHEIT",  "TEMPERATURE"));
            svc.Convert(new QuantityDTO(100, "CELSIUS",     "TEMPERATURE"),
                        new QuantityDTO(0,   "KELVIN",      "TEMPERATURE"));
        }

        // ════════════════════════════════════════════════════════════════════
        // 3. CONTROLLER LAYER TESTS
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testController_DemonstrateEquality_Success()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            string output = ctrl.PerformComparison(q1, q2);

            Assert.Contains("true", output,
                "Controller should display 'true' for equal quantities");
            Assert.AreEqual(q1, mockSvc.LastQ1);
            Assert.AreEqual(q2, mockSvc.LastQ2);
        }

        [TestMethod]
        public void testController_DemonstrateConversion_Success()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1     = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var target = new QuantityDTO(0,   "INCHES", "LENGTH");

            string output = ctrl.PerformConversion(q1, target);

            Assert.Contains("Conversion Result", output);
            Assert.DoesNotContain("[ERROR]",      output);
        }

        [TestMethod]
        public void testController_DemonstrateAddition_Success()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(2.0, "FEET", "LENGTH");

            string output = ctrl.PerformAddition(q1, q2);

            Assert.Contains("Addition Result", output);
            Assert.DoesNotContain("[ERROR]",   output);
        }

        [TestMethod]
        public void testController_DemonstrateAddition_Error()
        {
            var mockSvc = new MockService { ShouldThrow = true };
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1 = new QuantityDTO(1.0, "CELSIUS", "TEMPERATURE");
            var q2 = new QuantityDTO(1.0, "CELSIUS", "TEMPERATURE");

            string output = ctrl.PerformAddition(q1, q2);

            Assert.Contains("[ERROR]", output,
                "Controller should display error when service throws");
        }

        [TestMethod]
        public void testController_DisplayResult_Success()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1 = new QuantityDTO(3.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            string output = ctrl.PerformSubtraction(q1, q2);

            Assert.Contains("Subtraction Result", output);
            Assert.DoesNotContain("[ERROR]",       output);
        }

        [TestMethod]
        public void testController_DisplayResult_Error()
        {
            var mockSvc = new MockService { ShouldThrow = true };
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1 = new QuantityDTO(4.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(2.0, "FEET", "LENGTH");

            string output = ctrl.PerformDivision(q1, q2);

            Assert.Contains("[ERROR]", output,
                "Error message must be visible in controller output");
        }

        [TestMethod]
        public void testController_NullService_Prevention()
        {
            var repo = new InMemoryRepository();

            Assert.ThrowsExactly<ArgumentNullException>(
                () => new QuantityMeasurementController(null!, repo));
        }

        [TestMethod]
        public void testController_AllOperations()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1 = new QuantityDTO(4.0, "FEET",   "LENGTH");
            var q2 = new QuantityDTO(2.0, "FEET",   "LENGTH");
            var tg = new QuantityDTO(0,   "INCHES", "LENGTH");

            Assert.DoesNotContain("[ERROR]", ctrl.PerformComparison(q1, q2),  "Compare should succeed");
            Assert.DoesNotContain("[ERROR]", ctrl.PerformConversion(q1, tg),  "Convert should succeed");
            Assert.DoesNotContain("[ERROR]", ctrl.PerformAddition(q1, q2),    "Add should succeed");
            Assert.DoesNotContain("[ERROR]", ctrl.PerformSubtraction(q1, q2), "Subtract should succeed");
            Assert.DoesNotContain("[ERROR]", ctrl.PerformDivision(q1, q2),    "Divide should succeed");
        }

        // ════════════════════════════════════════════════════════════════════
        // 4. LAYER SEPARATION TESTS
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testLayerSeparation_ServiceIndependence()
        {
            var svc    = CreateService();
            var result = svc.Add(new QuantityDTO(1.0, "FEET", "LENGTH"),
                                 new QuantityDTO(2.0, "FEET", "LENGTH"));

            Assert.AreEqual(3.0, result.Value, 0.01,
                "Service should work independently without controller");
        }

        [TestMethod]
        public void testLayerSeparation_ControllerIndependence()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            string output = ctrl.PerformAddition(new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                 new QuantityDTO(2.0, "FEET", "LENGTH"));

            Assert.DoesNotContain("[ERROR]", output,
                "Controller should work with any service implementation");
        }

        [TestMethod]
        public void testLayerDecoupling_ServiceChange()
        {
            var repo  = new InMemoryRepository();
            var ctrl1 = new QuantityMeasurementController(new MockService(), repo);
            var ctrl2 = new QuantityMeasurementController(
                            new QuantityMeasurementServiceImpl(repo), repo);

            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            Assert.DoesNotContain("[ERROR]", ctrl1.PerformComparison(q1, q2));
            Assert.DoesNotContain("[ERROR]", ctrl2.PerformComparison(q1, q2));
        }

        [TestMethod]
        public void testLayerDecoupling_EntityChange()
        {
            var op1    = new QuantityDTO(1.0, "FEET", "LENGTH");
            var op2    = new QuantityDTO(2.0, "FEET", "LENGTH");
            var result = new QuantityDTO(3.0, "FEET", "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", op1, op2, result);

            Assert.IsNotNull(entity.Operand1);
            Assert.IsNotNull(entity.Operand2);
            Assert.IsNotNull(entity.Result);
            Assert.AreEqual("ADD", entity.OperationType);
        }

        // ════════════════════════════════════════════════════════════════════
        // 5. DATA FLOW TESTS
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testDataFlow_ControllerToService()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            var q1 = new QuantityDTO(5.0, "KILOGRAM", "WEIGHT");
            var q2 = new QuantityDTO(3.0, "GRAM",     "WEIGHT");

            ctrl.PerformAddition(q1, q2);

            Assert.AreEqual(5.0,        mockSvc.LastQ1?.Value);
            Assert.AreEqual("KILOGRAM", mockSvc.LastQ1?.UnitName);
            Assert.AreEqual(3.0,        mockSvc.LastQ2?.Value);
            Assert.AreEqual("GRAM",     mockSvc.LastQ2?.UnitName);
        }

        [TestMethod]
        public void testDataFlow_ServiceToController()
        {
            var mockSvc = new MockService();
            var repo    = new InMemoryRepository();
            var ctrl    = new QuantityMeasurementController(mockSvc, repo);

            string output = ctrl.PerformAddition(new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                 new QuantityDTO(2.0, "FEET", "LENGTH"));

            Assert.Contains("3",    output, "Result value must appear in output");
            Assert.Contains("FEET", output, "Result unit must appear in output");
        }

        // ════════════════════════════════════════════════════════════════════
        // 6. REPOSITORY TESTS
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testRepository_SaveAndRetrieve()
        {
            var repo   = new InMemoryRepository();
            var entity = new QuantityMeasurementEntity(
                "CONVERT",
                new QuantityDTO(1.0,  "FEET",   "LENGTH"),
                new QuantityDTO(12.0, "INCHES", "LENGTH"));

            repo.Save(entity);

            var all = repo.GetAllMeasurements();
            Assert.HasCount(1, all);
            Assert.AreEqual("CONVERT", all[0].OperationType);
        }

        [TestMethod]
        public void testRepository_ServiceSavesOnEachOperation()
        {
            var (svc, repo) = CreateServiceWithRepo();

            svc.Compare(new QuantityDTO(1.0, "FEET",   "LENGTH"),
                        new QuantityDTO(1.0, "FEET",   "LENGTH"));
            svc.Convert(new QuantityDTO(1.0, "FEET",   "LENGTH"),
                        new QuantityDTO(0,   "INCHES", "LENGTH"));
            svc.Add(    new QuantityDTO(1.0, "FEET",   "LENGTH"),
                        new QuantityDTO(2.0, "FEET",   "LENGTH"));

            Assert.HasCount(3, repo.GetAllMeasurements(),
                "Repository should have one entry per operation");
        }

        [TestMethod]
        public void testRepository_SavesErrorEntities()
        {
            var (svc, repo) = CreateServiceWithRepo();

            try
            {
                svc.Add(new QuantityDTO(100, "CELSIUS", "TEMPERATURE"),
                        new QuantityDTO(50,  "CELSIUS", "TEMPERATURE"));
            }
            catch (QuantityMeasurementException) { /* expected */ }

            var all = repo.GetAllMeasurements();
            Assert.HasCount(1, all);
            Assert.IsTrue(all[0].HasError,
                "Error operations must also be persisted in repository");
        }

        // ════════════════════════════════════════════════════════════════════
        // 7. INTEGRATION TESTS
        // ════════════════════════════════════════════════════════════════════

        [TestMethod]
        public void testIntegration_EndToEnd_LengthAddition()
        {
            var repo = new InMemoryRepository();
            var svc  = new QuantityMeasurementServiceImpl(repo);
            var ctrl = new QuantityMeasurementController(svc, repo);

            string output = ctrl.PerformAddition(new QuantityDTO(1.0, "FEET", "LENGTH"),
                                                 new QuantityDTO(2.0, "FEET", "LENGTH"));

            Assert.DoesNotContain("[ERROR]", output, "Should succeed");
            Assert.Contains("3", output, "Sum should be 3");
            Assert.HasCount(1, repo.GetAllMeasurements(),
                "Operation should be recorded in repository");
        }

        [TestMethod]
        public void testIntegration_EndToEnd_TemperatureUnsupported()
        {
            var repo = new InMemoryRepository();
            var svc  = new QuantityMeasurementServiceImpl(repo);
            var ctrl = new QuantityMeasurementController(svc, repo);

            string output = ctrl.PerformAddition(
                new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE"),
                new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE"));

            Assert.Contains("[ERROR]", output,
                "Temperature addition must return error to user");
            Assert.HasCount(1, repo.GetAllMeasurements(),
                "Error operation must still be recorded in repository");
            Assert.IsTrue(repo.GetAllMeasurements()[0].HasError,
                "Stored entity must have HasError = true");
        }

        [TestMethod]
        public void testIntegration_EndToEnd_WeightConversion()
        {
            var repo = new InMemoryRepository();
            var svc  = new QuantityMeasurementServiceImpl(repo);
            var ctrl = new QuantityMeasurementController(svc, repo);

            string output = ctrl.PerformConversion(
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(0,   "GRAM",     "WEIGHT"));

            Assert.Contains("1000", output, "1 kg = 1000 g");
            Assert.DoesNotContain("[ERROR]", output);
        }

        [TestMethod]
        public void testIntegration_EndToEnd_TemperatureConversion()
        {
            var repo = new InMemoryRepository();
            var svc  = new QuantityMeasurementServiceImpl(repo);
            var ctrl = new QuantityMeasurementController(svc, repo);

            string output = ctrl.PerformConversion(
                new QuantityDTO(0.0, "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(0,   "FAHRENHEIT", "TEMPERATURE"));

            Assert.Contains("32", output, "0 C = 32 F");
            Assert.DoesNotContain("[ERROR]", output);
        }

        [TestMethod]
        public void testScalability_NewOperation_Addition()
        {
            var svc = CreateService();

            var compareResult = svc.Compare(new QuantityDTO(12.0, "INCHES", "LENGTH"),
                                            new QuantityDTO(1.0,  "FEET",   "LENGTH"));
            var addResult     = svc.Add(    new QuantityDTO(1.0,  "FEET",   "LENGTH"),
                                            new QuantityDTO(1.0,  "FEET",   "LENGTH"));
            var divResult     = svc.Divide( new QuantityDTO(4.0,  "FEET",   "LENGTH"),
                                            new QuantityDTO(2.0,  "FEET",   "LENGTH"));

            Assert.AreEqual(1.0, compareResult.Value,       "Compare still works");
            Assert.AreEqual(2.0, addResult.Value,     0.01, "Add still works");
            Assert.AreEqual(2.0, divResult.Value,     0.01, "Divide still works");
        }

        [TestMethod]
        public void testBackwardCompatibility_AllUC1_UC14_Tests()
        {
            var svc = CreateService();

            Assert.AreEqual(1.0, svc.Compare(
                new QuantityDTO(1.0,  "FEET",   "LENGTH"),
                new QuantityDTO(12.0, "INCHES", "LENGTH")).Value, "UC1-UC5 length compare");

            Assert.AreEqual(2.0, svc.Add(
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH")).Value, 0.01, "UC6 addition");

            Assert.AreEqual(1.0, svc.Subtract(
                new QuantityDTO(2.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH")).Value, 0.01, "UC7 subtraction");

            Assert.AreEqual(2.0, svc.Divide(
                new QuantityDTO(4.0, "FEET", "LENGTH"),
                new QuantityDTO(2.0, "FEET", "LENGTH")).Value, 0.01, "UC8 division");

            Assert.AreEqual(1000.0, svc.Convert(
                new QuantityDTO(1.0, "KILOGRAM",  "WEIGHT"),
                new QuantityDTO(0,   "GRAM",      "WEIGHT")).Value, 0.01, "UC9 weight");

            Assert.AreEqual(1000.0, svc.Convert(
                new QuantityDTO(1.0, "LITRE",     "VOLUME"),
                new QuantityDTO(0,   "MILLILITRE", "VOLUME")).Value, 0.01, "UC11 volume");

            Assert.AreEqual(32.0, svc.Convert(
                new QuantityDTO(0.0, "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(0,   "FAHRENHEIT", "TEMPERATURE")).Value, 0.01, "UC14 temperature");
        }
    }
}