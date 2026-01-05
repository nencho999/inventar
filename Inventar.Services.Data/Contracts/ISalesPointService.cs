using Inventar.Web.ViewModels.SalesPoint;

namespace Inventar.Services.Data.Contracts;

public interface ISalesPointService
{
    Task<SalesPointIndexViewModel> GetIndexDataAsync(string searchTerm);

    Task<SalesPointFormViewModel> GetFormForCreateAsync();

    Task<SalesPointFormViewModel> GetFormForEditAsync(Guid id);

    Task CreateAsync(SalesPointFormViewModel model);

    Task UpdateAsync(SalesPointFormViewModel model);
    Task<SalesPointAddExpenseViewModel> GetExpenseFormAsync(Guid salesPointId);
    Task AddExpenseAsync(SalesPointAddExpenseViewModel model);

    Task<SalesPointDeleteViewModel> GetDeleteDetailsAsync(Guid id);
    Task DeleteAsync(Guid id);
}