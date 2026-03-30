using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Interface
{
    /// <summary>
    /// UC17: Async EF Core repository interface for Web API persistence.
    /// The existing sync IQuantityMeasurementRepository is preserved unchanged.
    /// </summary>
    public interface IQuantityMeasurementApiRepository
    {
        Task SaveAsync(QuantityMeasurementApiEntity entity);
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetAllAsync();
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByOperationTypeAsync(string operationType);
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetByCategoryAsync(string category);
        Task<IReadOnlyList<QuantityMeasurementApiEntity>> GetErrorsAsync();
        Task<int> GetCountByOperationAsync(string operationType);
        Task<int> GetTotalCountAsync();
    }

    /// <summary>UC17: User persistence repository interface.</summary>
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
