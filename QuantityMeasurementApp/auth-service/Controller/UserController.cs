using BusinessService.Auth.Exceptions;
using BusinessService.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService.Auth.Dto;
using System.Security.Claims;

namespace AuthService.Controller
{
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService          _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger      = logger;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDTO request)
        {
            try
            {
                var result = await _userService.SignupAsync(request);
                _logger.LogInformation("Signup: {Email}", request.Email);
                return StatusCode(201, ApiResponse<AuthResponseDTO>.Ok(result, "Account created successfully."));
            }
            catch (AuthException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var result = await _userService.LoginAsync(request);
                _logger.LogInformation("Login: {Email}", request.Email);
                return Ok(ApiResponse<AuthResponseDTO>.Ok(result, "Login successful."));
            }
            catch (AuthException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDTO request)
        {
            try
            {
                var result = await _userService.GoogleLoginAsync(request.IdToken);
                _logger.LogInformation("Google Login: success");
                return Ok(ApiResponse<AuthResponseDTO>.Ok(result, "Google login successful."));
            }
            catch (AuthException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token claims."));

            try
            {
                var result = await _userService.GetProfileAsync(userId);
                return Ok(ApiResponse<UserResponseDTO>.Ok(result));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}
