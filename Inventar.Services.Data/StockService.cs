using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels;
using Inventar.Web.ViewModels.Stock;
using Microsoft.EntityFrameworkCore;

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
            decimal change = model.IsAcquisition ? model.Quantity : -model.Quantity;

            var transaction = new StockTransaction
            {
                BaseId = model.BaseId,
                MaterialId = model.MaterialId,
                QuantityChange = change,
                TransactionDate = model.TransactionDate,
                Notes = model.Notes,
                CreatedByUserId = userId
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
    }
}