using QuantityMeasurementModel.Interfaces;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementModel;
using QuantityMeasurementRepository;

namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC15: Service implementation.
    /// UC15 Data Flow per operation:
    ///   1. Accept QuantityDTO
    ///   2. Extract QuantityModel   ← NEW STEP
    ///   3. Validate QuantityModel  ← NEW STEP
    ///   4. Perform business logic using QuantityModel
    ///   5. Handle exceptions → QuantityMeasurementException
    ///   6. Save QuantityMeasurementEntity to Repository
    ///   7. Return QuantityDTO result
    /// </summary>
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;

        // Constructor Injection (Dependency Injection)
        public QuantityMeasurementServiceImpl(
            IQuantityMeasurementRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        // ── COMPARE ──────────────────────────────────────────────────────

        public QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Compare");
            try
            {
                // Step 2: Extract QuantityModel
                // Step 3: Validate + Step 4: Business logic
                bool equal  = CompareUsingModel(q1, q2);
                var  result = new QuantityDTO(
                    equal ? 1 : 0,
                    equal ? "EQUAL" : "NOT_EQUAL",
                    "RESULT");

                // Step 6: Save to repository
                _repository.Save(
                    new QuantityMeasurementEntity("COMPARE", q1, q2, result));

                // Step 7: Return QuantityDTO
                return result;
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _repository.Save(
                    new QuantityMeasurementEntity("COMPARE", q1, q2, ex.Message));
                throw new QuantityMeasurementException(
                    $"Compare failed: {ex.Message}", ex);
            }
        }

        // ── CONVERT ──────────────────────────────────────────────────────

        public QuantityDTO Convert(QuantityDTO q1, QuantityDTO targetUnitDTO)
        {
            ValidateNotNull(q1, targetUnitDTO);
            try
            {
                // Step 2+3+4: Extract model, validate, perform logic
                var result = ConvertUsingModel(q1, targetUnitDTO.UnitName);

                // Step 6: Save
                _repository.Save(
                    new QuantityMeasurementEntity("CONVERT", q1, result));

                return result;
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _repository.Save(
                    new QuantityMeasurementEntity("CONVERT", q1, null, ex.Message));
                throw new QuantityMeasurementException(
                    $"Convert failed: {ex.Message}", ex);
            }
        }

        // ── ADD ──────────────────────────────────────────────────────────

        public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Add");
            try
            {
                var result = ArithmeticUsingModel(q1, q2, "ADD");
                _repository.Save(
                    new QuantityMeasurementEntity("ADD", q1, q2, result));
                return result;
            }
            catch (NotSupportedException)
            {
                string msg = "Temperature does not support Add.";
                _repository.Save(
                    new QuantityMeasurementEntity("ADD", q1, q2, msg));
                throw new QuantityMeasurementException(msg);
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _repository.Save(
                    new QuantityMeasurementEntity("ADD", q1, q2, ex.Message));
                throw new QuantityMeasurementException(
                    $"Add failed: {ex.Message}", ex);
            }
        }

        // ── SUBTRACT ─────────────────────────────────────────────────────

        public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Subtract");
            try
            {
                var result = ArithmeticUsingModel(q1, q2, "SUBTRACT");
                _repository.Save(
                    new QuantityMeasurementEntity("SUBTRACT", q1, q2, result));
                return result;
            }
            catch (NotSupportedException)
            {
                string msg = "Temperature does not support Subtract.";
                _repository.Save(
                    new QuantityMeasurementEntity("SUBTRACT", q1, q2, msg));
                throw new QuantityMeasurementException(msg);
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _repository.Save(
                    new QuantityMeasurementEntity("SUBTRACT", q1, q2, ex.Message));
                throw new QuantityMeasurementException(
                    $"Subtract failed: {ex.Message}", ex);
            }
        }

        // ── DIVIDE ───────────────────────────────────────────────────────

        public QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateNotNull(q1, q2);
            ValidateSameCategory(q1, q2, "Divide");
            try
            {
                var result = ArithmeticUsingModel(q1, q2, "DIVIDE");
                _repository.Save(
                    new QuantityMeasurementEntity("DIVIDE", q1, q2, result));
                return result;
            }
            catch (NotSupportedException)
            {
                string msg = "Temperature does not support Divide.";
                _repository.Save(
                    new QuantityMeasurementEntity("DIVIDE", q1, q2, msg));
                throw new QuantityMeasurementException(msg);
            }
            catch (Exception ex) when (ex is not QuantityMeasurementException)
            {
                _repository.Save(
                    new QuantityMeasurementEntity("DIVIDE", q1, q2, ex.Message));
                throw new QuantityMeasurementException(
                    $"Divide failed: {ex.Message}", ex);
            }
        }

        // ════════════════════════════════════════════════════════════════
        // PRIVATE HELPERS — use QuantityModel internally (UC15 Step 2+3+4)
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Step 2+3+4: Extract QuantityModel from DTO, validate, compare.
        /// QuantityModel is used INTERNALLY — never exposed outside service.
        /// </summary>
        private bool CompareUsingModel(QuantityDTO q1, QuantityDTO q2)
        {
            return q1.Category.ToUpperInvariant() switch
            {
                "LENGTH" =>
                    ModelEquals(
                        ToModel<LengthUnitM>(q1),
                        ToModel<LengthUnitM>(q2)),
                "WEIGHT" =>
                    ModelEquals(
                        ToModel<WeightUnitM>(q1),
                        ToModel<WeightUnitM>(q2)),
                "VOLUME" =>
                    ModelEquals(
                        ToModel<VolumeUnitM>(q1),
                        ToModel<VolumeUnitM>(q2)),
                "TEMPERATURE" =>
                    ModelEquals(
                        ToModel<TemperatureUnit>(q1),
                        ToModel<TemperatureUnit>(q2)),
                _ => throw new QuantityMeasurementException(
                        $"Unknown category: {q1.Category}")
            };
        }

        private QuantityDTO ConvertUsingModel(QuantityDTO q1, string targetUnit)
        {
            return q1.Category.ToUpperInvariant() switch
            {
                "LENGTH" =>
                    FromModel(
                        ToModel<LengthUnitM>(q1)
                            .ConvertTo(ResolveLengthUnit(targetUnit)),
                        "LENGTH"),
                "WEIGHT" =>
                    FromModel(
                        ToModel<WeightUnitM>(q1)
                            .ConvertTo(ResolveWeightUnit(targetUnit)),
                        "WEIGHT"),
                "VOLUME" =>
                    FromModel(
                        ToModel<VolumeUnitM>(q1)
                            .ConvertTo(ResolveVolumeUnit(targetUnit)),
                        "VOLUME"),
                "TEMPERATURE" =>
                    FromModel(
                        ToModel<TemperatureUnit>(q1)
                            .ConvertTo(ResolveTempUnit(targetUnit)),
                        "TEMPERATURE"),
                _ => throw new QuantityMeasurementException(
                        $"Unknown category: {q1.Category}")
            };
        }

        private QuantityDTO ArithmeticUsingModel(
            QuantityDTO q1, QuantityDTO q2, string op)
        {
            return q1.Category.ToUpperInvariant() switch
            {
                "LENGTH"      => ApplyLengthOp(q1, q2, op),
                "WEIGHT"      => ApplyWeightOp(q1, q2, op),
                "VOLUME"      => ApplyVolumeOp(q1, q2, op),
                "TEMPERATURE" =>
                    throw new NotSupportedException(
                        "Temperature arithmetic not supported."),
                _ => throw new QuantityMeasurementException(
                        $"Unknown category: {q1.Category}")
            };
        }

        // ── Per-category arithmetic helpers ──────────────────────────────

        private QuantityDTO ApplyLengthOp(
            QuantityDTO q1, QuantityDTO q2, string op)
        {
            // Step 2: Extract QuantityModel
            var modelA = ToModel<LengthUnitM>(q1);
            var modelB = ToModel<LengthUnitM>(q2);

            // Step 4: Business logic via Quantity<U>
            var qa = ToQuantity(modelA);
            var qb = ToQuantity(modelB);

            return op switch
            {
                "ADD"      => FromModel(
                                  ToModel(qa.Add(qb), modelA.Unit), "LENGTH"),
                "SUBTRACT" => FromModel(
                                  ToModel(qa.Subtract(qb), modelA.Unit), "LENGTH"),
                "DIVIDE"   => new QuantityDTO(qa.Divide(qb), "RATIO", "SCALAR"),
                _ => throw new InvalidOperationException($"Unknown op: {op}")
            };
        }

        private QuantityDTO ApplyWeightOp(
            QuantityDTO q1, QuantityDTO q2, string op)
        {
            var modelA = ToModel<WeightUnitM>(q1);
            var modelB = ToModel<WeightUnitM>(q2);
            var qa     = ToQuantity(modelA);
            var qb     = ToQuantity(modelB);

            return op switch
            {
                "ADD"      => FromModel(
                                  ToModel(qa.Add(qb), modelA.Unit), "WEIGHT"),
                "SUBTRACT" => FromModel(
                                  ToModel(qa.Subtract(qb), modelA.Unit), "WEIGHT"),
                "DIVIDE"   => new QuantityDTO(qa.Divide(qb), "RATIO", "SCALAR"),
                _ => throw new InvalidOperationException($"Unknown op: {op}")
            };
        }

        private QuantityDTO ApplyVolumeOp(
            QuantityDTO q1, QuantityDTO q2, string op)
        {
            var modelA = ToModel<VolumeUnitM>(q1);
            var modelB = ToModel<VolumeUnitM>(q2);
            var qa     = ToQuantity(modelA);
            var qb     = ToQuantity(modelB);

            return op switch
            {
                "ADD"      => FromModel(
                                  ToModel(qa.Add(qb), modelA.Unit), "VOLUME"),
                "SUBTRACT" => FromModel(
                                  ToModel(qa.Subtract(qb), modelA.Unit), "VOLUME"),
                "DIVIDE"   => new QuantityDTO(qa.Divide(qb), "RATIO", "SCALAR"),
                _ => throw new InvalidOperationException($"Unknown op: {op}")
            };
        }

        // ── QuantityModel ↔ Quantity<U> bridge ───────────────────────────

        /// <summary>
        /// Step 2: QuantityDTO → QuantityModel (internal representation).
        /// This is the key UC15 step that was missing before.
        /// </summary>
        private QuantityModel<U> ToModel<U>(QuantityDTO dto)
            where U : class, IMeasurable
        {
            ValidateValue(dto.Value, $"Value '{dto.Value}'");
            var unit = (U)ResolveUnit(dto.UnitName, dto.Category);
            return new QuantityModel<U>(dto.Value, unit);
        }

        /// <summary>QuantityModel → Quantity<U> for business logic.</summary>
        private Quantity<U> ToQuantity<U>(QuantityModel<U> model)
            where U : class, IMeasurable
            => new Quantity<U>(model.Value, model.Unit);

        /// <summary>Quantity<U> result → QuantityModel wrapper.</summary>
        private QuantityModel<U> ToModel<U>(Quantity<U> q, U unit)
            where U : class, IMeasurable
            => new QuantityModel<U>(q.Value, unit);

        /// <summary>Step 7: QuantityModel → QuantityDTO for return.</summary>
        private static QuantityDTO FromModel<U>(
            QuantityModel<U> model, string category)
            where U : class, IMeasurable
            => new QuantityDTO(model.Value, model.Unit.GetUnitName(), category);

        /// <summary>Compare two QuantityModels by base unit.</summary>
        private static bool ModelEquals<U>(
            QuantityModel<U> a, QuantityModel<U> b)
            where U : class, IMeasurable
            => Math.Abs(a.ToBaseUnit() - b.ToBaseUnit()) < 0.01;

        // ── Unit resolvers ────────────────────────────────────────────────

        private static IMeasurable ResolveUnit(string unitName, string category)
            => category.ToUpperInvariant() switch
            {
                "LENGTH"      => ResolveLengthUnit(unitName),
                "WEIGHT"      => ResolveWeightUnit(unitName),
                "VOLUME"      => ResolveVolumeUnit(unitName),
                "TEMPERATURE" => ResolveTempUnit(unitName),
                _ => throw new QuantityMeasurementException(
                        $"Unknown category: {category}")
            };

        private static LengthUnitM ResolveLengthUnit(string name)
            => name.ToUpperInvariant() switch
            {
                "FEET"        or "FT" or "F"  => LengthUnitM.FEET,
                "INCHES"      or "IN" or "I"  => LengthUnitM.INCHES,
                "YARDS"       or "YD" or "Y"  => LengthUnitM.YARDS,
                "CENTIMETERS" or "CM" or "C"  => LengthUnitM.CENTIMETERS,
                _ => throw new QuantityMeasurementException(
                        $"Unknown length unit: '{name}'. Use FEET/FT/F, INCHES/IN/I, YARDS/YD/Y, CENTIMETERS/CM/C")
            };

        private static WeightUnitM ResolveWeightUnit(string name)
            => name.ToUpperInvariant() switch
            {
                "KILOGRAM" or "KG" or "K" => WeightUnitM.KILOGRAM,
                "GRAM"     or "G"         => WeightUnitM.GRAM,
                "POUND"    or "LB" or "P" => WeightUnitM.POUND,
                _ => throw new QuantityMeasurementException(
                        $"Unknown weight unit: '{name}'. Use KILOGRAM/KG/K, GRAM/G, POUND/LB/P")
            };

        private static VolumeUnitM ResolveVolumeUnit(string name)
            => name.ToUpperInvariant() switch
            {
                "LITRE"      or "L"   => VolumeUnitM.LITRE,
                "MILLILITRE" or "ML"  => VolumeUnitM.MILLILITRE,
                "GALLON"     or "GAL" => VolumeUnitM.GALLON,
                _ => throw new QuantityMeasurementException(
                        $"Unknown volume unit: '{name}'. Use LITRE/L, MILLILITRE/ML, GALLON/GAL")
            };

        private static TemperatureUnit ResolveTempUnit(string name)
            => name.ToUpperInvariant() switch
            {
                "CELSIUS"    or "C"  => TemperatureUnit.CELSIUS,
                "FAHRENHEIT" or "F"  => TemperatureUnit.FAHRENHEIT,
                "KELVIN"     or "K"  => TemperatureUnit.KELVIN,
                _ => throw new QuantityMeasurementException(
                        $"Unknown temperature unit: '{name}'. Use CELSIUS/C, FAHRENHEIT/F, KELVIN/K")
            };

        // ── Validation helpers ────────────────────────────────────────────

        private static void ValidateValue(double value, string label = "Value")
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new QuantityMeasurementException(
                    $"{label} must be a valid number.");
            if (value < 0)
                throw new QuantityMeasurementException(
                    $"{label} cannot be negative. Got: {value}");
            if (value > 1_000_000)
                throw new QuantityMeasurementException(
                    $"{label} is too large (max 1,000,000). Got: {value}");
        }

        private static void ValidateNotNull(QuantityDTO? q1, QuantityDTO? q2)
        {
            if (q1 == null)
                throw new QuantityMeasurementException(
                    "First operand cannot be null.");
            if (q2 == null)
                throw new QuantityMeasurementException(
                    "Second operand cannot be null.");
        }

        private static void ValidateSameCategory(
            QuantityDTO q1, QuantityDTO q2, string operation)
        {
            if (!string.Equals(q1.Category, q2.Category,
                    StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException(
                    $"Cannot {operation} across different categories: " +
                    $"{q1.Category} and {q2.Category}.");
        }
    }
}