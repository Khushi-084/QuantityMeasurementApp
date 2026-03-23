using Microsoft.Extensions.Configuration;

namespace QuantityMeasurementRepository.Util
{
    /// <summary>
    /// UC16: Loads database configuration from appsettings.json.
    /// Equivalent to Java ApplicationConfig.java.
    ///
    /// CONNECTION STRING for SSMS (SQL Server):
    ///   "Server=YOUR_SERVER_NAME;Database=QuantityMeasurementDb;
    ///    Trusted_Connection=True;TrustServerCertificate=True;"
    ///
    /// Replace YOUR_SERVER_NAME with your actual server name from SSMS login screen.
    /// Examples: "localhost", ".\SQLEXPRESS", "DESKTOP-ABC\SQLEXPRESS"
    /// </summary>
    public class DatabaseConfig
    {
        public string ConnectionString  { get; }
        public int    MaxPoolSize       { get; }
        public int    ConnectionTimeout { get; }

        public DatabaseConfig(IConfiguration? configuration = null)
        {
            if (configuration != null)
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "Connection string 'DefaultConnection' not found in appsettings.json.");
                MaxPoolSize       = int.Parse(configuration["Database:MaxPoolSize"]       ?? "5");
                ConnectionTimeout = int.Parse(configuration["Database:ConnectionTimeout"] ?? "30");
            }
            else
            {
                // Manual load — used by Console App
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

                ConnectionString = config.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "Connection string 'DefaultConnection' not found in appsettings.json.");
                MaxPoolSize       = int.Parse(config["Database:MaxPoolSize"]       ?? "5");
                ConnectionTimeout = int.Parse(config["Database:ConnectionTimeout"] ?? "30");
            }
        }
    }
}
