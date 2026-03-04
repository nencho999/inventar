using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Logistics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Services.Data
{
    public class ProductionLogisticsService : IProductionLogisticsService
    {
        private readonly ApplicationDbContext _context;

        public ProductionLogisticsService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<SelectListItem>> GetAllWarehousesAsync()
        {
            return await _context.Warehouses
                .Select(w => new SelectListItem { Value = w.Id.ToString(), Text = w.Name })
                .ToListAsync();
        }

        public async Task<List<TransferProductItemViewModel>> GetWarehouseProductStocksAsync(Guid warehouseId)
        {
            return await _context.WarehouseProducts
                .Where(wp => wp.WarehouseId == warehouseId && wp.Quantity > 0)
                .Select(wp => new TransferProductItemViewModel
                {
                    ProductId = wp.ProductId,
                    ProductName = wp.Product.Name,
                    AvailableQuantity = wp.Quantity
                }).ToListAsync();
        }

        public async Task<bool> ExecuteWarehouseTransferAsync(WarehouseTransferViewModel model)
        {
            if (model.FromWarehouseId == model.ToWarehouseId) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in model.Products.Where(p => p.TransferQuantity > 0))
                {
                    var source = await _context.WarehouseProducts
                        .FirstAsync(wp => wp.WarehouseId == model.FromWarehouseId && wp.ProductId == item.ProductId);
                    source.Quantity -= item.TransferQuantity;

                    var dest = await _context.WarehouseProducts
                        .FirstOrDefaultAsync(wp => wp.WarehouseId == model.ToWarehouseId && wp.ProductId == item.ProductId);

                    if (dest == null)
                    {
                        await _context.WarehouseProducts.AddAsync(new WarehouseProduct
                        {
                            WarehouseId = model.ToWarehouseId,
                            ProductId = item.ProductId,
                            Quantity = item.TransferQuantity
                        });
                    }
                    else
                    {
                        dest.Quantity += item.TransferQuantity;
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
