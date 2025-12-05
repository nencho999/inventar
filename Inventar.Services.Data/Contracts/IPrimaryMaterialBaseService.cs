using Inventar.Data.Models;
using Inventar.Models;

namespace Inventar.Services.Data.Contracts;

public interface IPrimaryMaterialBaseService
{
    Task<IEnumerable<PrimaryMaterialBase>> GetAllBasesAsync<T>();
    Task<PrimaryMaterialBase?> GetBaseByIdAsync(Guid id);
    Task AddBaseAsync(PrimaryMaterialBase newBase);
    Task UpdateBaseAsync(PrimaryMaterialBase baseToUpdate);
    Task DeleteBaseAsync(Guid id);
}