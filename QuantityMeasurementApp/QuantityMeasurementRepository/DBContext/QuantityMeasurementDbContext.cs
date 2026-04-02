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

            modelBuilder.Entity<QuantityMeasurementApiEntity>(e =>
            {
                e.HasIndex(x => x.OperationType).HasDatabaseName("IX_QM_OperationType");
                e.HasIndex(x => x.MeasurementCategory).HasDatabaseName("IX_QM_Category");
                e.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_QM_CreatedAt");
                // Index on UserId for fast per-user queries
                e.HasIndex(x => x.UserId).HasDatabaseName("IX_QM_UserId");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.HasError).HasDefaultValue(false);
                // UserId is nullable — anonymous ops have no user
                e.Property(x => x.UserId).IsRequired(false);
            });

            modelBuilder.Entity<UserEntity>(e =>
            {
                e.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Users_Email");
                e.HasIndex(x => x.Username).IsUnique().HasDatabaseName("IX_Users_Username");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.IsActive).HasDefaultValue(true);
                e.Property(x => x.Role).HasDefaultValue("User");
            });
        }
    }
}
