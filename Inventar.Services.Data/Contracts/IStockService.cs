using Inventar.Web.ViewModels.Stock;
using Inventar.Web.ViewModels;

namespace Inventar.Services.Data.Contracts;

public interface IStockService
{
    Task RecordTransactionAsync(StockTransactionViewModel model, string userId);
    Task<decimal> GetCurrentStockLevelAsync(Guid baseId, Guid materialId);
    Task<IEnumerable<DropdownPairViewModel>> GetMaterialsDropdownAsync();
    Task<IEnumerable<MaterialViewModel>> GetAllMaterialsListAsync();
    Task SaveMaterialAsync(MaterialViewModel model);
    Task DeleteMaterialAsync(Guid id);

    Task<IEnumerable<StockLevelViewModel>> GetStockLevelsAsync(Guid? baseId = null, Guid? materialId = null, string searchTerm = null);
}