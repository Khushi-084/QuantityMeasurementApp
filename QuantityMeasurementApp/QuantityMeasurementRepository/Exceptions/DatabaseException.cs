namespace QuantityMeasurementRepository.Exceptions
{
    /// <summary>
    /// UC16: Custom exception for database-layer errors.
    /// Equivalent to DatabaseException.java in Java UC16.
    /// Wraps SqlException with meaningful messages for upper layers.
    /// </summary>
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }
        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
