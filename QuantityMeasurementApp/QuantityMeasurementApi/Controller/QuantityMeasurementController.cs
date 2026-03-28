using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementApi.Controller
{
    /// <summary>
    /// UC17: REST controller for all quantity measurement operations.
    /// All endpoints require a valid JWT Bearer token.
    /// Base URL: /api/v1/quantities
    ///
    /// POST /compare                          → Compare two quantities
    /// POST /convert                          → Convert to different unit
    /// POST /add                              → Add two quantities
    /// POST /subtract                         → Subtract quantities
    /// POST /divide                           → Divide (returns SCALAR ratio)
    /// GET  /history/operation/{opType}       → Records by operation type
    /// GET  /history/category/{category}      → Records by measurement category
    /// GET  /history/errors                   → Error audit trail
    /// GET  /count/{opType}                   → Successful operation count
    /// </summary>
    [ApiController]
    [Route("api/v1/quantities")]
    [Authorize]
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

        // ── POST /api/v1/quantities/compare ───────────────────────────────

        /// <summary>Compare two quantities for equality.</summary>
        /// <remarks>
        /// Both quantities normalised to base unit before comparison.
        /// ResultValue=1 (EQUAL) or 0 (NOT_EQUAL).
        ///
        ///     POST /api/v1/quantities/compare
        ///     {
        ///       "thisQuantityDTO": { "value": 1.0,  "unitName": "FEET",   "category": "LENGTH" },
        ///       "thatQuantityDTO": { "value": 12.0, "unitName": "INCHES", "category": "LENGTH" }
        ///     }
        /// </remarks>
        [HttpPost("compare")]
        [ProducesResponseType(typeof(ApiResponse<QuantityMeasurementDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        public async Task<IActionResult> Compare([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("COMPARE {V1}{U1} vs {V2}{U2}",
                input.ThisQuantityDTO.Value, input.ThisQuantityDTO.UnitName,
                input.ThatQuantityDTO.Value, input.ThatQuantityDTO.UnitName);
            var result = await _service.CompareAsync(input);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        // ── POST /api/v1/quantities/convert ───────────────────────────────

        /// <summary>Convert a quantity to a different unit within the same category.</summary>
        /// <remarks>
        ///     POST /api/v1/quantities/convert
        ///     {
        ///       "thisQuantityDTO": { "value": 1.0, "unitName": "FEET", "category": "LENGTH" },
        ///       "targetUnit": "INCHES"
        ///     }
        /// </remarks>
        [HttpPost("convert")]
        [ProducesResponseType(typeof(ApiResponse<QuantityMeasurementDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        public async Task<IActionResult> Convert([FromBody] ConvertRequestDTO input)
        {
            _logger.LogInformation("CONVERT {V}{U} → {T}",
                input.ThisQuantityDTO.Value, input.ThisQuantityDTO.UnitName, input.TargetUnit);
            var result = await _service.ConvertAsync(input);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        // ── POST /api/v1/quantities/add ───────────────────────────────────

        /// <summary>Add two quantities. Result expressed in first operand's unit.</summary>
        /// <remarks>
        ///     POST /api/v1/quantities/add
        ///     {
        ///       "thisQuantityDTO": { "value": 1.0,  "unitName": "FEET",   "category": "LENGTH" },
        ///       "thatQuantityDTO": { "value": 12.0, "unitName": "INCHES", "category": "LENGTH" }
        ///     }
        /// </remarks>
        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse<QuantityMeasurementDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        public async Task<IActionResult> Add([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("ADD {V1}{U1} + {V2}{U2}",
                input.ThisQuantityDTO.Value, input.ThisQuantityDTO.UnitName,
                input.ThatQuantityDTO.Value, input.ThatQuantityDTO.UnitName);
            var result = await _service.AddAsync(input);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        // ── POST /api/v1/quantities/subtract ──────────────────────────────

        /// <summary>Subtract second quantity from first. Result in first operand's unit.</summary>
        [HttpPost("subtract")]
        [ProducesResponseType(typeof(ApiResponse<QuantityMeasurementDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        public async Task<IActionResult> Subtract([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("SUBTRACT {V1}{U1} - {V2}{U2}",
                input.ThisQuantityDTO.Value, input.ThisQuantityDTO.UnitName,
                input.ThatQuantityDTO.Value, input.ThatQuantityDTO.UnitName);
            var result = await _service.SubtractAsync(input);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        // ── POST /api/v1/quantities/divide ────────────────────────────────

        /// <summary>Divide first by second. Returns a dimensionless ratio (SCALAR).</summary>
        [HttpPost("divide")]
        [ProducesResponseType(typeof(ApiResponse<QuantityMeasurementDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
        public async Task<IActionResult> Divide([FromBody] QuantityInputDTO input)
        {
            _logger.LogInformation("DIVIDE {V1}{U1} / {V2}{U2}",
                input.ThisQuantityDTO.Value, input.ThisQuantityDTO.UnitName,
                input.ThatQuantityDTO.Value, input.ThatQuantityDTO.UnitName);
            var result = await _service.DivideAsync(input);
            return Ok(ApiResponse<QuantityMeasurementDTO>.Ok(result));
        }

        // ── GET history / count ───────────────────────────────────────────

        /// <summary>Get all records for a given operation type (COMPARE/CONVERT/ADD/SUBTRACT/DIVIDE).</summary>
        [HttpGet("history/operation/{operationType}")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>), 200)]
        public async Task<IActionResult> GetHistoryByOperation([FromRoute] string operationType)
        {
            var result = await _service.GetHistoryByOperationAsync(operationType);
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        /// <summary>Get all records for a given measurement category (LENGTH/WEIGHT/VOLUME/TEMPERATURE).</summary>
        [HttpGet("history/category/{category}")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>), 200)]
        public async Task<IActionResult> GetHistoryByCategory([FromRoute] string category)
        {
            var result = await _service.GetHistoryByCategoryAsync(category);
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        /// <summary>Get all records that resulted in errors (audit trail).</summary>
        [HttpGet("history/errors")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>), 200)]
        public async Task<IActionResult> GetErrorHistory()
        {
            var result = await _service.GetErrorHistoryAsync();
            return Ok(ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>.Ok(result));
        }

        /// <summary>Get successful operation count for a given operation type.</summary>
        [HttpGet("count/{operationType}")]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        public async Task<IActionResult> GetOperationCount([FromRoute] string operationType)
        {
            var count = await _service.GetOperationCountAsync(operationType);
            return Ok(ApiResponse<int>.Ok(count,
                $"Successful {operationType.ToUpperInvariant()} operations: {count}"));
        }
    }
}
