using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Interface
{
    public interface IQuantityMeasurementApiRepository
    {
        Task SaveAsync(QuantityMeasurementApiEntity entity);

        // Per-user queries (pass userId to filter only that user's records)
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetAllByUserAsync(int userId);
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByOperationTypeAsync(string operationType, int userId);
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByCategoryAsync(string category, int userId);
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetErrorsAsync(int userId);
        Task<int> GetCountByOperationAsync(string operationType, int userId);
        Task<int> GetTotalCountAsync(int userId);
    }

    public interface IUserRepository
    {
        Task<UserEntity?>  GetByEmailAsync(string email);
        Task<UserEntity?>  GetByIdAsync(int id);
        Task<bool>         ExistsByEmailAsync(string email);
        Task<bool>         ExistsByUsernameAsync(string username);
        Task<UserEntity>   CreateAsync(UserEntity user);
        Task               UpdateLastLoginAsync(int userId);
    }
}
