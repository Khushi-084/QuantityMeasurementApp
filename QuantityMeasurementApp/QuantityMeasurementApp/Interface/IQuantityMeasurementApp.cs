namespace QuantityMeasurementApp.Interface
{
    /// <summary>
    /// UC15: Application-layer interface.
    /// Decouples Program from the concrete Controller — Program depends only on this contract.
    /// </summary>
    public interface IQuantityMeasurementApp
    {
        void Run();
    }
}
