using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Exception;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// UC17: Async Web API service implementation.
    ///
    /// Reuses the EXISTING UC16 business logic (QuantityMeasurementServiceImpl)
    /// internally — wraps its sync methods to produce async QuantityMeasurementDTO results
    /// and persists via the new EF Core async repository (IQuantityMeasurementApiRepository).
    ///
    /// UC17 Data Flow:
    ///   1. Receive QuantityInputDTO / ConvertRequestDTO from controller
    ///   2. Extract QuantityDTO operands
    ///   3. Delegate to existing UC16 service (Compare/Convert/Add/Subtract/Divide)
    ///   4. Build QuantityMeasurementApiEntity from result
    ///   5. SaveAsync to EF Core repository (+ Redis cache invalidation)
    ///   6. Return QuantityMeasurementDTO to controller
    /// </summary>
    public class QuantityMeasurementApiServiceImpl : IQuantityMeasurementApiService
    {
        private readonly IQuantityMeasurementService    _uc16Service;
        private readonly IQuantityMeasurementApiRepository _repository;
        private readonly ILogger<QuantityMeasurementApiServiceImpl> _logger;

        public QuantityMeasurementApiServiceImpl(
            IQuantityMeasurementService uc16Service,
            IQuantityMeasurementApiRepository repository,
            ILogger<QuantityMeasurementApiServiceImpl> logger)
        {
            _uc16Service = uc16Service ?? throw new ArgumentNullException(nameof(uc16Service));
            _repository  = repository  ?? throw new ArgumentNullException(nameof(repository));
            _logger      = logger      ?? throw new ArgumentNullException(nameof(logger));
        }

        // ── COMPARE ───────────────────────────────────────────────────────

        public async Task<QuantityMeasurementDTO> CompareAsync(QuantityInputDTO input)
        {
            var entity = BuildEntity("COMPARE", input.ThisQuantityDTO, input.ThatQuantityDTO);
            try
            {
                var result = _uc16Service.Compare(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;

                await _repository.SaveAsync(entity);
                _logger.LogInformation("COMPARE: {V1}{U1} vs {V2}{U2} = {R}",
                    input.ThisQuantityDTO.Value, input.ThisQuantityDTO.UnitName,
                    input.ThatQuantityDTO.Value, input.ThatQuantityDTO.UnitName,
                    result.UnitName);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                await SaveErrorAsync(entity, ex.Message);
                throw new QuantityMeasurementException($"Compare failed: {ex.Message}", ex);
            }
        }

        // ── CONVERT ───────────────────────────────────────────────────────

        public async Task<QuantityMeasurementDTO> ConvertAsync(ConvertRequestDTO input)
        {
            var targetDto = new QuantityDTO(0, input.TargetUnit, input.ThisQuantityDTO.Category);
            var entity    = BuildEntity("CONVERT", input.ThisQuantityDTO, null);
            try
            {
                var result = _uc16Service.Convert(input.ThisQuantityDTO, targetDto);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;

                await _repository.SaveAsync(entity);
                _logger.LogInformation("CONVERT: {V}{U} → {R}{RU}",
                    input.ThisQuantityDTO.Value, input.ThisQuantityDTO.UnitName,
                    result.Value, result.UnitName);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                await SaveErrorAsync(entity, ex.Message);
                throw new QuantityMeasurementException($"Convert failed: {ex.Message}", ex);
            }
        }

        // ── ADD ───────────────────────────────────────────────────────────

        public async Task<QuantityMeasurementDTO> AddAsync(QuantityInputDTO input)
        {
            var entity = BuildEntity("ADD", input.ThisQuantityDTO, input.ThatQuantityDTO);
            try
            {
                var result = _uc16Service.Add(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;

                await _repository.SaveAsync(entity);
                _logger.LogInformation("ADD: {R}{U}", result.Value, result.UnitName);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                await SaveErrorAsync(entity, ex.Message);
                throw new QuantityMeasurementException($"Add failed: {ex.Message}", ex);
            }
        }

        // ── SUBTRACT ─────────────────────────────────────────────────────

        public async Task<QuantityMeasurementDTO> SubtractAsync(QuantityInputDTO input)
        {
            var entity = BuildEntity("SUBTRACT", input.ThisQuantityDTO, input.ThatQuantityDTO);
            try
            {
                var result = _uc16Service.Subtract(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;

                await _repository.SaveAsync(entity);
                _logger.LogInformation("SUBTRACT: {R}{U}", result.Value, result.UnitName);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                await SaveErrorAsync(entity, ex.Message);
                throw new QuantityMeasurementException($"Subtract failed: {ex.Message}", ex);
            }
        }

        // ── DIVIDE ────────────────────────────────────────────────────────

        public async Task<QuantityMeasurementDTO> DivideAsync(QuantityInputDTO input)
        {
            var entity = BuildEntity("DIVIDE", input.ThisQuantityDTO, input.ThatQuantityDTO);
            try
            {
                var result = _uc16Service.Divide(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;

                await _repository.SaveAsync(entity);
                _logger.LogInformation("DIVIDE: {R}{U}", result.Value, result.UnitName);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                await SaveErrorAsync(entity, ex.Message);
                throw new QuantityMeasurementException($"Divide failed: {ex.Message}", ex);
            }
        }

        // ── HISTORY / COUNT ───────────────────────────────────────────────

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByOperationAsync(
            string operationType)
        {
            var list = await _repository.GetByOperationTypeAsync(operationType.ToUpperInvariant());
            return QuantityMeasurementDTO.FromEntityList(list);
        }

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByCategoryAsync(
            string category)
        {
            var list = await _repository.GetByCategoryAsync(category.ToUpperInvariant());
            return QuantityMeasurementDTO.FromEntityList(list);
        }

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetErrorHistoryAsync()
        {
            var list = await _repository.GetErrorsAsync();
            return QuantityMeasurementDTO.FromEntityList(list);
        }

        public async Task<int> GetOperationCountAsync(string operationType)
            => await _repository.GetCountByOperationAsync(operationType.ToUpperInvariant());

        // ── PRIVATE ───────────────────────────────────────────────────────

        private static QuantityMeasurementApiEntity BuildEntity(
            string opType, QuantityDTO q1, QuantityDTO? q2) => new()
        {
            OperationType       = opType,
            MeasurementCategory = q1.Category,
            Operand1Value       = q1.Value,
            Operand1Unit        = q1.UnitName,
            Operand2Value       = q2?.Value,
            Operand2Unit        = q2?.UnitName
        };

        private async Task SaveErrorAsync(QuantityMeasurementApiEntity entity, string msg)
        {
            entity.HasError = true; entity.ErrorMessage = msg;
            try { await _repository.SaveAsync(entity); }
            catch (System.Exception ex) { _logger.LogError(ex, "Failed to save error entity."); }
        }
    }
}