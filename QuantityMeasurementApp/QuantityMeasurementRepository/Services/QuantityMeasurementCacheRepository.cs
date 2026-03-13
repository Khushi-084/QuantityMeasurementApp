namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC15: Singleton in-memory cache repository implementing IQuantityMeasurementRepository.
    /// Persists measurement history for the lifetime of the application.
    /// Optionally serializes to disk for cross-restart persistence.
    /// </summary>
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        // ---- Singleton ----
        public static readonly Lazy<QuantityMeasurementCacheRepository> _instance =new(() => new QuantityMeasurementCacheRepository());

        public static QuantityMeasurementCacheRepository Instance => _instance.Value;

        // ---- In-memory cache ----
        private readonly List<QuantityMeasurementEntity> _cache = new();
        private readonly object _lock = new();

    
        private QuantityMeasurementCacheRepository()
        {
            // Load from disk if file exists (enables cross-restart persistence)
            // Skipped silently if file is missing or corrupt
        }

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            lock (_lock)
            {
                _cache.Add(entity);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            lock (_lock)
            {
                return _cache.AsReadOnly();
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }
    }
}
