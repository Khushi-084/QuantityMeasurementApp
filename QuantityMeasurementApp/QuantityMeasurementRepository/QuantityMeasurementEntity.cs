using QuantityMeasurementModel;

namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC15: Entity for storing operation history in repository.
    /// Immutable by design — all fields set via constructors.
    /// Three constructors: single operand, binary operand, error.
    /// </summary>
    [Serializable]
    public class QuantityMeasurementEntity
    {
        public QuantityDTO?  Operand1      { get; private set; }
        public QuantityDTO?  Operand2      { get; private set; }
        public QuantityDTO?  Result        { get; private set; }
        public string        OperationType { get; private set; }
        public DateTime      Timestamp     { get; private set; }
        public bool          HasError      { get; private set; }
        public string        ErrorMessage  { get; private set; }

        // Single operand (e.g. convert)
        public QuantityMeasurementEntity(
            string operationType,
            QuantityDTO operand1,
            QuantityDTO? result)
        {
            OperationType = operationType;
            Operand1      = operand1;
            Result        = result;
            HasError      = false;
            ErrorMessage  = string.Empty;
            Timestamp     = DateTime.UtcNow;
        }

        // Binary operand (compare, add, subtract, divide)
        public QuantityMeasurementEntity(
            string operationType,
            QuantityDTO operand1,
            QuantityDTO operand2,
            QuantityDTO result)
        {
            OperationType = operationType;
            Operand1      = operand1;
            Operand2      = operand2;
            Result        = result;
            HasError      = false;
            ErrorMessage  = string.Empty;
            Timestamp     = DateTime.UtcNow;
        }

        // Error constructor
        public QuantityMeasurementEntity(
            string operationType,
            QuantityDTO? operand1,
            QuantityDTO? operand2,
            string errorMessage)
        {
            OperationType = operationType;
            Operand1      = operand1;
            Operand2      = operand2;
            HasError      = true;
            ErrorMessage  = errorMessage;
            Timestamp     = DateTime.UtcNow;
        }

        public override string ToString()
        {
            if (HasError)
            {
                if (Operand2 != null)
                    return $"  Operation: {OperationType} | " +
                           $"This: {Operand1?.Value} {Operand1?.UnitName} | " +
                           $"That: {Operand2?.Value} {Operand2?.UnitName} | " +
                           $"Result: Error: {ErrorMessage}";
                return $"  Operation: {OperationType} | " +
                       $"This: {Operand1?.Value} {Operand1?.UnitName} | " +
                       $"Result: Error: {ErrorMessage}";
            }

            if (OperationType == "CONVERT")
                return $"  Operation: {OperationType} | " +
                       $"This: {Operand1?.Value} {Operand1?.UnitName} | " +
                       $"Result: {Result?.Value} {Result?.UnitName}";

            if (OperationType == "COMPARE")
            {
                string label = Result?.Value == 1 ? "EQUAL" : "NOT_EQUAL";
                return $"  Operation: {OperationType} | " +
                       $"This: {Operand1?.Value} {Operand1?.UnitName} | " +
                       $"That: {Operand2?.Value} {Operand2?.UnitName} | " +
                       $"Result: {Result?.Value} {label}";
            }

            if (OperationType == "DIVIDE")
                return $"  Operation: {OperationType} | " +
                       $"This: {Operand1?.Value} {Operand1?.UnitName} | " +
                       $"That: {Operand2?.Value} {Operand2?.UnitName} | " +
                       $"Result: {Result?.Value} RATIO";

            return $"  Operation: {OperationType} | " +
                   $"This: {Operand1?.Value} {Operand1?.UnitName} | " +
                   $"That: {Operand2?.Value} {Operand2?.UnitName} | " +
                   $"Result: {Result?.Value} {Result?.UnitName}";
        }
    }
}
