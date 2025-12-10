using Inventar.Data.Models;
using Inventar.Models;
using Inventar.Web.ViewModels;
using Inventar.Web.ViewModels.Base;

namespace Inventar.Services.Data.Contracts;

public interface IPrimaryMaterialBaseService
{
    Task<BaseFormViewModel> GetBaseForEditAsync(Guid id);
    Task<DashboardViewModel> GetDashboardAsync();
    Task SaveBaseAsync(BaseFormViewModel model);
    Task DeleteBaseAsync(Guid id);

    Task<IEnumerable<DropdownPairViewModel>> GetBasesDropdownAsync();
}