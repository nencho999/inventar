using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Expense;

namespace Inventar.Services.Data
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;

        public ExpenseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateExpenseAsync(ExpenseFormViewModel model, string userId, bool isAdmin)
        {
            if (model.IsRecurring)
            {
                var recurring = new RecurringExpense
                {
                    BaseId = model.BaseId,
                    Name = model.Name,
                    Amount = model.Amount,
                    Frequency = model.Frequency,
                    IntervalMonths = model.IntervalMonths,
                    StartDate = model.StartDate,
                    IsActive = true,
                    NextDueDate = model.StartDate
                };

                await _context.RecurringExpenses.AddAsync(recurring);
            }
            else
            {
                var expense = new Expense
                {
                    BaseId = model.BaseId,
                    Name = model.Name,
                    Amount = model.Amount,
                    ExpenseDate = model.ExpenseDate,
                    CreatedByUserId = userId,
                    IsCreatedByAdmin = isAdmin,
                    Description = model.Description
                };

                await _context.Expenses.AddAsync(expense);
            }

            await _context.SaveChangesAsync();
        }
    }
}