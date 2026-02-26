using Inventar.Data.Models;
using Inventar.Data;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.SalesPoint;
using Microsoft.EntityFrameworkCore;

namespace Inventar.Services.Data
{
    public class SalesPointService : ISalesPointService
    {
        private readonly ApplicationDbContext _context;

        public SalesPointService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SalesPointIndexViewModel> GetIndexDataAsync(string searchTerm)
        {
            var query = _context.SalesPoints
                .Include(sp => sp.Expenses)
                .Include(sp => sp.SalesPointProducts)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();

                query = query.Where(sp =>
                    sp.Name.ToLower().Contains(term) ||
                    sp.Address.ToLower().Contains(term)
                );
            }

            var entities = await query.ToListAsync();

            decimal globalMonthly = 0;
            decimal globalOneTime = 0;

            var dtos = new List<SalesPointListDTO>();

            foreach (var sp in entities)
            {
                decimal spMonthly = 0;
                decimal spOneTime = 0;

                foreach (var exp in sp.Expenses)
                {
                    if (exp.IsRecurring)
                    {
                        spMonthly += CalculateMonthlyAmount(exp);
                    }
                    else
                    {
                        spOneTime += exp.Amount;
                    }
                }

                globalMonthly += spMonthly;
                globalOneTime += spOneTime;

                dtos.Add(new SalesPointListDTO
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Address = sp.Address,
                    Type = sp.Type.ToString(),
                    Status = sp.Status,
                    ProductCount = sp.SalesPointProducts.Count(p => p.IsSelected),
                    MonthlyExpenses = spMonthly,
                    OneTimeExpenses = spOneTime
                });
            }

