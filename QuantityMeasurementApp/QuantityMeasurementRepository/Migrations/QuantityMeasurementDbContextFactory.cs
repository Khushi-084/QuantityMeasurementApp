using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementRepository.Database;

namespace QuantityMeasurementRepository.Migrations
{
    /// <summary>
    /// UC18: Design-time factory used by the EF Core CLI tools
    /// (dotnet ef migrations add / dotnet ef database update).
    ///
    /// This class is ONLY invoked by tooling — never at runtime.
    /// It reads the connection string from QuantityMeasurementApi/appsettings.json
    /// so you can run migration commands from any working directory, e.g.:
    ///
    ///   dotnet ef migrations add InitialCreate \
    ///       --project QuantityMeasurementRepository \
    ///       --startup-project QuantityMeasurementApi
    ///
    ///   dotnet ef database update \
    ///       --project QuantityMeasurementRepository \
    ///       --startup-project QuantityMeasurementApi
    /// </summary>
    public class QuantityMeasurementDbContextFactory
        : IDesignTimeDbContextFactory<QuantityMeasurementDbContext>
    {
        public QuantityMeasurementDbContext CreateDbContext(string[] args)
        {
            // Walk up from the Repository project to find the API's appsettings.json.
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "QuantityMeasurementApi");

            if (!Directory.Exists(basePath))
                basePath = Directory.GetCurrentDirectory(); // fallback

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection")
                ?? "Server=localhost\\SQLEXPRESS;Database=QuantityMeasurementDb;"
                 + "Trusted_Connection=True;TrustServerCertificate=True;";

            var optionsBuilder = new DbContextOptionsBuilder<QuantityMeasurementDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sql =>
            {
                sql.CommandTimeout(30);
                sql.MigrationsAssembly("QuantityMeasurementRepository");
            });

            return new QuantityMeasurementDbContext(optionsBuilder.Options);
        }
    }
}