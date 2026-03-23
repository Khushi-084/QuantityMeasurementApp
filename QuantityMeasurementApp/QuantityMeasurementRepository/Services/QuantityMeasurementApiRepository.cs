using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Database;
using QuantityMeasurementRepository.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace QuantityMeasurementRepository.Services
{
    /// <summary>
    /// UC17: EF Core + Redis implementation of IQuantityMeasurementApiRepository.
    /// Read: Redis cache-first → SQL Server fallback.
    /// Write: SQL Server → invalidate Redis cache.
    /// UC16's ADO.NET QuantityMeasurementDatabaseRepository is preserved unchanged.
    /// </summary>
    public class QuantityMeasurementApiRepository : IQuantityMeasurementApiRepository
    {
        private readonly QuantityMeasurementDbContext                _context;
        private readonly ILogger<QuantityMeasurementApiRepository>   _logger;
        private readonly IDatabase?                                   _redis;

        private const string AllCacheKey  = "qm:all";
        private const int    CacheTtlSecs = 300;

        public QuantityMeasurementApiRepository(
            QuantityMeasurementDbContext context,
            ILogger<QuantityMeasurementApiRepository> logger,
            IConnectionMultiplexer? redis = null)
        {
            _context = context;
            _logger  = logger;
            _redis   = redis?.GetDatabase();
        }

        public async Task SaveAsync(QuantityMeasurementApiEntity entity)
        {
            _context.QuantityMeasurements.Add(entity);
            await _context.SaveChangesAsync();
            await InvalidateCacheAsync();
            _logger.LogDebug("Saved {Op} entity id={Id}", entity.OperationType, entity.Id);
        }

        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetAllAsync()
        {
            if (_redis is not null)
            {
                try
                {
                    var cached = await _redis.StringGetAsync(AllCacheKey);
                    if (cached.HasValue)
                    {
                        var list = JsonSerializer.Deserialize<List<QuantityMeasurementApiEntity>>((string)cached!);
                        if (list is not null) { _logger.LogDebug("Cache HIT: qm:all"); return list; }
                    }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed."); }
            }

            var entities = await _context.QuantityMeasurements
                .AsNoTracking().OrderByDescending(e => e.CreatedAt).ToListAsync();

            if (_redis is not null)
            {
                try
                {
                    await _redis.StringSetAsync(AllCacheKey,
                        JsonSerializer.Serialize(entities), TimeSpan.FromSeconds(CacheTtlSecs));
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Redis write failed."); }
            }
            return entities;
        }

        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByOperationTypeAsync(string op)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.OperationType == op.ToUpperInvariant())
                .OrderByDescending(e => e.CreatedAt).ToListAsync();

        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByCategoryAsync(string category)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.MeasurementCategory == category.ToUpperInvariant())
                .OrderByDescending(e => e.CreatedAt).ToListAsync();

        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetErrorsAsync()
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.HasError).OrderByDescending(e => e.CreatedAt).ToListAsync();

        public async Task<int> GetCountByOperationAsync(string op)
            => await _context.QuantityMeasurements
                .CountAsync(e => e.OperationType == op.ToUpperInvariant() && !e.HasError);

        public async Task<int> GetTotalCountAsync()
            => await _context.QuantityMeasurements.CountAsync();

        private async Task InvalidateCacheAsync()
        {
            if (_redis is null) return;
            try { await _redis.KeyDeleteAsync(AllCacheKey); }
            catch (Exception ex) { _logger.LogWarning(ex, "Redis invalidation failed."); }
        }
    }

    /// <summary>UC17: EF Core implementation of IUserRepository.</summary>
    public class UserRepository : IUserRepository
    {
        private readonly QuantityMeasurementDbContext _context;
        private readonly ILogger<UserRepository>      _logger;

        public UserRepository(QuantityMeasurementDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger  = logger;
        }

        public async Task<UserEntity?> GetByEmailAsync(string email)
            => await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        public async Task<UserEntity?> GetByIdAsync(int id)
            => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task<bool> ExistsByEmailAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant());

        public async Task<bool> ExistsByUsernameAsync(string username)
            => await _context.Users.AnyAsync(u => u.Username == username.Trim());

        public async Task<UserEntity> CreateAsync(UserEntity user)
        {
            user.Email = user.Email.ToLowerInvariant();
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created user: {Email}", user.Email);
            return user;
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is not null) { user.LastLoginAt = DateTime.UtcNow; await _context.SaveChangesAsync(); }
        }
    }
}
