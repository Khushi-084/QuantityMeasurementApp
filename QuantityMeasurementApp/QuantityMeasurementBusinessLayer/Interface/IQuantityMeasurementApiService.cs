using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Interface
{
    public interface IQuantityMeasurementApiService
    {
        // Public operations — userId is null when called anonymously (no login)
        Task<QuantityMeasurementDTO> CompareAsync(QuantityInputDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> ConvertAsync(ConvertRequestDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> AddAsync(QuantityInputDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> SubtractAsync(QuantityInputDTO input, int? userId = null);
        Task<QuantityMeasurementDTO> DivideAsync(QuantityInputDTO input, int? userId = null);

        // History — always requires userId (caller must be authenticated)
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetAllHistoryAsync(int userId);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByOperationAsync(string operationType, int userId);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByCategoryAsync(string category, int userId);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetErrorHistoryAsync(int userId);
        Task<int> GetOperationCountAsync(string operationType, int userId);
    }

    public interface IUserService
    {
        Task<AuthResponseDTO> SignupAsync(SignupRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<UserResponseDTO> GetProfileAsync(int userId);
    }

    public interface IJwtService
    {
        string GenerateToken(int userId, string email, string username, string role);
        (bool isValid, int userId, string email, string role) ValidateToken(string token);
    }

    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
