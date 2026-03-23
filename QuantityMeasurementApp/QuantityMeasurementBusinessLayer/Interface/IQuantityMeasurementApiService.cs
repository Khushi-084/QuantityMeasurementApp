using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// UC17: Async service interface for the Web API.
    /// Preserves the existing sync IQuantityMeasurementService unchanged.
    /// All methods return Task and use QuantityMeasurementDTO for structured responses.
    /// </summary>
    public interface IQuantityMeasurementApiService
    {
        Task<QuantityMeasurementDTO> CompareAsync(QuantityInputDTO input);
        Task<QuantityMeasurementDTO> ConvertAsync(ConvertRequestDTO input);
        Task<QuantityMeasurementDTO> AddAsync(QuantityInputDTO input);
        Task<QuantityMeasurementDTO> SubtractAsync(QuantityInputDTO input);
        Task<QuantityMeasurementDTO> DivideAsync(QuantityInputDTO input);

        Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByOperationAsync(string operationType);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetHistoryByCategoryAsync(string category);
        Task<IReadOnlyList<QuantityMeasurementDTO>> GetErrorHistoryAsync();
        Task<int> GetOperationCountAsync(string operationType);
    }

    /// <summary>UC17: User authentication service interface.</summary>
    public interface IUserService
    {
        Task<AuthResponseDTO> SignupAsync(SignupRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<UserResponseDTO> GetProfileAsync(int userId);
    }

    /// <summary>UC17: JWT token generation and validation interface.</summary>
    public interface IJwtService
    {
        string GenerateToken(int userId, string email, string username, string role);
        (bool isValid, int userId, string email, string role) ValidateToken(string token);
    }

    /// <summary>UC17: AES encryption/decryption service interface.</summary>
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
