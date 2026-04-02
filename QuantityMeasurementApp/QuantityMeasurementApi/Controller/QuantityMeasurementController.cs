using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;
using System.Security.Claims;

namespace QuantityMeasurementApi.Controller
{
    /// <summary>
    /// PUBLIC operations (compare / convert / add / subtract / divide):
    ///   - No JWT required — anyone can call them.
    ///   - If a valid JWT is present, the userId is extracted and stored on the record
    ///     so the operation appears in that user's history.
    ///   - If no JWT, userId = null — record is saved but never shown in anyone's history.
    ///
    /// PROTECTED history endpoints:
    ///   - [Authorize] — must have a valid JWT.
    ///   - Only return records belonging to the authenticated user.
    /// </summary>
    [ApiController]
    [Route("api/v1/quantities")]
    [Produces("application/json")]
    public class QuantityMeasurementController : ControllerBase
    {
        private readonly IQuantityMeasurementApiService _service;
        private readonly ILogger<QuantityMeasurementController> _logger;

        public QuantityMeasurementController(
            IQuantityMeasurementApiService service,
            ILogger<QuantityMeasurementController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // ── Helper: extract userId from JWT claims (null if anonymous) ────
        private int? GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("UserId");
            return int.TryParse(claim, out var id) ? id : null;
        }

        // ── Helper: extract userId from JWT, throw 401 if missing ─────────
        private int RequireUserId()
        {
            var id = GetCurrentUserId();
            if (!id.HasValue)
                throw new UnauthorizedAccessException("Authentication required.");
            return id.Value;
        }

        // ── PUBLIC OPERATIONS ─────────────────────────────────────────────

        [HttpPost("compare")]
        [AllowAnonymous]
        public async Task<IActionResult> Compare([FromBody] QuantityInputDTO input)
        {
            var userId = GetCurrentUserId();   // null if no token
            var result = await _service.CompareAsync(input, userId);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        [HttpPost("convert")]
        [AllowAnonymous]
        public async Task<IActionResult> Convert([FromBody] ConvertRequestDTO input)
        {
            var userId = GetCurrentUserId();
            var result = await _service.ConvertAsync(input, userId);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] QuantityInputDTO input)
        {
            var userId = GetCurrentUserId();
            var result = await _service.AddAsync(input, userId);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        [HttpPost("subtract")]
        [AllowAnonymous]
        public async Task<IActionResult> Subtract([FromBody] QuantityInputDTO input)
        {
            var userId = GetCurrentUserId();
            var result = await _service.SubtractAsync(input, userId);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        [HttpPost("divide")]
        [AllowAnonymous]
        public async Task<IActionResult> Divide([FromBody] QuantityInputDTO input)
        {
            var userId = GetCurrentUserId();
            var result = await _service.DivideAsync(input, userId);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        // ── PROTECTED HISTORY (own records only) ──────────────────────────

        [HttpGet("history/all")]
        [Authorize]
        public async Task<IActionResult> GetAllHistory()
        {
            var userId = RequireUserId();
            var result = await _service.GetAllHistoryAsync(userId);
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("history/operation/{operationType}")]
        [Authorize]
        public async Task<IActionResult> GetHistoryByOperation([FromRoute] string operationType)
        {
            var userId = RequireUserId();
            var result = await _service.GetHistoryByOperationAsync(operationType, userId);
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("history/category/{category}")]
        [Authorize]
        public async Task<IActionResult> GetHistoryByCategory([FromRoute] string category)
        {
            var userId = RequireUserId();
            var result = await _service.GetHistoryByCategoryAsync(category, userId);
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("history/errors")]
        [Authorize]
        public async Task<IActionResult> GetErrorHistory()
        {
            var userId = RequireUserId();
            var result = await _service.GetErrorHistoryAsync(userId);
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("count/{operationType}")]
        [Authorize]
        public async Task<IActionResult> GetOperationCount([FromRoute] string operationType)
        {
            var userId = RequireUserId();
            var count  = await _service.GetOperationCountAsync(operationType, userId);
            return Ok(ApiResponse<int>.Ok(count,
                $"Successful {operationType.ToUpperInvariant()} operations: {count}"));
        }
    }
}
