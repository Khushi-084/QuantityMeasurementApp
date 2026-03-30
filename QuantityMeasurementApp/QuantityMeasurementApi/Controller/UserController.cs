using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;
using System.Security.Claims;

namespace QuantityMeasurementApi.Controller
{
    /// <summary>
    /// UC17: User authentication REST controller.
    /// POST /api/v1/users/signup  → Register (no token needed)
    /// POST /api/v1/users/login   → Authenticate, receive JWT (no token needed)
    /// GET  /api/v1/users/profile → View own profile (JWT required)
    /// </summary>
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger      = logger;
        }

        /// <summary>Register a new user account.</summary>
        /// <remarks>
        /// Password: min 8 chars, must include uppercase, lowercase, digit and special character.
        /// Stored as BCrypt hash (work factor 12) — never plain text.
        ///
        ///     POST /api/v1/users/signup
        ///     { "username": "john_doe", "email": "john@example.com", "password": "SecureP@ss1" }
        /// </remarks>
        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), 201)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 409)]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDTO request)
        {
            var result = await _userService.SignupAsync(request);
            _logger.LogInformation("Signup: {Email}", request.Email);
            return StatusCode(201, ApiResponse<AuthResponseDTO>.Ok(result, "Account created successfully."));
        }

        /// <summary>Login and receive a JWT Bearer token.</summary>
        /// <remarks>
        /// Use the returned token in all protected endpoints:
        /// Authorization: Bearer {token}
        ///
        ///     POST /api/v1/users/login
        ///     { "email": "john@example.com", "password": "SecureP@ss1" }
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var result = await _userService.LoginAsync(request);
            _logger.LogInformation("Login: {Email}", request.Email);
            return Ok(ApiResponse<AuthResponseDTO>.Ok(result, "Login successful."));
        }

        /// <summary>Get the authenticated user's profile.</summary>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 401)]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token claims."));

            var result = await _userService.GetProfileAsync(userId);
            return Ok(ApiResponse<UserResponseDTO>.Ok(result));
        }
    }
}
