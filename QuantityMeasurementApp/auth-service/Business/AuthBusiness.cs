// ─────────────────────────────────────────────────────────────────────────────
// AUTH BUSINESS LAYER — Interfaces
// ─────────────────────────────────────────────────────────────────────────────

using ModelService.Auth.Dto;

namespace BusinessService.Auth.Interface
{
    public interface IUserService
    {
        Task<AuthResponseDTO> SignupAsync(SignupRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<UserResponseDTO> GetProfileAsync(int userId);
        Task<AuthResponseDTO> GoogleLoginAsync(string idToken);
    }

    public interface IJwtService
    {
        string GenerateToken(int userId, string email, string username, string role);
        (bool isValid, int userId, string email, string role) ValidateToken(string token);
        int ExpiryHours { get; }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// AUTH BUSINESS LAYER — Exceptions
// ─────────────────────────────────────────────────────────────────────────────

namespace BusinessService.Auth.Exceptions
{
    public class AuthException : Exception
    {
        public int StatusCode { get; }
        public AuthException(string message, int statusCode = 401) : base(message)
            => StatusCode = statusCode;
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// AUTH BUSINESS LAYER — JwtService
// ─────────────────────────────────────────────────────────────────────────────

namespace BusinessService.Auth.Service
{
    using BusinessService.Auth.Interface;
    using BusinessService.Auth.Exceptions;
    using ModelService.Auth.Dto;
    using ModelService.Auth.Entities;
    using RepositoryService.Auth.Interface;
    using Google.Apis.Auth;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

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
            var claims = new[]
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
                var handler    = new JwtSecurityTokenHandler();
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


        public int ExpiryHours => _expiryHours;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UserService
    // ─────────────────────────────────────────────────────────────────────────

    public class UserService : IUserService
    {
        private readonly IUserRepository      _userRepo;
        private readonly IJwtService          _jwtService;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration       _config;
        private const int WorkFactor = 12;

        public UserService(IUserRepository userRepo, IJwtService jwtService,
            ILogger<UserService> logger, IConfiguration config)
        {
            _userRepo   = userRepo;
            _jwtService = jwtService;
            _logger     = logger;
            _config     = config;
        }

        public async Task<AuthResponseDTO> SignupAsync(SignupRequestDTO request)
        {
            if (await _userRepo.ExistsByEmailAsync(request.Email))
                throw new AuthException("An account with this email already exists.", 409);
            if (await _userRepo.ExistsByUsernameAsync(request.Username))
                throw new AuthException("This username is already taken.", 409);

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
            return BuildAuthResponse(created);
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email.ToLowerInvariant())
                       ?? throw new AuthException("Invalid email or password.", 401);

            if (!user.IsActive)
                throw new AuthException("Account is disabled.", 403);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new AuthException("Invalid email or password.", 401);

            await _userRepo.UpdateLastLoginAsync(user.Id);
            _logger.LogInformation("Login: {Email}", user.Email);
            return BuildAuthResponse(user);
        }

        public async Task<UserResponseDTO> GetProfileAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new NotFoundException($"User {userId} not found.");
            return new UserResponseDTO
            {
                Id        = user.Id,
                Username  = user.Username,
                Email     = user.Email,
                Role      = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<AuthResponseDTO> GoogleLoginAsync(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Google:ClientId"] }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new AuthException("Invalid Google token.", 401);
            }

            var email = payload.Email.ToLowerInvariant();
            var user  = await _userRepo.GetByEmailAsync(email);

            if (user == null)
            {
                var username     = payload.Name?.Replace(" ", "_").ToLower() ?? email.Split('@')[0];
                var baseUsername = username;
                int suffix = 1;
                while (await _userRepo.ExistsByUsernameAsync(username))
                    username = $"{baseUsername}_{suffix++}";

                user = new UserEntity
                {
                    Username     = username,
                    Email        = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString(), WorkFactor),
                    Role         = "User",
                    IsActive     = true
                };
                user = await _userRepo.CreateAsync(user);
                _logger.LogInformation("Google signup: {Email}", email);
            }
            else if (!user.IsActive)
                throw new AuthException("Account is disabled.", 403);

            await _userRepo.UpdateLastLoginAsync(user.Id);
            return BuildAuthResponse(user);
        }

        private AuthResponseDTO BuildAuthResponse(UserEntity user) => new()
        {
            Token     = _jwtService.GenerateToken(user.Id, user.Email, user.Username, user.Role),
            Username  = user.Username,
            Email     = user.Email,
            Role      = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(_jwtService.ExpiryHours)
        };
    }
}
