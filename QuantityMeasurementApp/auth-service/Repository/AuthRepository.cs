// ─────────────────────────────────────────────────────────────────────────────
// AUTH REPOSITORY — DbContext
// ─────────────────────────────────────────────────────────────────────────────

namespace RepositoryService.Auth.DBContext
{
    using Microsoft.EntityFrameworkCore;
    using ModelService.Auth.Entities;

    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        public DbSet<UserEntity> Users => Set<UserEntity>();
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// AUTH REPOSITORY — Interface
// ─────────────────────────────────────────────────────────────────────────────

namespace RepositoryService.Auth.Interface
{
    using ModelService.Auth.Entities;

    public interface IUserRepository
    {
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetByIdAsync(int id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<UserEntity> CreateAsync(UserEntity user);
        Task UpdateLastLoginAsync(int userId);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// AUTH REPOSITORY — Redis User Cache
// ─────────────────────────────────────────────────────────────────────────────

namespace RepositoryService.Auth.Cache
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using ModelService.Auth.Entities;
    using StackExchange.Redis;
    using System.Text.Json;

    public interface IUserCache
    {
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetByIdAsync(int id);
        Task SetAsync(UserEntity user);
        Task InvalidateAsync(string email, int? id = null);
    }

    // No-op fallback when Redis is not configured
    public sealed class NullUserCache : IUserCache
    {
        public Task<UserEntity?> GetByEmailAsync(string email)      => Task.FromResult<UserEntity?>(null);
        public Task<UserEntity?> GetByIdAsync(int id)               => Task.FromResult<UserEntity?>(null);
        public Task SetAsync(UserEntity user)                        => Task.CompletedTask;
        public Task InvalidateAsync(string email, int? id = null)   => Task.CompletedTask;
    }

    // Redis-backed cache — read-through, write-through, invalidate on mutation
    public sealed class RedisUserCache : IUserCache
    {
        private readonly IDatabase              _db;
        private readonly ILogger<RedisUserCache> _logger;
        private readonly TimeSpan               _ttl;

        private static readonly JsonSerializerOptions _json =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public RedisUserCache(IConnectionMultiplexer mux, IConfiguration config,
                              ILogger<RedisUserCache> logger)
        {
            _db     = mux.GetDatabase();
            _logger = logger;
            _ttl = TimeSpan.FromSeconds(int.TryParse(config["Cache:UserTtlSeconds"], out var ttl) ? ttl : 600);
        }

        public async Task<UserEntity?> GetByEmailAsync(string email) => string.IsNullOrWhiteSpace(email) ? null : await GetAsync(EmailKey(email));
        public async Task<UserEntity?> GetByIdAsync(int id)          => await GetAsync(IdKey(id));

        public async Task SetAsync(UserEntity user)
        {
            var json = JsonSerializer.Serialize(user, _json);
            try
            {
                var batch = _db.CreateBatch();
                var t1 = batch.StringSetAsync(EmailKey(user.Email), json, _ttl);
                var t2 = batch.StringSetAsync(IdKey(user.Id),       json, _ttl);
                batch.Execute();
                await Task.WhenAll(t1, t2);
                _logger.LogDebug("Cache SET user email={Email} id={Id}", user.Email, user.Id);
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Redis SET failed for {Email}", user.Email); }
        }

        public async Task InvalidateAsync(string email, int? id = null)
        {
            try
            {
                var keys = new List<RedisKey> { EmailKey(email) };
                if (id.HasValue) keys.Add(IdKey(id.Value));
                await _db.KeyDeleteAsync(keys.ToArray());
                _logger.LogDebug("Cache INVALIDATE email={Email} id={Id}", email, id?.ToString() ?? "-");
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Redis INVALIDATE failed for {Email}", email); }
        }

        private async Task<UserEntity?> GetAsync(RedisKey key)
        {
            try
            {
                var raw = await _db.StringGetAsync(key);
                if (!raw.HasValue) return null;
                _logger.LogDebug("Cache HIT key={Key}", key);
                var jsonStr = raw.ToString();
                return string.IsNullOrEmpty(jsonStr) ? null : JsonSerializer.Deserialize<UserEntity>(jsonStr, _json);
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Redis GET failed for key {Key}", key); return null; }
        }

        private static RedisKey EmailKey(string email) => $"auth:user:email:{email.ToLowerInvariant()}";
        private static RedisKey IdKey(int id)          => $"auth:user:id:{id}";
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// AUTH REPOSITORY — Implementation (EF Core + Redis cache)
// ─────────────────────────────────────────────────────────────────────────────

namespace RepositoryService.Auth.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using ModelService.Auth.Entities;
    using RepositoryService.Auth.Cache;
    using RepositoryService.Auth.DBContext;
    using RepositoryService.Auth.Interface;

    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext           _context;
        private readonly ILogger<UserRepository> _logger;
        private readonly IUserCache              _cache;

        public UserRepository(AuthDbContext context, ILogger<UserRepository> logger, IUserCache cache)
        {
            _context = context;
            _logger  = logger;
            _cache   = cache;
        }

        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            var normalised = email.ToLowerInvariant();
            var cached = await _cache.GetByEmailAsync(normalised);
            if (cached is not null) return cached;

            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == normalised);
            if (user is not null) await _cache.SetAsync(user);
            return user;
        }

        public async Task<UserEntity?> GetByIdAsync(int id)
        {
            var cached = await _cache.GetByIdAsync(id);
            if (cached is not null) return cached;

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (user is not null) await _cache.SetAsync(user);
            return user;
        }

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
            await _cache.SetAsync(user); // warm cache
            _logger.LogInformation("Created user: {Email} id={Id}", user.Email, user.Id);
            return user;
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await _cache.InvalidateAsync(user.Email, user.Id); // force fresh read next time
            }
        }
    }
}
