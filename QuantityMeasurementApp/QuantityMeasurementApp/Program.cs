using QuantityMeasurementBusinessLayer;
using QuantityMeasurementRepository;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// UC15: Application Layer — Entry Point
    /// Responsibilities:
    ///   - Initialize dependencies (Factory Pattern)
    ///   - Wire layers together (Dependency Injection)
    ///   - Delegate ALL logic to Controller (Facade Pattern)
    ///   - No business logic here
    /// </summary>
    class Program
    {
        static void Main()
        {
            // -- Factory Pattern: create instances --------------------------

            // Singleton Repository
            IQuantityMeasurementRepository repo =
                QuantityMeasurementCacheRepository.Instance;

            // Factory: create service with injected repository
            IQuantityMeasurementService service =
                CreateService(repo);

            // Factory: create controller with injected service + repo
            QuantityMeasurementController controller =
                CreateController(service, repo);

            // -- Delegate ALL logic to controller ---------------------------
            controller.Start();
        }

        // -- Factory Methods ------------------------------------------------

        private static IQuantityMeasurementService CreateService(
            IQuantityMeasurementRepository repo)
            => new QuantityMeasurementServiceImpl(repo);

        private static QuantityMeasurementController CreateController(
            IQuantityMeasurementService service,
            IQuantityMeasurementRepository repo)
            => new QuantityMeasurementController(service, repo);
    }
}
