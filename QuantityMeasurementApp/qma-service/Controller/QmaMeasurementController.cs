using BusinessService.Qma.Exceptions;
using BusinessService.Qma.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService.Qma.Dto;
using System.Security.Claims;

namespace QmaService.Controller
{
    [ApiController]
    [Route("api/v1/quantities")]
    [Produces("application/json")]
    public class QmaMeasurementController : ControllerBase
    {
        private readonly IQmaService                        _service;
        private readonly ILogger<QmaMeasurementController> _logger;

        public QmaMeasurementController(IQmaService service, ILogger<QmaMeasurementController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId");
            return int.TryParse(claim, out var id) ? id : null;
        }

        private int RequireUserId()
        {
            var id = GetCurrentUserId();
            if (!id.HasValue) throw new UnauthorizedAccessException("Authentication required.");
            return id.Value;
        }

        // ── PUBLIC OPERATIONS ─────────────────────────────────────────────

        [HttpPost("compare")]
        [AllowAnonymous]
        public async Task<IActionResult> Compare([FromBody] QuantityInputDTO input)
        {
            try
            {
                var result = await _service.CompareAsync(input, GetCurrentUserId());
                return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
            }
            catch (QmaMeasurementException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        }

        [HttpPost("convert")]
        [AllowAnonymous]
        public async Task<IActionResult> Convert([FromBody] ConvertRequestDTO input)
        {
            try
            {
                var result = await _service.ConvertAsync(input, GetCurrentUserId());
                return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
            }
            catch (QmaMeasurementException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        }

        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] QuantityInputDTO input)
        {
            try
            {
                var result = await _service.AddAsync(input, GetCurrentUserId());
                return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
            }
            catch (QmaMeasurementException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        }

        [HttpPost("subtract")]
        [AllowAnonymous]
        public async Task<IActionResult> Subtract([FromBody] QuantityInputDTO input)
        {
            try
            {
                var result = await _service.SubtractAsync(input, GetCurrentUserId());
                return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
            }
            catch (QmaMeasurementException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        }

        [HttpPost("divide")]
        [AllowAnonymous]
        public async Task<IActionResult> Divide([FromBody] QuantityInputDTO input)
        {
            try
            {
                var result = await _service.DivideAsync(input, GetCurrentUserId());
                return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
            }
            catch (QmaMeasurementException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        }

        // ── PROTECTED HISTORY ─────────────────────────────────────────────

        [HttpGet("history/all")]
        [Authorize]
        public async Task<IActionResult> GetAllHistory()
        {
            var result = await _service.GetAllHistoryAsync(RequireUserId());
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("history/operation/{operationType}")]
        [Authorize]
        public async Task<IActionResult> GetHistoryByOperation([FromRoute] string operationType)
        {
            var result = await _service.GetHistoryByOperationAsync(operationType, RequireUserId());
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("history/category/{category}")]
        [Authorize]
        public async Task<IActionResult> GetHistoryByCategory([FromRoute] string category)
        {
            var result = await _service.GetHistoryByCategoryAsync(category, RequireUserId());
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("history/errors")]
        [Authorize]
        public async Task<IActionResult> GetErrorHistory()
        {
            var result = await _service.GetErrorHistoryAsync(RequireUserId());
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        [HttpGet("count/{operationType}")]
        [Authorize]
        public async Task<IActionResult> GetOperationCount([FromRoute] string operationType)
        {
            var count = await _service.GetOperationCountAsync(operationType, RequireUserId());
            return Ok(ApiResponse<int>.Ok(count,
                $"Successful {operationType.ToUpperInvariant()} operations: {count}"));
        }
    }
}
