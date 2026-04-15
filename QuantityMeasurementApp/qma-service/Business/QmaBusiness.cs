// ─────────────────────────────────────────────────────────────────────────────
// QMA BUSINESS LAYER — Interface
// ─────────────────────────────────────────────────────────────────────────────

using ModelService.Qma.Dto;

namespace BusinessService.Qma.Interface
{
    public interface IQmaService
    {
        Task<QuantityMeasurementDTO> CompareAsync(QuantityInputDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> ConvertAsync(ConvertRequestDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> AddAsync(QuantityInputDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> SubtractAsync(QuantityInputDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> DivideAsync(QuantityInputDTO input, int? userId = null);

        Task<IReadOnlyList<QuantityMeasurementDTO>> GetAllHistoryAsync(int userId);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByOperationAsync(string operationType, int userId);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByCategoryAsync(string category, int userId);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetErrorHistoryAsync(int userId);
        Task<int> GetOperationCountAsync(string operationType, int userId);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// QMA BUSINESS LAYER — Exception
// ─────────────────────────────────────────────────────────────────────────────

namespace BusinessService.Qma.Exceptions
{
    public class QmaMeasurementException : Exception
    {
        public QmaMeasurementException(string message) : base(message) { }
        public QmaMeasurementException(string message, Exception inner) : base(message, inner) { }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// QMA BUSINESS LAYER — Core units (IMeasurable, unit classes, Quantity<U>)
// ─────────────────────────────────────────────────────────────────────────────

namespace BusinessService.Qma.Core
{
    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();
        bool   SupportsArithmetic() => true;
        void   ValidateOperationSupport(string operation) { }
    }

    public class LengthUnitM : IMeasurable
    {
        public static readonly LengthUnitM FEET        = new("FEET",        1.0);
        public static readonly LengthUnitM INCHES      = new("INCHES",      1.0 / 12.0);
        public static readonly LengthUnitM YARDS       = new("YARDS",       3.0);
        public static readonly LengthUnitM CENTIMETERS = new("CENTIMETERS", 1.0 / 30.48);

        private readonly string name;
        private readonly double factor;
        private LengthUnitM(string name, double factor) { this.name = name; this.factor = factor; }

        public double GetConversionFactor()           => factor;
        public double ConvertToBaseUnit(double value) => value * factor;
        public double ConvertFromBaseUnit(double bv)
        {
            if (Math.Abs(factor) < 1e-15) throw new ArgumentException("Unsupported unit");
            return bv / factor;
        }
        public string GetUnitName() => name;
        public override string ToString() => name;
    }

    public class WeightUnitM : IMeasurable
    {
        public static readonly WeightUnitM KILOGRAM = new("KILOGRAM", 1.0);
        public static readonly WeightUnitM GRAM     = new("GRAM",     0.001);
        public static readonly WeightUnitM POUND    = new("POUND",    0.453592);

        private readonly string name;
        private readonly double factor;
        private WeightUnitM(string name, double factor) { this.name = name; this.factor = factor; }

        public double GetConversionFactor()           => factor;
        public double ConvertToBaseUnit(double value) => value * factor;
        public double ConvertFromBaseUnit(double bv)
        {
            if (Math.Abs(factor) < 1e-15) throw new ArgumentException("Unsupported weight unit");
            return bv / factor;
        }
        public string GetUnitName() => name;
        public override string ToString() => name;
    }

    public class VolumeUnitM : IMeasurable
    {
        public static readonly VolumeUnitM LITRE      = new("LITRE",      1.0);
        public static readonly VolumeUnitM MILLILITRE = new("MILLILITRE", 0.001);
        public static readonly VolumeUnitM GALLON     = new("GALLON",     3.78541);

        private readonly string name;
        private readonly double factor;
        private VolumeUnitM(string name, double factor) { this.name = name; this.factor = factor; }

        public double GetConversionFactor()           => factor;
        public double ConvertToBaseUnit(double value) => value * factor;
        public double ConvertFromBaseUnit(double bv)
        {
            if (Math.Abs(factor) < 1e-15) throw new ArgumentException("Unsupported volume unit");
            return bv / factor;
        }
        public string GetUnitName() => name;
        public override string ToString() => name;
    }

    public class TemperatureUnit : IMeasurable
    {
        public static readonly TemperatureUnit CELSIUS    = new("CELSIUS",    c => c + 273.15,                       k => k - 273.15);
        public static readonly TemperatureUnit FAHRENHEIT = new("FAHRENHEIT", f => (f - 32.0) * 5.0 / 9.0 + 273.15, k => (k - 273.15) * 9.0 / 5.0 + 32.0);
        public static readonly TemperatureUnit KELVIN     = new("KELVIN",     k => k,                                k => k);

        private readonly string              name;
        private readonly Func<double,double> toKelvin;
        private readonly Func<double,double> fromKelvin;

        private TemperatureUnit(string name, Func<double,double> toKelvin, Func<double,double> fromKelvin)
        {
            this.name = name; this.toKelvin = toKelvin; this.fromKelvin = fromKelvin;
        }

        public double GetConversionFactor()           => 1.0;
        public double ConvertToBaseUnit(double value) => toKelvin(value);
        public double ConvertFromBaseUnit(double bv)  => fromKelvin(bv);
        public string GetUnitName()                   => name;
        public bool   SupportsArithmetic()            => false;
        public void   ValidateOperationSupport(string op)
            => throw new NotSupportedException(
                $"Temperature does not support '{op}'. Only conversion and comparison are supported.");
        public override string ToString() => name;
    }

    public class Quantity<U> where U : class, IMeasurable
    {
        public double Value { get; }
        public U      Unit  { get; }

        private enum Op { ADD, SUBTRACT, DIVIDE }

        private static double Compute(Op op, double a, double b) => op switch
        {
            Op.ADD      => a + b,
            Op.SUBTRACT => a - b,
            Op.DIVIDE   => Math.Abs(b) < 1e-15
                ? throw new ArithmeticException("Division by zero.")
                : a / b,
            _ => throw new InvalidOperationException()
        };

        public Quantity(double value, U unit)
        {
            if (unit == null) throw new ArgumentException("Unit cannot be null.");
            if (!double.IsFinite(value)) throw new ArgumentException("Value must be finite.");
            Value = value; Unit = unit;
        }

        public double      ConvertToBaseUnit() => Unit.ConvertToBaseUnit(Value);
        public Quantity<U> ConvertTo(U targetUnit)
        {
            if (targetUnit == null) throw new ArgumentException("Target unit cannot be null.");
            double bv = Unit.ConvertToBaseUnit(Value);
            return new Quantity<U>(Math.Round(targetUnit.ConvertFromBaseUnit(bv), 2, MidpointRounding.AwayFromZero), targetUnit);
        }

        private void ValidateOperands(Quantity<U> other)
        {
            Unit.ValidateOperationSupport("arithmetic");
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (Unit.GetType() != other.Unit.GetType())
                throw new ArgumentException("Cannot perform arithmetic on different measurement categories.");
        }

        private double PerformBase(Quantity<U> other, Op op)
        {
            double a = Unit.ConvertToBaseUnit(Value);
            double b = other.Unit.ConvertToBaseUnit(other.Value);
            return Compute(op, a, b);
        }

        public Quantity<U> Add(Quantity<U> other)
        {
            ValidateOperands(other);
            return new Quantity<U>(Math.Round(Unit.ConvertFromBaseUnit(PerformBase(other, Op.ADD)), 2, MidpointRounding.AwayFromZero), Unit);
        }

        public Quantity<U> Subtract(Quantity<U> other)
        {
            ValidateOperands(other);
            return new Quantity<U>(Math.Round(Unit.ConvertFromBaseUnit(PerformBase(other, Op.SUBTRACT)), 2, MidpointRounding.AwayFromZero), Unit);
        }

        public double Divide(Quantity<U> other)
        {
            ValidateOperands(other);
            return PerformBase(other, Op.DIVIDE);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Quantity<U> that) return false;
            if (Unit.GetType() != that.Unit.GetType()) return false;
            return Math.Abs(Unit.ConvertToBaseUnit(Value) - that.Unit.ConvertToBaseUnit(that.Value)) < 0.01;
        }

        public override int GetHashCode() => Unit.ConvertToBaseUnit(Value).GetHashCode();
        public override string ToString()  => $"Quantity({Value}, {Unit.GetUnitName()})";
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// QMA BUSINESS LAYER — Service Implementation
// ─────────────────────────────────────────────────────────────────────────────

namespace BusinessService.Qma.Service
{
    using BusinessService.Qma.Core;
    using BusinessService.Qma.Exceptions;
    using BusinessService.Qma.Interface;
    using Microsoft.Extensions.Logging;
    using ModelService.Qma.Dto;
    using ModelService.Qma.Entities;
    using RepositoryService.Qma.Interface;

    public class QmaServiceImpl : IQmaService
    {
        private readonly IQmaRepository          _repository;
        private readonly ILogger<QmaServiceImpl> _logger;

        public QmaServiceImpl(IQmaRepository repository, ILogger<QmaServiceImpl> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<QuantityMeasurementDTO> CompareAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("COMPARE", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                bool equal = CompareQuantities(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue = equal ? 1 : 0;
                entity.ResultUnit  = equal ? "EQUAL" : "NOT_EQUAL";
                entity.ResultCategory = "RESULT";
                await _repository.SaveAsync(entity);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QmaMeasurementException) { throw; }
            catch (Exception ex) { return await SaveAndThrow(entity, ex); }
        }

        public async Task<QuantityMeasurementDTO> ConvertAsync(ConvertRequestDTO input, int? userId = null)
        {
            var entity = BuildEntity("CONVERT", input.ThisQuantityDTO, null, userId);
            try
            {
                var (value, unit, category) = ConvertQuantity(input.ThisQuantityDTO, input.TargetUnit);
                entity.ResultValue = value; entity.ResultUnit = unit; entity.ResultCategory = category;
                await _repository.SaveAsync(entity);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QmaMeasurementException) { throw; }
            catch (Exception ex) { return await SaveAndThrow(entity, ex); }
        }

        public async Task<QuantityMeasurementDTO> AddAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("ADD", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                var (value, unit, category) = ArithmeticOp(input.ThisQuantityDTO, input.ThatQuantityDTO, "ADD");
                entity.ResultValue = value; entity.ResultUnit = unit; entity.ResultCategory = category;
                await _repository.SaveAsync(entity);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QmaMeasurementException) { throw; }
            catch (Exception ex) { return await SaveAndThrow(entity, ex); }
        }

        public async Task<QuantityMeasurementDTO> SubtractAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("SUBTRACT", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                var (value, unit, category) = ArithmeticOp(input.ThisQuantityDTO, input.ThatQuantityDTO, "SUBTRACT");
                entity.ResultValue = value; entity.ResultUnit = unit; entity.ResultCategory = category;
                await _repository.SaveAsync(entity);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QmaMeasurementException) { throw; }
            catch (Exception ex) { return await SaveAndThrow(entity, ex); }
        }

        public async Task<QuantityMeasurementDTO> DivideAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("DIVIDE", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                var (value, unit, category) = ArithmeticOp(input.ThisQuantityDTO, input.ThatQuantityDTO, "DIVIDE");
                entity.ResultValue = value; entity.ResultUnit = unit; entity.ResultCategory = category;
                await _repository.SaveAsync(entity);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QmaMeasurementException) { throw; }
            catch (Exception ex) { return await SaveAndThrow(entity, ex); }
        }

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetAllHistoryAsync(int userId)
            => QuantityMeasurementDTO.FromEntityList(await _repository.GetAllByUserAsync(userId));

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByOperationAsync(string operationType, int userId)
            => QuantityMeasurementDTO.FromEntityList(await _repository.GetByOperationTypeAsync(operationType.ToUpperInvariant(), userId));

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByCategoryAsync(string category, int userId)
            => QuantityMeasurementDTO.FromEntityList(await _repository.GetByCategoryAsync(category.ToUpperInvariant(), userId));

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetErrorHistoryAsync(int userId)
            => QuantityMeasurementDTO.FromEntityList(await _repository.GetErrorsAsync(userId));

        public async Task<int> GetOperationCountAsync(string operationType, int userId)
            => await _repository.GetCountByOperationAsync(operationType.ToUpperInvariant(), userId);

        // ── Helpers ───────────────────────────────────────────────────────────

        private static QmaMeasurementEntity BuildEntity(string opType, QuantityDTO q1, QuantityDTO? q2, int? userId) => new()
        {
            UserId = userId, OperationType = opType,
            MeasurementCategory = q1.Category.ToUpperInvariant(),
            Operand1Value = q1.Value, Operand1Unit = q1.UnitName,
            Operand2Value = q2?.Value, Operand2Unit = q2?.UnitName
        };

        private async Task<QuantityMeasurementDTO> SaveAndThrow(QmaMeasurementEntity entity, Exception ex)
        {
            entity.HasError = true; entity.ErrorMessage = ex.Message;
            try { await _repository.SaveAsync(entity); } catch { }
            throw new QmaMeasurementException(ex.Message, ex);
        }

        private static bool CompareQuantities(QuantityDTO q1, QuantityDTO q2)
        {
            ValidateSameCategory(q1, q2, "Compare");
            return q1.Category.ToUpperInvariant() switch
            {
                "LENGTH"      => CompareModels<LengthUnitM>(q1, q2),
                "WEIGHT"      => CompareModels<WeightUnitM>(q1, q2),
                "VOLUME"      => CompareModels<VolumeUnitM>(q1, q2),
                "TEMPERATURE" => CompareModels<TemperatureUnit>(q1, q2),
                _ => throw new QmaMeasurementException($"Unknown category: {q1.Category}")
            };
        }

        private static bool CompareModels<U>(QuantityDTO q1, QuantityDTO q2) where U : class, IMeasurable
        {
            var u1 = ResolveUnit<U>(q1.UnitName);
            var u2 = ResolveUnit<U>(q2.UnitName);
            return Math.Abs(u1.ConvertToBaseUnit(q1.Value) - u2.ConvertToBaseUnit(q2.Value)) < 0.01;
        }

        private static (double value, string unit, string category) ConvertQuantity(QuantityDTO q, string targetUnit)
        {
            return q.Category.ToUpperInvariant() switch
            {
                "LENGTH"      => DoConvert<LengthUnitM>(q, targetUnit, "LENGTH"),
                "WEIGHT"      => DoConvert<WeightUnitM>(q, targetUnit, "WEIGHT"),
                "VOLUME"      => DoConvert<VolumeUnitM>(q, targetUnit, "VOLUME"),
                "TEMPERATURE" => DoConvert<TemperatureUnit>(q, targetUnit, "TEMPERATURE"),
                _ => throw new QmaMeasurementException($"Unknown category: {q.Category}")
            };
        }

        private static (double, string, string) DoConvert<U>(QuantityDTO q, string targetUnit, string category)
            where U : class, IMeasurable
        {
            var src = ResolveUnit<U>(q.UnitName);
            var tgt = ResolveUnit<U>(targetUnit);
            var res = new Quantity<U>(q.Value, src).ConvertTo(tgt);
            return (res.Value, res.Unit.GetUnitName(), category);
        }

        private static (double value, string unit, string category) ArithmeticOp(QuantityDTO q1, QuantityDTO q2, string op)
        {
            ValidateSameCategory(q1, q2, op);
            var category = q1.Category.ToUpperInvariant();
            return category switch
            {
                "LENGTH"      => DoArithmetic<LengthUnitM>(q1, q2, op, category),
                "WEIGHT"      => DoArithmetic<WeightUnitM>(q1, q2, op, category),
                "VOLUME"      => DoArithmetic<VolumeUnitM>(q1, q2, op, category),
                "TEMPERATURE" => throw new QmaMeasurementException("Temperature does not support arithmetic."),
                _ => throw new QmaMeasurementException($"Unknown category: {category}")
            };
        }

        private static (double, string, string) DoArithmetic<U>(QuantityDTO q1, QuantityDTO q2, string op, string category)
            where U : class, IMeasurable
        {
            var qa = new Quantity<U>(q1.Value, ResolveUnit<U>(q1.UnitName));
            var qb = new Quantity<U>(q2.Value, ResolveUnit<U>(q2.UnitName));
            return op switch
            {
                "ADD"      => (qa.Add(qb).Value,      qa.Unit.GetUnitName(), category),
                "SUBTRACT" => (qa.Subtract(qb).Value, qa.Unit.GetUnitName(), category),
                "DIVIDE"   => (qa.Divide(qb),         "RATIO",               "SCALAR"),
                _ => throw new InvalidOperationException($"Unknown op: {op}")
            };
        }

        private static U ResolveUnit<U>(string unitName) where U : class, IMeasurable
        {
            string n = unitName.Trim().ToUpperInvariant();
            if (typeof(U) == typeof(LengthUnitM))     return (U)(IMeasurable)ResolveLengthUnit(n);
            if (typeof(U) == typeof(WeightUnitM))     return (U)(IMeasurable)ResolveWeightUnit(n);
            if (typeof(U) == typeof(VolumeUnitM))     return (U)(IMeasurable)ResolveVolumeUnit(n);
            if (typeof(U) == typeof(TemperatureUnit)) return (U)(IMeasurable)ResolveTempUnit(n);
            throw new QmaMeasurementException($"Unknown unit type: {typeof(U).Name}");
        }

        private static LengthUnitM ResolveLengthUnit(string n) => n switch
        {
            "FEET" or "FOOT" or "FT" or "FT."   => LengthUnitM.FEET,
            "INCHES" or "INCH" or "IN" or "IN." => LengthUnitM.INCHES,
            "YARDS" or "YARD" or "YD" or "YDS"  => LengthUnitM.YARDS,
            "CENTIMETERS" or "CENTIMETER" or "CM" or "CM." => LengthUnitM.CENTIMETERS,
            _ => throw new QmaMeasurementException($"Unknown length unit: '{n}'")
        };

        private static WeightUnitM ResolveWeightUnit(string n) => n switch
        {
            "KILOGRAM" or "KILOGRAMS" or "KG" or "KG."              => WeightUnitM.KILOGRAM,
            "GRAM" or "GRAMS" or "G" or "GR"                        => WeightUnitM.GRAM,
            "POUND" or "POUNDS" or "LB" or "LBS" or "LB." or "LBS." => WeightUnitM.POUND,
            _ => throw new QmaMeasurementException($"Unknown weight unit: '{n}'")
        };

        private static VolumeUnitM ResolveVolumeUnit(string n) => n switch
        {
            "LITRE" or "LITRES" or "LITER" or "LITERS" or "L" or "LT" or "LTR" => VolumeUnitM.LITRE,
            "MILLILITRE" or "MILLILITRES" or "MILLILITER" or "ML" or "ML."      => VolumeUnitM.MILLILITRE,
            "GALLON" or "GALLONS" or "GAL" or "GAL."                            => VolumeUnitM.GALLON,
            _ => throw new QmaMeasurementException($"Unknown volume unit: '{n}'")
        };

        private static TemperatureUnit ResolveTempUnit(string n) => n switch
        {
            "CELSIUS" or "C" or "CEL"               => TemperatureUnit.CELSIUS,
            "FAHRENHEIT" or "F" or "FAH" or "FAHR"  => TemperatureUnit.FAHRENHEIT,
            "KELVIN" or "K" or "KEL"                => TemperatureUnit.KELVIN,
            _ => throw new QmaMeasurementException($"Unknown temperature unit: '{n}'")
        };

        private static void ValidateSameCategory(QuantityDTO q1, QuantityDTO q2, string op)
        {
            if (!string.Equals(q1.Category, q2.Category, StringComparison.OrdinalIgnoreCase))
                throw new QmaMeasurementException(
                    $"Cannot {op} across different categories: {q1.Category} and {q2.Category}.");
        }
    }
}
