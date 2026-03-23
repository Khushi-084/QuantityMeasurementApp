using System.Text.Json;
using System.Text.Json.Serialization;
using QuantityMeasurementModel;

namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC16 update to UC15 CacheRepository.
    /// Adds JSON file persistence — every Save() and Clear() syncs to
    /// measurements.json in the application's base directory.
    /// All UC15 in-memory logic is UNCHANGED.
    /// </summary>
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        // ── Singleton ──────────────────────────────────────────────────────
        public static readonly Lazy<QuantityMeasurementCacheRepository> _instance =
            new(() => new QuantityMeasurementCacheRepository());

        public static QuantityMeasurementCacheRepository Instance => _instance.Value;

        // ── In-memory cache ────────────────────────────────────────────────
        private readonly List<QuantityMeasurementEntity> _cache = new();
        private readonly object _lock = new();

        // ── JSON file path — sits alongside appsettings.json in project root ─
        // Directory.GetCurrentDirectory() resolves to the project folder when
        // running via "dotnet run" or Visual Studio (not the bin/Debug folder).
        private static readonly string _jsonFilePath =
            Path.Combine(Directory.GetCurrentDirectory(), "measurements.json");

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented        = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // ── Constructor: load existing JSON on startup ─────────────────────
        private QuantityMeasurementCacheRepository()
        {
            LoadFromJson();
        }

        // ── UC15: Save — adds JSON sync ────────────────────────────────────
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            lock (_lock)
            {
                _cache.Add(entity);
                PersistToJson();
            }
        }

        // ── UC15: GetAllMeasurements — UNCHANGED ───────────────────────────
        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            lock (_lock) { return _cache.AsReadOnly(); }
        }

        // ── UC15: Clear — adds JSON sync ───────────────────────────────────
        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
                PersistToJson();
            }
        }

        // ── UC16: GetByOperationType ───────────────────────────────────────
        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(string operationType)
        {
            if (string.IsNullOrWhiteSpace(operationType))
                throw new ArgumentException("Operation type cannot be empty.");
            lock (_lock)
            {
                return _cache
                    .Where(e => string.Equals(e.OperationType, operationType,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList().AsReadOnly();
            }
        }

        // ── UC16: GetByCategory ────────────────────────────────────────────
        public IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty.");
            lock (_lock)
            {
                return _cache
                    .Where(e => string.Equals(e.MeasurementCategory, category,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList().AsReadOnly();
            }
        }

        // ── UC16: GetTotalCount ────────────────────────────────────────────
        public int GetTotalCount()
        {
            lock (_lock) { return _cache.Count; }
        }

        // ── UC16: GetRepositoryInfo ────────────────────────────────────────
        public string GetRepositoryInfo()
            => $"In-Memory + JSON Cache Repository | Records: {GetTotalCount()} | File: {_jsonFilePath}";

        // ══════════════════════════════════════════════════════════════════
        // JSON PERSISTENCE HELPERS
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Serializes the entire cache to measurements.json.
        /// Called inside the lock from Save() and Clear().
        /// </summary>
        private void PersistToJson()
        {
            try
            {
                var records = _cache.Select(e => new JsonRecord(e)).ToList();
                string json = JsonSerializer.Serialize(records, _jsonOptions);
                File.WriteAllText(_jsonFilePath, json);
                Console.WriteLine($"[CacheRepository] Saved to JSON: {_jsonFilePath} ({_cache.Count} record(s))");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CacheRepository] WARNING: Could not write JSON file: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads records from measurements.json into the cache on startup.
        /// If the file doesn't exist yet, starts with an empty cache.
        /// </summary>
        private void LoadFromJson()
        {
            try
            {
                if (!File.Exists(_jsonFilePath))
                {
                    Console.WriteLine("[CacheRepository] No existing JSON file found. Starting fresh.");
                    return;
                }

                string json = File.ReadAllText(_jsonFilePath);
                var records = JsonSerializer.Deserialize<List<JsonRecord>>(json, _jsonOptions);

                if (records == null) return;

                foreach (var r in records)
                {
                    var entity = r.ToEntity();
                    if (entity != null)
                        _cache.Add(entity);
                }

                Console.WriteLine($"[CacheRepository] Loaded {_cache.Count} record(s) from JSON: {_jsonFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CacheRepository] WARNING: Could not read JSON file: {ex.Message}");
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // JSON RECORD — flat DTO for serialization
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Flat serializable record for one QuantityMeasurementEntity.
        /// System.Text.Json cannot serialize private setters directly,
        /// so we project to/from this plain DTO.
        /// </summary>
        private class JsonRecord
        {
            public string   OperationType       { get; set; } = string.Empty;
            public string   MeasurementCategory { get; set; } = string.Empty;
            public DateTime Timestamp           { get; set; }
            public bool     HasError            { get; set; }
            public string?  ErrorMessage        { get; set; }

            // Operand1
            public double?  Operand1Value       { get; set; }
            public string?  Operand1Unit        { get; set; }

            // Operand2
            public double?  Operand2Value       { get; set; }
            public string?  Operand2Unit        { get; set; }

            // Result
            public double?  ResultValue         { get; set; }
            public string?  ResultUnit          { get; set; }

            // Parameterless constructor required by System.Text.Json
            public JsonRecord() { }

            // Build from entity
            public JsonRecord(QuantityMeasurementEntity e)
            {
                OperationType       = e.OperationType;
                MeasurementCategory = e.MeasurementCategory;
                Timestamp           = e.Timestamp;
                HasError            = e.HasError;
                ErrorMessage        = e.HasError ? e.ErrorMessage : null;

                Operand1Value = e.Operand1?.Value;
                Operand1Unit  = e.Operand1?.UnitName;

                Operand2Value = e.Operand2?.Value;
                Operand2Unit  = e.Operand2?.UnitName;

                ResultValue   = e.Result?.Value;
                ResultUnit    = e.Result?.UnitName;
            }

            // Rebuild entity from JSON record
            public QuantityMeasurementEntity? ToEntity()
            {
                string category = MeasurementCategory;

                QuantityDTO? op1 = Operand1Value.HasValue
                    ? new QuantityDTO(Operand1Value.Value, Operand1Unit ?? "", category)
                    : null;

                QuantityDTO? op2 = Operand2Value.HasValue
                    ? new QuantityDTO(Operand2Value.Value, Operand2Unit ?? "", category)
                    : null;

                QuantityDTO? res = ResultValue.HasValue
                    ? new QuantityDTO(ResultValue.Value, ResultUnit ?? "", category)
                    : null;

                if (HasError)
                    return new QuantityMeasurementEntity(OperationType, op1, op2, ErrorMessage ?? "");

                if (op2 != null && res != null)
                    return new QuantityMeasurementEntity(OperationType, op1!, op2, res);

                return new QuantityMeasurementEntity(OperationType, op1!, res);
            }
        }
    }
}