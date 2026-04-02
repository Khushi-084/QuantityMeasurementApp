using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Exception;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// Async Web API service.
    /// Operations accept an optional userId:
    ///   - When the caller is logged in, userId is set on the saved entity.
    ///   - When anonymous (no token), userId is null — record is still saved
    ///     but will NOT appear in any user's history.
    /// History methods always require a userId and only return that user's records.
    /// </summary>
    public class QuantityMeasurementApiServiceImpl : IQuantityMeasurementApiService
    {
        private readonly IQuantityMeasurementService       _uc16Service;
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

        // ── COMPARE ──────────────────────────────────────────────────────
        public async Task<QuantityMeasurementDTO> CompareAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("COMPARE", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                var result = _uc16Service.Compare(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;
                await _repository.SaveAsync(entity);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                await SaveErrorAsync(entity, ex.Message);
                throw new QuantityMeasurementException($"Compare failed: {ex.Message}", ex);
            }
        }

        // ── CONVERT ──────────────────────────────────────────────────────
        public async Task<QuantityMeasurementDTO> ConvertAsync(ConvertRequestDTO input, int? userId = null)
        {
            var targetDto = new QuantityDTO(0, input.TargetUnit, input.ThisQuantityDTO.Category);
            var entity    = BuildEntity("CONVERT", input.ThisQuantityDTO, null, userId);
            try
            {
                var result = _uc16Service.Convert(input.ThisQuantityDTO, targetDto);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;
                await _repository.SaveAsync(entity);
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
        public async Task<QuantityMeasurementDTO> AddAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("ADD", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                var result = _uc16Service.Add(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;
                await _repository.SaveAsync(entity);
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
        public async Task<QuantityMeasurementDTO> SubtractAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("SUBTRACT", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                var result = _uc16Service.Subtract(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;
                await _repository.SaveAsync(entity);
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
        public async Task<QuantityMeasurementDTO> DivideAsync(QuantityInputDTO input, int? userId = null)
        {
            var entity = BuildEntity("DIVIDE", input.ThisQuantityDTO, input.ThatQuantityDTO, userId);
            try
            {
                var result = _uc16Service.Divide(input.ThisQuantityDTO, input.ThatQuantityDTO);
                entity.ResultValue    = result.Value;
                entity.ResultUnit     = result.UnitName;
                entity.ResultCategory = result.Category;
                await _repository.SaveAsync(entity);
                return QuantityMeasurementDTO.FromEntity(entity);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                await SaveErrorAsync(entity, ex.Message);
                throw new QuantityMeasurementException($"Divide failed: {ex.Message}", ex);
            }
        }

        // ── HISTORY ───────────────────────────────────────────────────────
        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetAllHistoryAsync(int userId)
        {
            var list = await _repository.GetAllByUserAsync(userId);
            return QuantityMeasurementDTO.FromEntityList(list);
        }

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByOperationAsync(
            string operationType, int userId)
        {
            var list = await _repository.GetByOperationTypeAsync(operationType.ToUpperInvariant(), userId);
            return QuantityMeasurementDTO.FromEntityList(list);
        }

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByCategoryAsync(
            string category, int userId)
        {
            var list = await _repository.GetByCategoryAsync(category.ToUpperInvariant(), userId);
            return QuantityMeasurementDTO.FromEntityList(list);
        }

        public async Task<IReadOnlyList<QuantityMeasurementDTO>> GetErrorHistoryAsync(int userId)
        {
            var list = await _repository.GetErrorsAsync(userId);
            return QuantityMeasurementDTO.FromEntityList(list);
        }

        public async Task<int> GetOperationCountAsync(string operationType, int userId)
            => await _repository.GetCountByOperationAsync(operationType.ToUpperInvariant(), userId);

        // ── PRIVATE ───────────────────────────────────────────────────────
        private static QuantityMeasurementApiEntity BuildEntity(
            string opType, QuantityDTO q1, QuantityDTO? q2, int? userId) => new()
        {
            UserId              = userId,
            OperationType       = opType,
            MeasurementCategory = q1.Category,
            Operand1Value       = q1.Value,
            Operand1Unit        = q1.UnitName,
            Operand2Value       = q2?.Value,
            Operand2Unit        = q2?.UnitName
        };

        private async Task SaveErrorAsync(QuantityMeasurementApiEntity entity, string msg)
        {
            entity.HasError     = true;
            entity.ErrorMessage = msg;
            try { await _repository.SaveAsync(entity); }
            catch (System.Exception ex) { _logger.LogError(ex, "Failed to save error entity."); }
        }
    }
}
