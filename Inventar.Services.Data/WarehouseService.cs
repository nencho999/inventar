using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Warehouse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Services.Data
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _context;

        public WarehouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Warehouse> GetByIdAsync(Guid id)
        {
            return await _context.Warehouses.FindAsync(id);
        }

        public async Task<WarehouseIndexViewModel> GetDashboardDataAsync(string search = null)
        {
            var query = _context.Warehouses
                .Include(w => w.Expenses)
                .Include(w => w.RecurringExpenses)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(w => w.Name.Contains(search) || w.Location.Contains(search));
            }

            var warehouses = await query.ToListAsync();

            var viewModel = new WarehouseIndexViewModel
            {
                Warehouses = warehouses,
                TotalWarehouses = warehouses.Count,

                TotalMonthlyExpenses = warehouses.Sum(w => w.RecurringExpenses.Sum(re => re.Amount)),

                TotalOneTimeExpenses = warehouses.Sum(w => w.Expenses.Sum(e => e.Amount))
            };

            return viewModel;
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await _context.Warehouses.ToListAsync();
        }

        public async Task CreateAsync(WarehouseFormViewModel model)
        {
            var warehouse = new Warehouse
            {
                Name = model.Name,
                Location = model.Location,
                ContactInfo = model.ContactInfo,
                Capacity = model.Capacity,
                Status = model.Status
            };
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task<WarehouseFormViewModel> GetForEditAsync(Guid id)
        {
            var w = await _context.Warehouses.FindAsync(id);
            if (w == null) return null;

            return new WarehouseFormViewModel
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location,
                ContactInfo = w.ContactInfo,
                Capacity = w.Capacity,
                Status = w.Status
            };
        }

        public async Task UpdateAsync(WarehouseFormViewModel model)
        {
            var w = await _context.Warehouses.FindAsync(model.Id);
            if (w != null)
            {
                w.Name = model.Name;
                w.Location = model.Location;
                w.ContactInfo = model.ContactInfo;
                w.Capacity = model.Capacity;
                w.Status = model.Status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var w = await _context.Warehouses.FindAsync(id);
            if (w != null)
            {
                _context.Warehouses.Remove(w);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<WarehouseAddExpenseViewModel> GetExpenseFormAsync(Guid warehouseId)
        {
            var w = await _context.Warehouses.FindAsync(warehouseId);
            if (w == null) return null;

            return new WarehouseAddExpenseViewModel
            {
                WarehouseId = w.Id,
                WarehouseName = w.Name,
                ExpenseDate = DateTime.Today
            };
        }

        public async Task AddExpenseAsync(WarehouseAddExpenseViewModel model, string userId)
        {
            if (model.IsRecurring)
            {
                var recurring = new RecurringExpense
                {
                    WarehouseId = model.WarehouseId,
                    Name = model.Name,
                    Amount = model.Amount,
                    Frequency = model.Frequency,
                    IntervalMonths = model.Frequency == ExpenseFrequency.CustomMonthInterval ? model.IntervalMonths : (int?)null,
                    StartDate = DateTime.Now,
                    IsActive = true
                };
                _context.RecurringExpenses.Add(recurring);
            }
            else
            {
                var expense = new Expense
                {
                    WarehouseId = model.WarehouseId,
                    Name = model.Name,
                    Amount = model.Amount,
                    ExpenseDate = model.ExpenseDate,
                    Description = model.Description,
                    CreatedByUserId = userId,
                    IsCreatedByAdmin = true
                };
                _context.Expenses.Add(expense);
            }

            await _context.SaveChangesAsync();
        }
    }
}
