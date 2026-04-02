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
    /// EF Core + optional Redis implementation.
    /// All query methods now filter by UserId so each user sees only their own records.
    /// Save stores UserId on the entity (null for anonymous operations).
    /// Redis cache key includes userId for per-user isolation.
    /// </summary>
    public class QuantityMeasurementApiRepository : IQuantityMeasurementApiRepository
    {
        private readonly QuantityMeasurementDbContext               _context;
        private readonly ILogger<QuantityMeasurementApiRepository>  _logger;
        private readonly IDatabase?                                  _redis;
        private const int CacheTtlSecs = 300;

        public QuantityMeasurementApiRepository(
            QuantityMeasurementDbContext context,
            ILogger<QuantityMeasurementApiRepository> logger,
            IConnectionMultiplexer? redis = null)
        {
            _context = context;
            _logger  = logger;
            _redis   = redis?.GetDatabase();
        }

        // ── Write ─────────────────────────────────────────────────────────
        public async Task SaveAsync(QuantityMeasurementApiEntity entity)
        {
            _context.QuantityMeasurements.Add(entity);
            await _context.SaveChangesAsync();
            // Invalidate this user's cache (or global if anonymous)
            await InvalidateCacheAsync(entity.UserId);
            _logger.LogDebug("Saved {Op} entity id={Id} userId={Uid}",
                entity.OperationType, entity.Id, entity.UserId?.ToString() ?? "anon");
        }

        // ── Per-user reads ────────────────────────────────────────────────
        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetAllByUserAsync(int userId)
        {
            var cacheKey = UserCacheKey(userId);
            if (_redis is not null)
            {
                try
                {
                    var cached = await _redis.StringGetAsync(cacheKey);
                    if (cached.HasValue)
                    {
                        var list = JsonSerializer.Deserialize<List<QuantityMeasurementApiEntity>>((string)cached!);
                        if (list is not null) { _logger.LogDebug("Cache HIT: {Key}", cacheKey); return list; }
                    }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed."); }
            }

            var entities = await _context.QuantityMeasurements
                .AsNoTracking()
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            if (_redis is not null)
            {
                try
                {
                    await _redis.StringSetAsync(cacheKey,
                        JsonSerializer.Serialize(entities),
                        TimeSpan.FromSeconds(CacheTtlSecs));
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Redis write failed."); }
            }

            return entities;
        }

        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByOperationTypeAsync(
            string op, int userId)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.UserId == userId && e.OperationType == op.ToUpperInvariant())
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByCategoryAsync(
            string category, int userId)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.UserId == userId && e.MeasurementCategory == category.ToUpperInvariant())
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        public async Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetErrorsAsync(int userId)
            => await _context.QuantityMeasurements.AsNoTracking()
                .Where(e => e.UserId == userId && e.HasError)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        public async Task<int> GetCountByOperationAsync(string op, int userId)
            => await _context.QuantityMeasurements
                .CountAsync(e => e.UserId == userId
                              && e.OperationType == op.ToUpperInvariant()
                              && !e.HasError);

        public async Task<int> GetTotalCountAsync(int userId)
            => await _context.QuantityMeasurements
                .CountAsync(e => e.UserId == userId);

        // ── Cache helpers ─────────────────────────────────────────────────
        private static string UserCacheKey(int? userId)
            => userId.HasValue ? $"qm:user:{userId}" : "qm:anon";

        private async Task InvalidateCacheAsync(int? userId)
        {
            if (_redis is null) return;
            try { await _redis.KeyDeleteAsync(UserCacheKey(userId)); }
            catch (Exception ex) { _logger.LogWarning(ex, "Redis invalidation failed."); }
        }
    }

    /// <summary>EF Core implementation of IUserRepository — unchanged.</summary>
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
            user.Email     = user.Email.ToLowerInvariant();
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created user: {Email}", user.Email);
            return user;
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
