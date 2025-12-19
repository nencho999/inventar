using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels;
using Inventar.Web.ViewModels.Stock;
using Microsoft.EntityFrameworkCore;
using static Inventar.Common.Messages.ErrorMessages.Stock;

namespace Inventar.Services.Data
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;

        public StockService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RecordTransactionAsync(StockTransactionViewModel model, string userId)
        {
            decimal currentStock = await GetCurrentStockLevelAsync(model.BaseId, model.MaterialId);

            if (!model.IsAcquisition)
            {
                if (currentStock < model.Quantity)
                {
                    throw new InvalidOperationException(string.Format(InsufficientStock, currentStock, model.Quantity));
                }
            }
            else
            {
                var capacityRecord = await _context.Capacities
                    .FirstOrDefaultAsync(bc => bc.PrimaryMaterialBaseId == model.BaseId && bc.MaterialId == model.MaterialId);

                decimal maxLimit = capacityRecord?.CapacityLimit ?? 0;

                if (maxLimit == 0)
                {
                    throw new InvalidOperationException(NoCapacityDefined);
                }

                decimal futureStock = currentStock + model.Quantity;

                if (futureStock > maxLimit)
                {
                    decimal availableSpace = maxLimit - currentStock;
                    throw new InvalidOperationException(string.Format(CapacityExceeded, maxLimit, currentStock, availableSpace));
                }
            }

            decimal change = model.IsAcquisition ? model.Quantity : -model.Quantity;

            var transaction = new StockTransaction
            {
                BaseId = model.BaseId,
                MaterialId = model.MaterialId,
                QuantityChange = change,
                TransactionDate = model.TransactionDate,
                Notes = model.Notes,
                CreatedByUserId = userId,
                UnitPrice = model.IsAcquisition ? model.UnitPrice : null
            };

            await _context.StockTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCurrentStockLevelAsync(Guid baseId, Guid materialId)
        {
            var total = await _context.StockTransactions
                .Where(t => t.BaseId == baseId && t.MaterialId == materialId)
                .SumAsync(t => t.QuantityChange);

            return total;
        }

        public async Task<IEnumerable<DropdownPairViewModel>> GetMaterialsDropdownAsync()
        {
            return await _context.Materials
                .Select(m => new DropdownPairViewModel
                {
                    Id = m.Id,
                    Name = $"{m.Name} ({m.Unit})"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialViewModel>> GetAllMaterialsListAsync()
        {
            return await _context.Materials
                .Select(m => new MaterialViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Unit = m.Unit
                })
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task SaveMaterialAsync(MaterialViewModel model)
        {
            Material entity;

            if (model.Id == Guid.Empty)
            {
                entity = new Material();
                await _context.Materials.AddAsync(entity);
            }
            else
            {
                entity = await _context.Materials.FindAsync(model.Id);
                if (entity == null) return;
            }

            entity.Name = model.Name;
            entity.Unit = model.Unit;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMaterialAsync(Guid id)
        {
            var entity = await _context.Materials.FindAsync(id);
            if (entity != null)
            {
                _context.Materials.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StockLevelViewModel>> GetStockLevelsAsync(Guid? baseId = null, Guid? materialId = null, string searchTerm = null)
        {
            var query = _context.StockTransactions
                .Include(t => t.Base)
                .Include(t => t.Material)
                .AsQueryable();

            if (baseId.HasValue)
            {
                query = query.Where(t => t.BaseId == baseId.Value);
            }

            if (materialId.HasValue)
            {
                query = query.Where(t => t.MaterialId == materialId.Value);
            }

            var groupedQuery = await query
                .GroupBy(t => new { t.Base.Name, MaterialName = t.Material.Name, t.Material.Unit })
                .Select(g => new
                {
                    BaseName = g.Key.Name,
                    MaterialName = g.Key.MaterialName,
                    Unit = g.Key.Unit,
                    CurrentQuantity = g.Sum(t => t.QuantityChange),
                    TotalCostIn = g.Where(t => t.QuantityChange > 0 && t.UnitPrice != null)
                        .Sum(t => t.QuantityChange * t.UnitPrice.Value),
                    TotalQtyIn = g.Where(t => t.QuantityChange > 0 && t.UnitPrice != null)
                        .Sum(t => t.QuantityChange)
                })
                .ToListAsync();

            var resultList = groupedQuery
                .Where(x => x.CurrentQuantity != 0)
                .Select(x => new StockLevelViewModel()
                {
                    BaseName = x.BaseName,
                    MaterialName = x.MaterialName,
                    Unit = x.Unit,
                    Quantity = x.CurrentQuantity,
                    AveragePrice = x.TotalQtyIn > 0 ? (x.TotalCostIn / x.TotalQtyIn) : 0,
                    TotalValue = x.CurrentQuantity * (x.TotalQtyIn > 0 ? (x.TotalCostIn / x.TotalQtyIn) : 0)
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                resultList = resultList
                    .Where(x => x.BaseName.ToLower().Contains(searchTerm) ||
                                x.MaterialName.ToLower().Contains(searchTerm))
                    .ToList();
            }

            return resultList.OrderBy(x => x.BaseName).ThenBy(x => x.MaterialName);
        }
        public async Task<IEnumerable<StockHistoryViewModel>> GetLastTransactionsAsync(int count = 100)
        {
            return await _context.StockTransactions
                .Include(t => t.Base)
                .Include(t => t.Material)
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .Select(t => new StockHistoryViewModel
                {
                    Id = t.Id,
                    Date = t.TransactionDate,
                    BaseName = t.Base.Name,
                    MaterialName = t.Material.Name,
                    Unit = t.Material.Unit,
                    IsAcquisition = t.QuantityChange > 0,
                    Quantity = Math.Abs(t.QuantityChange),
                    UnitPrice = t.UnitPrice
                })
                .ToListAsync();
        }
    }
}