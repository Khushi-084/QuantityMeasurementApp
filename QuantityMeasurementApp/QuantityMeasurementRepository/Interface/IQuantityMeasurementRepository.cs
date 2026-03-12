namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC15: Repository interface — ISP-compliant contract for data access.
    /// Allows swapping between in-memory cache, database, etc.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        void Save(QuantityMeasurementEntity entity);
        IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
        void Clear();
    }
}
