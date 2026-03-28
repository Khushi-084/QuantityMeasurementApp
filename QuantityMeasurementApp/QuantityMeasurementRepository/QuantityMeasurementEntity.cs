using QuantityMeasurementModel;

namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC16: Extended entity.
    /// UC15 code (all three constructors, ToString, all properties) is UNCHANGED.
    /// UC16 adds two new properties: Id (DB primary key) and MeasurementCategory.
    /// </summary>
    [Serializable]
    public class QuantityMeasurementEntity
    {
        // ── UC16: New properties ───────────────────────────────────────────
        public int    Id                  { get; private set; }
        public string MeasurementCategory { get; private set; } = string.Empty;

        // ── UC15: Original properties — UNCHANGED ─────────────────────────
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
            OperationType       = operationType;
            Operand1            = operand1;
            Result              = result;
            HasError            = false;
            ErrorMessage        = string.Empty;
            Timestamp           = DateTime.UtcNow;
            MeasurementCategory = operand1?.Category ?? string.Empty;
        }

        // Binary operand (compare, add, subtract, divide)
        public QuantityMeasurementEntity(
            string operationType,
            QuantityDTO operand1,
            QuantityDTO operand2,
            QuantityDTO result)
        {
            OperationType       = operationType;
            Operand1            = operand1;
            Operand2            = operand2;
            Result              = result;
            HasError            = false;
            ErrorMessage        = string.Empty;
            Timestamp           = DateTime.UtcNow;
            MeasurementCategory = operand1?.Category ?? string.Empty;
        }

        // Error constructor
        public QuantityMeasurementEntity(
            string operationType,
            QuantityDTO? operand1,
            QuantityDTO? operand2,
            string errorMessage)
        {
            OperationType       = operationType;
            Operand1            = operand1;
            Operand2            = operand2;
            HasError            = true;
            ErrorMessage        = errorMessage;
            Timestamp           = DateTime.UtcNow;
            MeasurementCategory = operand1?.Category ?? string.Empty;
        }

        // UC15 ToString — UNCHANGED
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