            return new SalesPointIndexViewModel
            {
                SearchTerm = searchTerm,
                SalesPoints = dtos,
                TotalCount = entities.Count,
                PhysicalCount = entities.Count(x => x.Type == SalesPointType.Physical),
                OnlineCount = entities.Count(x => x.Type == SalesPointType.Online),
                OperationalCount = entities.Count(x => x.Status == WarehouseStatus.Operational),
                UnderConstructionCount = entities.Count(x => x.Status == WarehouseStatus.UnderConstruction),
                ClosedCount = entities.Count(x => x.Status == WarehouseStatus.Closed),
                TotalMonthlyExpenses = globalMonthly,
                TotalOneTimeExpenses = globalOneTime
            };
        }

        public async Task<SalesPointFormViewModel> GetFormForCreateAsync()
        {
            var products = await _context.Products
                .Select(p => new SalesPointProductViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    IsSelected = true,
                    PriceReduction = 0,
                    IsPercentage = true,
                    OriginalPrice = p.Price
                }).ToListAsync();

            return new SalesPointFormViewModel
            {
                Products = products,
                Type = SalesPointType.Physical
            };
        }

        public async Task CreateAsync(SalesPointFormViewModel model)
        {
            var entity = new SalesPoint
            {
                Name = model.Name,
                Address = model.Address,
                Description = model.Description,
                Type = model.Type,
                Status = model.Status
            };

            foreach (var p in model.Products.Where(x => x.IsSelected))
            {
                entity.SalesPointProducts.Add(new SalesPointProduct
                {
                    ProductId = p.ProductId,
                    IsSelected = true,
                    PriceReductionValue = p.PriceReduction,
                    IsPercentage = p.IsPercentage
                });
            }

            _context.SalesPoints.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<SalesPointFormViewModel> GetFormForEditAsync(Guid id)
        {
            var entity = await _context.SalesPoints
                .Include(sp => sp.SalesPointProducts)
                .Include(sp => sp.Expenses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            var allMaterials = await _context.Products.ToListAsync();
            var productVms = new List<SalesPointProductViewModel>();

            foreach (var mat in allMaterials)
            {
                var existingLink = entity.SalesPointProducts.FirstOrDefault(x => x.ProductId == mat.Id);

                productVms.Add(new SalesPointProductViewModel
                {
                    ProductId = mat.Id,
                    ProductName = mat.Name,
                    IsSelected = existingLink != null,
                    PriceReduction = existingLink?.PriceReductionValue ?? 0,
                    IsPercentage = existingLink?.IsPercentage ?? true,
                    OriginalPrice = mat.Price
                });
            }

            return new SalesPointFormViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address,
                Description = entity.Description,
                Type = entity.Type,
                Products = productVms,
                Status = entity.Status
            };
        }

        public async Task UpdateAsync(SalesPointFormViewModel model)
        {
            var entity = await _context.SalesPoints
                .Include(sp => sp.SalesPointProducts)
                .Include(sp => sp.Expenses)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null) return;

            entity.Name = model.Name;
            entity.Address = model.Address;
            entity.Description = model.Description;
            entity.Type = model.Type;
            entity.Status = model.Status;

            entity.SalesPointProducts.Clear();
            foreach (var p in model.Products.Where(x => x.IsSelected))
            {
                entity.SalesPointProducts.Add(new SalesPointProduct
                {
                    SalesPointId = entity.Id,
                    ProductId = p.ProductId,
                    IsSelected = true,
                    PriceReductionValue = p.PriceReduction,
                    IsPercentage = p.IsPercentage
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<SalesPointAddExpenseViewModel> GetExpenseFormAsync(Guid salesPointId)
        {
            var sp = await _context.SalesPoints.FindAsync(salesPointId);
            if (sp == null) return null;

            return new SalesPointAddExpenseViewModel
            {
                SalesPointId = sp.Id,
                SalesPointName = sp.Name,
                OneTimeDate = DateTime.Today
            };
        }

        public async Task AddExpenseAsync(SalesPointAddExpenseViewModel model)
        {
            var expense = new SalesPointExpense
            {
                SalesPointId = model.SalesPointId,
                Name = model.Name,
                Amount = model.Amount,
                IsRecurring = model.IsRecurring,
                Frequency = model.IsRecurring ? model.Frequency : null,
                CustomIntervalCount = model.CustomIntervalCount,
                OneTimeDate = !model.IsRecurring ? model.OneTimeDate : null
            };

            _context.SalesPointExpenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task<SalesPointDeleteViewModel> GetDeleteDetailsAsync(Guid id)
        {
            var sp = await _context.SalesPoints
                .Include(x => x.SalesPointProducts)
                .Include(x => x.Expenses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sp == null) return null;

            return new SalesPointDeleteViewModel
            {
                Id = sp.Id,
                Name = sp.Name,
                Address = sp.Address,
                Type = sp.Type.ToString(),
                LinkedProductsCount = sp.SalesPointProducts.Count(p => p.IsSelected),
                LinkedExpensesCount = sp.Expenses.Count
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.SalesPoints.FindAsync(id);
            if (entity != null)
            {
                _context.SalesPoints.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        private decimal CalculateMonthlyAmount(SalesPointExpense exp)
        {
            if (!exp.IsRecurring) return 0;

            switch (exp.Frequency)
            {
                case ExpenseFrequency.Weekly:
                    return exp.Amount * 4.33m;
                case ExpenseFrequency.Monthly:
                    return exp.Amount;
                case ExpenseFrequency.Yearly:
                    return exp.Amount / 12m;
                case ExpenseFrequency.CustomMonthInterval:
                    if (exp.CustomIntervalCount.HasValue && exp.CustomIntervalCount > 0)
                        return exp.Amount / exp.CustomIntervalCount.Value;
                    return exp.Amount;
                default:
                    return 0;
            }
        }

        public async Task<List<ExpenseListViewModel>> GetExpensesByTypeAsync(string type)
        {
            var query = _context.SalesPointExpenses
                .Include(e => e.SalesPoint)
                .AsQueryable();

            if (type == "Monthly") query = query.Where(e => e.IsRecurring);
            else if (type == "OneTime") query = query.Where(e => !e.IsRecurring);

            var list = await query.OrderByDescending(e => e.Amount).ToListAsync();

            return list.Select(e => new ExpenseListViewModel
            {
                SalesPointId = e.SalesPointId,
                SalesPointName = e.SalesPoint.Name,
                ExpenseName = e.Name,
                Amount = e.Amount,
                DateOrFrequency = e.IsRecurring
                    ? (e.Frequency == ExpenseFrequency.CustomMonthInterval ? $"Every {e.CustomIntervalCount} mo." : e.Frequency.ToString())
                    : (e.OneTimeDate.HasValue ? e.OneTimeDate.Value.ToString("dd.MM.yyyy") : "-")
            }).ToList();
        }
    }
}