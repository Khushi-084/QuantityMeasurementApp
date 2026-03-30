using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Exception;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Common;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuantityMeasurementBusinessLayer.Service
{
    // ═══════════════════════════════════════════════════════════════════════
    // JWT SERVICE
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// UC17: JWT token generation and validation.
    /// Algorithm: HMAC-SHA256. Claims: sub (userId), email, unique_name, role.
    /// Registered as Singleton — stateless, thread-safe.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int    _expiryHours;

        public JwtService(IConfiguration config)
        {
            _key         = config["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key not configured.");
            _issuer      = config["Jwt:Issuer"]   ?? "QuantityMeasurementApi";
            _audience    = config["Jwt:Audience"] ?? "QuantityMeasurementApi";
            _expiryHours = int.TryParse(config["Jwt:ExpiryHours"], out var h) ? h : 24;
        }

        public string GenerateToken(int userId, string email, string username, string role)
        {
            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims      = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,        userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,      email),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Role,                    role),
                new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                _issuer, _audience, claims,
                notBefore: DateTime.UtcNow,
                expires:   DateTime.UtcNow.AddHours(_expiryHours),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (bool isValid, int userId, string email, string role) ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                    ValidateIssuer           = true, ValidIssuer   = _issuer,
                    ValidateAudience         = true, ValidAudience = _audience,
                    ValidateLifetime         = true, ClockSkew     = TimeSpan.Zero
                };
                var principal = handler.ValidateToken(token, parameters, out _);
                var userId    = int.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "0");
                var email     = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value         ?? "";
                var role      = principal.FindFirst(ClaimTypes.Role)?.Value                       ?? "";
                return (true, userId, email, role);
            }
            catch { return (false, 0, string.Empty, string.Empty); }
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // USER SERVICE
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// UC17: User authentication service.
    /// Signup: BCrypt.HashPassword(password, workFactor=12) — auto-generates and embeds salt.
    /// Login:  BCrypt.Verify(candidate, storedHash) — constant-time comparison.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository     _userRepo;
        private readonly IJwtService         _jwtService;
        private readonly ILogger<UserService> _logger;
        private const int WorkFactor = 12;

        public UserService(IUserRepository userRepo, IJwtService jwtService, ILogger<UserService> logger)
        {
            _userRepo   = userRepo;
            _jwtService = jwtService;
            _logger     = logger;
        }

        public async Task<AuthResponseDTO> SignupAsync(SignupRequestDTO request)
        {
            if (await _userRepo.ExistsByEmailAsync(request.Email))
                throw new AuthException("An account with this email already exists.", 409);
            if (await _userRepo.ExistsByUsernameAsync(request.Username))
                throw new AuthException("This username is already taken.", 409);

            // BCrypt generates unique random salt per call and embeds it in the hash string.
            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password, WorkFactor);

            var user = new UserEntity
            {
                Username     = request.Username.Trim(),
                Email        = request.Email.Trim().ToLowerInvariant(),
                PasswordHash = hash,
                Role         = "User"
            };
            var created = await _userRepo.CreateAsync(user);
            _logger.LogInformation("Registered: {Email}", created.Email);

            return new AuthResponseDTO
            {
                Token     = _jwtService.GenerateToken(created.Id, created.Email, created.Username, created.Role),
                Username  = created.Username,
                Email     = created.Email,
                Role      = created.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email.ToLowerInvariant())
                       ?? throw new AuthException("Invalid email or password.", 401);

            if (!user.IsActive)
                throw new AuthException("Account is disabled.", 403);

            // BCrypt.Verify extracts salt from stored hash, rehashes, compares in constant time.
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new AuthException("Invalid email or password.", 401);

            await _userRepo.UpdateLastLoginAsync(user.Id);
            _logger.LogInformation("Login: {Email}", user.Email);

            return new AuthResponseDTO
            {
                Token     = _jwtService.GenerateToken(user.Id, user.Email, user.Username, user.Role),
                Username  = user.Username,
                Email     = user.Email,
                Role      = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<UserResponseDTO> GetProfileAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new NotFoundException($"User {userId} not found.");
            return new UserResponseDTO
            {
                Id = user.Id, Username = user.Username,
                Email = user.Email, Role = user.Role, CreatedAt = user.CreatedAt
            };
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // ENCRYPTION SERVICE
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>UC17: AES-256-CBC encryption service. Registered as Singleton.</summary>
    public class EncryptionService : IEncryptionService
    {
        private readonly string _key;
        public EncryptionService(IConfiguration config)
            => _key = config["Encryption:Key"] ?? throw new InvalidOperationException("Encryption:Key not configured.");

        public string Encrypt(string plainText)  => AesEncryptionHelper.Encrypt(plainText, _key);
        public string Decrypt(string cipherText) => AesEncryptionHelper.Decrypt(cipherText, _key);
    }
}
