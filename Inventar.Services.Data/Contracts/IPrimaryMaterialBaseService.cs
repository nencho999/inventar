using Inventar.Data.Models;
using Inventar.Models;
using Inventar.Web.ViewModels;
using Inventar.Web.ViewModels.Base;
using Inventar.Web.ViewModels.PrimaryBase;
using Inventar.Web.ViewModels.ProductionCenter;

namespace Inventar.Services.Data.Contracts;

public interface IPrimaryMaterialBaseService
{
    Task<BaseFormViewModel> GetBaseForEditAsync(Guid id);
    Task<DashboardViewModel> GetDashboardAsync();
    Task SaveBaseAsync(BaseFormViewModel model);
    Task DeleteBaseAsync(Guid id);
    Task<IEnumerable<MaterialDropDownModel>> GetMaterialsDropdownAsync();
    Task<IEnumerable<DropdownPairViewModel>> GetBasesDropdownAsync();
    Task<List<PrimaryBaseExpenseRowViewModel>> GetBaseExpensesByTypeAsync(string type);
    Task<PrimaryBaseAddExpenseViewModel> GetExpenseFormAsync(Guid baseId);
    Task AddExpenseAsync(PrimaryBaseAddExpenseViewModel model, string userId);
}