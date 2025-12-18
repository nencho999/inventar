using Inventar.Web.ViewModels.Expense;

namespace Inventar.Services.Data.Contracts;

public interface IExpenseService
{
    Task CreateExpenseAsync(ExpenseFormViewModel model, string userId, bool isAdmin);
    Task<IEnumerable<ExpenseViewModel>> GetAllExpensesAsync();
}