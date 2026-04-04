using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Database
{
    public class QuantityMeasurementDbContext : DbContext
    {
        public QuantityMeasurementDbContext(DbContextOptions<QuantityMeasurementDbContext> options)
            : base(options) { }

        public DbSet<QuantityMeasurementApiEntity> QuantityMeasurements
            => Set<QuantityMeasurementApiEntity>();

        public DbSet<UserEntity> Users => Set<UserEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var isRelational = Database.IsRelational();

            modelBuilder.Entity<QuantityMeasurementApiEntity>(e =>
            {
                e.HasIndex(x => x.OperationType).HasDatabaseName("IX_QM_OperationType");
                e.HasIndex(x => x.MeasurementCategory).HasDatabaseName("IX_QM_Category");
                e.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_QM_CreatedAt");
                e.HasIndex(x => x.UserId).HasDatabaseName("IX_QM_UserId");
                e.Property(x => x.UserId).IsRequired(false);

                // HasDefaultValueSql("GETUTCDATE()") marks CreatedAt as ValueGeneratedOnAdd,
                // which stops EF from sending the C# default (= DateTime.UtcNow) and instead
                // expects the DB to supply the value — but InMemory cannot execute SQL expressions,
                // causing "An error occurred while saving the entity changes."
                // Fix: only use SQL defaults for real relational DBs; InMemory uses the C# defaults.
                if (isRelational)
                {
                    e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");e.Property(x => x.HasError).HasDefaultValue(false);
                }
            });

            modelBuilder.Entity<UserEntity>(e =>
            {
                e.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Users_Email");
                e.HasIndex(x => x.Username).IsUnique().HasDatabaseName("IX_Users_Username");

                if (isRelational)
                {
                    e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                    e.Property(x => x.IsActive).HasDefaultValue(true);
                    e.Property(x => x.Role).HasDefaultValue("User");
                }
            });
        }
    }
}
