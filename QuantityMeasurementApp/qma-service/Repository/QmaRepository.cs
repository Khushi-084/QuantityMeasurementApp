// ─────────────────────────────────────────────────────────────────────────────
// QMA REPOSITORY — DbContext
// ─────────────────────────────────────────────────────────────────────────────

namespace RepositoryService.Qma.DBContext
{
    using Microsoft.EntityFrameworkCore;
    using ModelService.Qma.Entities;

    public class QmaDbContext : DbContext
    {
        public QmaDbContext(DbContextOptions<QmaDbContext> options) : base(options) { }

        public DbSet<QmaMeasurementEntity> QuantityMeasurements => Set<QmaMeasurementEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var isRelational = Database.IsRelational();

            modelBuilder.Entity<QmaMeasurementEntity>(e =>
            {
                e.HasIndex(x => x.OperationType).HasDatabaseName("IX_QM_OperationType");
                e.HasIndex(x => x.MeasurementCategory).HasDatabaseName("IX_QM_Category");
                e.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_QM_CreatedAt");
                e.HasIndex(x => x.UserId).HasDatabaseName("IX_QM_UserId");
                e.Property(x => x.UserId).IsRequired(false);

                if (isRelational)
                {
                    e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                    e.Property(x => x.HasError).HasDefaultValue(false);
                }
            });
        }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// QMA REPOSITORY — Interface
// ─────────────────────────────────────────────────────────────────────────────

namespace RepositoryService.Qma.Interface
{
    using ModelService.Qma.Entities;

    public interface IQmaRepository
    {
        Task SaveAsync(QmaMeasurementEntity entity);
        Task<IReadOnlyList<QmaMeasurementEntity>> GetAllByUserAsync(int userId);
        Task<IReadOnlyList<QmaMeasurementEntity>> GetByOperationTypeAsync(string op, int userId);
        Task<IReadOnlyList<QmaMeasurementEntity>> GetByCategoryAsync(string category, int userId);
        Task<IReadOnlyList<QmaMeasurementEntity>> GetErrorsAsync(int userId);
        Task<int> GetCountByOperationAsync(string op, int userId);
        Task<int> GetTotalCountAsync(int userId);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// QMA REPOSITORY — Implementation (EF Core + Redis cache)
// ─────────────────────────────────────────────────────────────────────────────

namespace RepositoryService.Qma.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using ModelService.Qma.Entities;
    using RepositoryService.Qma.DBContext;
    using RepositoryService.Qma.Interface;
    using StackExchange.Redis;
    using System.Text.Json;

    public class QmaRepository : IQmaRepository
    {
        private readonly QmaDbContext           _context;
        private readonly ILogger<QmaRepository> _logger;
        private readonly IDatabase?             _redis;
        private const int CacheTtlSecs = 300;

        public QmaRepository(QmaDbContext context, ILogger<QmaRepository> logger,
            IConnectionMultiplexer? redis = null)
        {
            _context = context;
            _logger  = logger;
            _redis   = redis?.GetDatabase();
        }

        public async Task SaveAsync(QmaMeasurementEntity entity)
        {
            _context.QuantityMeasurements.Add(entity);
            await _context.SaveChangesAsync();
            await InvalidateCacheAsync(entity.UserId);
        }

        public async Task<IReadOnlyList<QmaMeasurementEntity>> GetAllByUserAsync(int userId)
        {
            var cacheKey = UserCacheKey(userId);
            if (_redis is not null)
            {
                try
                {
                    var cached = await _redis.StringGetAsync(cacheKey);
                    if (cached.HasValue)
                    {
                        var list = JsonSerializer.Deserialize<List<QmaMeasurementEntity>>((string)cached!);
                        if (list is not null) { _logger.LogDebug("Cache HIT: {Key}", cacheKey); return list; }
                    }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed."); }
            }

            var entities = await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            if (_redis is not null)
            {
                try { await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(entities), TimeSpan.FromSeconds(CacheTtlSecs)); }
                catch (Exception ex) { _logger.LogWarning(ex, "Redis write failed."); }
            }

            return entities;
        }

        public async Task<IReadOnlyList<QmaMeasurementEntity>> GetByOperationTypeAsync(string op, int userId)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.UserId == userId && e.OperationType == op.ToUpperInvariant())
                .OrderByDescending(e => e.CreatedAt).ToListAsync();

        public async Task<IReadOnlyList<QmaMeasurementEntity>> GetByCategoryAsync(string category, int userId)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.UserId == userId && e.MeasurementCategory == category.ToUpperInvariant())
                .OrderByDescending(e => e.CreatedAt).ToListAsync();

        public async Task<IReadOnlyList<QmaMeasurementEntity>> GetErrorsAsync(int userId)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.UserId == userId && e.HasError)
                .OrderByDescending(e => e.CreatedAt).ToListAsync();

        public async Task<int> GetCountByOperationAsync(string op, int userId)
            => await _context.QuantityMeasurements
                .CountAsync(e => e.UserId == userId && e.OperationType == op.ToUpperInvariant() && !e.HasError);

        public async Task<int> GetTotalCountAsync(int userId)
            => await _context.QuantityMeasurements.CountAsync(e => e.UserId == userId);

        private static string UserCacheKey(int? userId) => userId.HasValue ? $"qm:user:{userId}" : "qm:anon";

        private async Task InvalidateCacheAsync(int? userId)
        {
            if (_redis is null) return;
            try { await _redis.KeyDeleteAsync(UserCacheKey(userId)); }
            catch (Exception ex) { _logger.LogWarning(ex, "Redis invalidation failed."); }
        }
    }
}
