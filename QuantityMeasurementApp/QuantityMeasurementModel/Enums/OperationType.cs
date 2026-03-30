namespace QuantityMeasurementModel.Enums
{
    /// <summary>UC17: Operation types for REST API history queries.</summary>
    public enum OperationType
    {
        COMPARE,
        CONVERT,
        ADD,
        SUBTRACT,
        DIVIDE
    }

    /// <summary>UC17: Measurement categories.</summary>
    public enum MeasurementCategory
    {
        LENGTH,
        WEIGHT,
        VOLUME,
        TEMPERATURE,
        SCALAR
    }

    /// <summary>UC17: User roles for JWT claims.</summary>
    public enum UserRole
    {
        User,
        Admin
    }
}
