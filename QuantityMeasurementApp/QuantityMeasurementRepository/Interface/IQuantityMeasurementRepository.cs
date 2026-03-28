namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC16: Extended repository interface.
    /// UC15 methods (Save, GetAllMeasurements, Clear) are UNCHANGED.
    /// UC16 adds: GetByOperationType, GetByCategory, GetTotalCount,
    ///            GetRepositoryInfo (pool stats), ReleaseResources.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        // ── UC15 methods — UNCHANGED ───────────────────────────────────────
        void Save(QuantityMeasurementEntity entity);
        IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
        void Clear();

        // ── UC16 additions ─────────────────────────────────────────────────
        IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(string operationType);
        IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category);
        int GetTotalCount();

        // Default methods — equivalent to Java interface default methods
        string GetRepositoryInfo() => "In-Memory Cache Repository";
        void ReleaseResources() { /* no-op for cache */ }
    }
}
