using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels;
using Inventar.Web.ViewModels.DropDowns;
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
        public async Task<bool> RegisterTransferAsync(ProductionTransferFormModel model)
        {
            var centerStorage = await _context.ProductionCenterStorages
                .FirstOrDefaultAsync(s => s.ProductionCenterId == model.FromProductionCenterId && s.ProductId == model.ProductId);

            if (centerStorage == null || centerStorage.CurrentStock < model.Quantity)
                return false;

            var warehouseStorage = await _context.WarehouseProducts
                .FirstOrDefaultAsync(w => w.WarehouseId == model.ToWarehouseId && w.ProductId == model.ProductId);

            if (warehouseStorage == null)
            {
                warehouseStorage = new WarehouseProduct
                {
                    WarehouseId = model.ToWarehouseId,
                    ProductId = model.ProductId,
                    Quantity = 0
                };
                _context.WarehouseProducts.Add(warehouseStorage);
            }

            centerStorage.CurrentStock -= model.Quantity;
            warehouseStorage.Quantity += model.Quantity;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<ProductionTransferFormModel> GetTransferModelAsync()
        {
            var centers = await _context.ProductionCenters
                .AsNoTracking()
                .Select(c => new ProductionCentersDropDown
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            var warehouses = await _context.Warehouses
                .AsNoTracking()
                .Select(w => new WarehouseDropDownModel
                {
                    Id = w.Id,
                    Name = w.Name
                })
                .ToListAsync();

            return new ProductionTransferFormModel
            {
                ProductionCenters = centers,
                Warehouses = warehouses,
            };
        }
        public async Task<bool> RegisterSalesPointTransferAsync(CenterToSalesPointFormModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var centerStorage = await _context.ProductionCenterStorages
                    .FirstOrDefaultAsync(s => s.ProductionCenterId == model.FromProductionCenterId
                                           && s.ProductId == model.ProductId);

                if (centerStorage == null || centerStorage.CurrentStock < model.Quantity)
                {
                    return false;
                }

                var salesPointStorage = await _context.SalesPointProducts
                    .FirstOrDefaultAsync(s => s.SalesPointId == model.ToSalesPointId
                                           && s.ProductId == model.ProductId);

                if (salesPointStorage == null)
                {
                    salesPointStorage = new SalesPointProduct
                    {
                        SalesPointId = model.ToSalesPointId,
                        ProductId = model.ProductId,
                        Quantity = 0
                    };
                    await _context.SalesPointProducts.AddAsync(salesPointStorage);
                }

                centerStorage.CurrentStock -= model.Quantity;
                salesPointStorage.Quantity += model.Quantity;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<CenterToSalesPointFormModel> GetSalesPointTransferModelAsync()
        {
            var centers = await _context.ProductionCenters
                .AsNoTracking()
                .Select(c => new ProductionCentersDropDown
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            var salesPoints = await _context.SalesPoints
                .AsNoTracking()
                .Select(s => new SalesPointDropDownModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();

            return new CenterToSalesPointFormModel
            {
                ProductionCenters = centers,
                SalesPoints = salesPoints
            };
        }

        public async Task<IEnumerable<SelectListItem>> GetAllSalesPointsAsync()
        {
            return await _context.SalesPoints
                .Select(sp => new SelectListItem { Value = sp.Id.ToString(), Text = sp.Name })
                .ToListAsync();
        }

        public async Task<bool> ExecuteWarehouseToSalesPointTransferAsync(WarehouseToSalesPointViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in model.Products.Where(p => p.TransferQuantity > 0))
                {
                    var warehouseStock = await _context.WarehouseProducts
                        .FirstAsync(wp => wp.WarehouseId == model.FromWarehouseId && wp.ProductId == item.ProductId);

                    if (warehouseStock.Quantity < item.TransferQuantity) return false;
                    warehouseStock.Quantity -= item.TransferQuantity;

                    var salesPointStock = await _context.SalesPointProducts
                        .FirstOrDefaultAsync(sp => sp.SalesPointId == model.ToSalesPointId && sp.ProductId == item.ProductId);

                    if (salesPointStock == null)
                    {
                        await _context.SalesPointProducts.AddAsync(new SalesPointProduct
                        {
                            SalesPointId = model.ToSalesPointId,
                            ProductId = item.ProductId,
                            Quantity = item.TransferQuantity
                        });
                    }
                    else
                    {
                        salesPointStock.Quantity += item.TransferQuantity;
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
