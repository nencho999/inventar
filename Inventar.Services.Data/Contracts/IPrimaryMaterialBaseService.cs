using Inventar.Data.Models;
using Inventar.Models;
using Inventar.Web.ViewModels.BaseViewModels;

namespace Inventar.Services.Data.Contracts;

public interface IPrimaryMaterialBaseService
{
    Task<IEnumerable<BaseListViewModel>> GetAllBasesAsync();
    Task<BaseEditViewModel?> GetBaseForEditAsync(Guid id);
    Task SaveBaseAsync(BaseEditViewModel model);
    Task DeleteBaseAsync(Guid id);
}