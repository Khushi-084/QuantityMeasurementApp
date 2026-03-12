using QuantityMeasurementModel;

namespace QuantityMeasurementBusinessLayer
{
    /// <summary>
    /// UC15: Service interface — defines the contract for all measurement operations.
    /// Accepts and returns QuantityDTO objects (string-based, layer-agnostic).
    /// </summary>
    public interface IQuantityMeasurementService
    {
        /// <summary>Compare two quantities. Returns QuantityDTO with Value=1 (equal) or 0.</summary>
        QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2);

        /// <summary>Convert q1 to the unit specified in targetUnitDTO.</summary>
        QuantityDTO Convert(QuantityDTO q1, QuantityDTO targetUnitDTO);

        /// <summary>Add two quantities. Result unit = q1's unit.</summary>
        QuantityDTO Add(QuantityDTO q1, QuantityDTO q2);

        /// <summary>Subtract q2 from q1. Result unit = q1's unit.</summary>
        QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2);

        /// <summary>Divide q1 by q2. Returns dimensionless scalar in QuantityDTO (Category="SCALAR").</summary>
        QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2);
    }
}
