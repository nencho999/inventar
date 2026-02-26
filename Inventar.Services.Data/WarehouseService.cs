using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Warehouse;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                .Include(w => w.Capacities)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(w => w.Name.Contains(search) || w.Location.Contains(search));
            }

            var warehouses = await query.ToListAsync();
            var warehouseViewModels = new List<WarehouseListItemViewModel>();

            foreach (var w in warehouses)
            {
                decimal maxCapacity = w.Capacities?.Sum(c => c.CapacityLimit) ?? 0;

                decimal currentlyUsed = 0;

                warehouseViewModels.Add(new WarehouseListItemViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    Status = w.Status,
                    TotalCapacity = maxCapacity,
                    UsedCapacity = currentlyUsed,
                    MonthlyExpensesTotal = w.RecurringExpenses.Sum(re => re.Amount),
                    OneTimeExpensesTotal = w.Expenses.Sum(e => e.Amount)
                });
            }

            var viewModel = new WarehouseIndexViewModel
            {
                Warehouses = warehouseViewModels,
                TotalWarehouses = warehouses.Count,
                TotalMonthlyExpenses = warehouses.Sum(w => w.RecurringExpenses.Sum(re => re.Amount)),
                TotalOneTimeExpenses = warehouses.Sum(w => w.Expenses.Sum(e => e.Amount)),
                OperationalCount = warehouses.Count(w => w.Status == WarehouseStatus.Operational),
                UnderConstructionCount = warehouses.Count(w => w.Status == WarehouseStatus.UnderConstruction),
            };

            return viewModel;
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await _context.Warehouses.ToListAsync();
        }

        public async Task<WarehouseFormViewModel> GetNewWarehouseFormAsync()
        {
            var model = new WarehouseFormViewModel();

            var materials = await _context.Materials.ToListAsync();

            model.AvailableMaterials = materials.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            });

            return model;
        }

        public async Task<IEnumerable<SelectListItem>> GetMaterialsSelectListAsync()
        {
            var materials = await _context.Materials.ToListAsync();
            return materials.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            });
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

            if (model.Capacities != null && model.Capacities.Any())
            {
                foreach (var cap in model.Capacities)
                {
                    if (cap.MaterialId != Guid.Empty && cap.CapacityLimit > 0)
                    {
                        warehouse.Capacities.Add(new Capacity
                        {
                            MaterialId = cap.MaterialId,
                            CapacityLimit = cap.CapacityLimit
                        });
                    }
                }
            }

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task<WarehouseFormViewModel> GetForEditAsync(Guid id)
        {
            var warehouse = await _context.Warehouses
                .Include(w => w.Capacities)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouse == null) return null;

            var model = new WarehouseFormViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                ContactInfo = warehouse.ContactInfo,
                Capacity = warehouse.Capacity,
                Status = warehouse.Status,

                Capacities = warehouse.Capacities.Select(c => new CapacityViewModel
                {
                    MaterialId = c.MaterialId,
                    CapacityLimit = c.CapacityLimit
                }).ToList()
            };

            var materials = await _context.Materials.ToListAsync();
            model.AvailableMaterials = materials.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            });

            return model;
        }

        public async Task UpdateAsync(WarehouseFormViewModel model)
        {
            var warehouse = await _context.Warehouses
        .Include(w => w.Capacities)
        .FirstOrDefaultAsync(w => w.Id == model.Id);

            if (warehouse != null)
            {
                warehouse.Name = model.Name;
                warehouse.Location = model.Location;
                warehouse.ContactInfo = model.ContactInfo;
                warehouse.Capacity = model.Capacity;
                warehouse.Status = model.Status;

                _context.Capacities.RemoveRange(warehouse.Capacities);

                if (model.Capacities != null && model.Capacities.Any())
                {
                    foreach (var cap in model.Capacities)
                    {
                        if (cap.MaterialId != Guid.Empty && cap.CapacityLimit > 0)
                        {
                            warehouse.Capacities.Add(new Capacity
                            {
                                MaterialId = cap.MaterialId,
                                CapacityLimit = cap.CapacityLimit
                            });
                        }
                    }
                }

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
