using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Database;

namespace QuantityMeasurementRepository.Database
{
    /// <summary>
    /// UC16 → UC17 Migration: Replaced ADO.NET with EF Core ORM.
    /// All raw SqlCommand / SqlConnection / ConnectionPool code removed.
    /// Now uses QuantityMeasurementDbContext injected via DI.
    /// </summary>
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly QuantityMeasurementDbContext _context;
        private readonly ILogger<QuantityMeasurementDatabaseRepository> _logger;

        public QuantityMeasurementDatabaseRepository(
            QuantityMeasurementDbContext context,
            ILogger<QuantityMeasurementDatabaseRepository> logger)
        {
            _context = context;
            _logger  = logger;
        }

        // ══════════════════════════════════════════════════════════════════
        // UC15 INTERFACE METHODS
        // ══════════════════════════════════════════════════════════════════

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var record = new QuantityMeasurementApiEntity
            {
                OperationType       = entity.OperationType,
                MeasurementCategory = entity.MeasurementCategory,
                Operand1Value       = entity.Operand1?.Value,
                Operand1Unit        = entity.Operand1?.UnitName,
                Operand2Value       = entity.Operand2?.Value,
                Operand2Unit        = entity.Operand2?.UnitName,
                ResultValue         = entity.Result?.Value,
                ResultUnit          = entity.Result?.UnitName,
                HasError            = entity.HasError,
                ErrorMessage        = entity.ErrorMessage,
                CreatedAt           = DateTime.UtcNow
            };

            _context.QuantityMeasurements.Add(record);
            _context.SaveChanges();

            _logger.LogInformation("Saved measurement: Op={Op} Cat={Cat}",
                entity.OperationType, entity.MeasurementCategory);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            return _context.QuantityMeasurements
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .AsEnumerable()
                .Select(MapToEntity)
                .ToList();
        }

        public void Clear()
        {
            _context.QuantityMeasurements.ExecuteDelete();
            _logger.LogInformation("All measurements deleted.");
        }

        // ══════════════════════════════════════════════════════════════════
        // UC16 ADDITIONAL METHODS
        // ══════════════════════════════════════════════════════════════════

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(string operationType)
        {
            if (string.IsNullOrWhiteSpace(operationType))
                throw new ArgumentException("Operation type cannot be empty.");

            return _context.QuantityMeasurements
                .AsNoTracking()
                .Where(x => x.OperationType == operationType.ToUpperInvariant())
                .OrderBy(x => x.CreatedAt)
                .AsEnumerable()
                .Select(MapToEntity)
                .ToList();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty.");

            return _context.QuantityMeasurements
                .AsNoTracking()
                .Where(x => x.MeasurementCategory == category.ToUpperInvariant())
                .OrderBy(x => x.CreatedAt)
                .AsEnumerable()
                .Select(MapToEntity)
                .ToList();
        }

        public int GetTotalCount()
            => _context.QuantityMeasurements.Count();

        public string GetRepositoryInfo()
            => $"EF Core ORM Repository | Records: {GetTotalCount()}";

        public void ReleaseResources() { /* EF Core manages connections automatically */ }

        // ══════════════════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ══════════════════════════════════════════════════════════════════

        private static QuantityMeasurementEntity MapToEntity(QuantityMeasurementApiEntity record)
        {
            var operand1 = record.Operand1Value.HasValue
                ? new QuantityDTO(record.Operand1Value.Value,
                    record.Operand1Unit ?? string.Empty,
                    record.MeasurementCategory)
                : null;

            var operand2 = record.Operand2Value.HasValue
                ? new QuantityDTO(record.Operand2Value.Value,
                    record.Operand2Unit ?? string.Empty,
                    record.MeasurementCategory)
                : null;

            var result = record.ResultValue.HasValue
                ? new QuantityDTO(record.ResultValue.Value,
                    record.ResultUnit ?? string.Empty,
                    record.MeasurementCategory)
                : null;

            if (record.HasError)
                return new QuantityMeasurementEntity(
                    record.OperationType, operand1, operand2,
                    record.ErrorMessage ?? string.Empty);

            if (operand2 != null && result != null)
                return new QuantityMeasurementEntity(
                    record.OperationType, operand1!, operand2, result);

            return new QuantityMeasurementEntity(
                record.OperationType, operand1!, result);
        }
    }
}
