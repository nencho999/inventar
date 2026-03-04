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
                .Include(w => w.WarehouseProducts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(w => w.Name.Contains(search) || w.Location.Contains(search));
            }

            var warehouses = await query.ToListAsync();
            var warehouseViewModels = new List<WarehouseListItemViewModel>();

            foreach (var w in warehouses)
            {
                decimal.TryParse(w.Capacity, out decimal maxCapacity);
                decimal currentlyUsed = (decimal)(w.WarehouseProducts?.Sum(wp => wp.Quantity) ?? 0);

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
            var allProducts = await _context.Products.ToListAsync();

            return new WarehouseFormViewModel
            {
                Products = allProducts.Select(p => new WarehouseProductSelectionViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    IsSelected = false,
                    Quantity = 0
                }).ToList()
            };
        }

        public async Task<IEnumerable<SelectListItem>> GetProductsSelectListAsync()
        {
            return await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToListAsync();
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

            if (model.Products != null && model.Products.Any())
            {
                foreach (var productRow in model.Products)
                {
                    if (productRow.IsSelected)
                    {
                        warehouse.WarehouseProducts.Add(new WarehouseProduct
                        {
                            ProductId = productRow.ProductId,
                            Quantity = productRow.Quantity
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
                .Include(w => w.WarehouseProducts)
                .ThenInclude(wp => wp.Product)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouse == null) return null;

            var allProducts = await _context.Products.ToListAsync();

            var model = new WarehouseFormViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                ContactInfo = warehouse.ContactInfo,
                Capacity = warehouse.Capacity,
                Status = warehouse.Status,
                Products = allProducts.Select(p => new WarehouseProductSelectionViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    IsSelected = warehouse.WarehouseProducts.Any(wp => wp.ProductId == p.Id),
                    Quantity = warehouse.WarehouseProducts
                        .FirstOrDefault(wp => wp.ProductId == p.Id)?.Quantity ?? 0
                }).ToList()
            };

            return model;
        }

        public async Task UpdateAsync(WarehouseFormViewModel model)
        {
            var warehouse = await _context.Warehouses
                .Include(w => w.WarehouseProducts)
                .FirstOrDefaultAsync(w => w.Id == model.Id);

            if (warehouse != null)
            {
                warehouse.Name = model.Name;
                warehouse.Location = model.Location;
                warehouse.ContactInfo = model.ContactInfo;
                warehouse.Capacity = model.Capacity;
                warehouse.Status = model.Status;

                _context.WarehouseProducts.RemoveRange(warehouse.WarehouseProducts);

                if (model.Products != null)
                {
                    foreach (var p in model.Products.Where(x => x.IsSelected))
                    {
                        warehouse.WarehouseProducts.Add(new WarehouseProduct
                        {
                            ProductId = p.ProductId,
                            Quantity = p.Quantity
                        });
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
