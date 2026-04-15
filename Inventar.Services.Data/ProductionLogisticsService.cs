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
                        .FirstOrDefaultAsync(wp => wp.WarehouseId == model.FromWarehouseId && wp.ProductId == item.ProductId);

                    if (warehouseStock.Quantity < item.TransferQuantity) return false;
                    warehouseStock.Quantity -= item.TransferQuantity;

                    var salesPointStock = await _context.SalesPointProducts
                        .FirstOrDefaultAsync(sp => sp.SalesPointId == model.ToSalesPointId && sp.ProductId == item.ProductId);

                    if (salesPointStock == null)
                    {
                        var newStock = new SalesPointProduct
                        {
                            SalesPointId = model.ToSalesPointId,
                            ProductId = item.ProductId,
                            Quantity = (int?)item.TransferQuantity,
                            IsSelected = true,
                            PriceReductionValue = 0
                        };
                        await _context.SalesPointProducts.AddAsync(newStock);
                    }
                    else
                    {
                        salesPointStock.Quantity = (salesPointStock.Quantity ?? 0) + (int?)item.TransferQuantity;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<SalesProductItemViewModel>> GetSalesPointProductsAsync(Guid salesPointId)
        {
            return await _context.SalesPointProducts
                .Where(sp => sp.SalesPointId == salesPointId && sp.Quantity > 0)
                .Include(sp => sp.Product)
                .Select(sp => new SalesProductItemViewModel
                {
                    ProductId = sp.ProductId,
                    ProductName = sp.Product.Name,
                    AvailableInStock = sp.Quantity ?? 0,
                    UnitPrice = sp.PriceReductionValue > 0 ? sp.PriceReductionValue : sp.Product.Price
                }).ToListAsync();
        }

        public async Task<bool> RegisterSalesActivityAsync(SalesActivityViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in model.Products.Where(p => p.SoldQuantity > 0))
                {
                    var stock = await _context.SalesPointProducts
                        .FirstAsync(sp => sp.SalesPointId == model.SalesPointId && sp.ProductId == item.ProductId);

                    if (stock.Quantity < item.SoldQuantity) return false;
                    stock.Quantity -= item.SoldQuantity;

                    var sale = new SaleRecord
                    {
                        SalesPointId = model.SalesPointId,
                        ProductId = item.ProductId,
                        Quantity = item.SoldQuantity,
                        UnitPrice = item.UnitPrice,
                        IsVatApplied = model.IsVatApplicable,
                        SaleDate = DateTime.Now,
                        TotalAmount = (decimal)item.SoldQuantity * item.UnitPrice
                    };

                    await _context.SaleRecords.AddAsync(sale);
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

        public async Task<IEnumerable<SelectListItem>> GetAllProductionCentersAsync()
        {
            return await _context.ProductionCenters
                .Select(pc => new SelectListItem { Value = pc.Id.ToString(), Text = pc.Name })
                .ToListAsync();
        }

        public async Task<bool> RegisterSalesPointReturnAsync(SalesPointReturnViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productsToReturn = model.Products.Where(p => p.ReturnQuantity > 0).ToList();

                foreach (var item in productsToReturn)
                {
                    var salesPointStock = await _context.SalesPointProducts
                        .FirstOrDefaultAsync(sp => sp.SalesPointId == model.FromSalesPointId && sp.ProductId == item.ProductId);

                    if (salesPointStock == null || (salesPointStock.Quantity ?? 0) < item.ReturnQuantity)
                        return false;

                    salesPointStock.Quantity -= item.ReturnQuantity;

                    if (model.DestinationType == "Warehouse")
                    {
                        var warehouseStock = await _context.WarehouseProducts
                            .FirstOrDefaultAsync(w => w.WarehouseId == model.ToDestinationId && w.ProductId == item.ProductId);

                        if (warehouseStock == null)
                        {
                            await _context.WarehouseProducts.AddAsync(new WarehouseProduct
                            {
                                WarehouseId = model.ToDestinationId,
                                ProductId = item.ProductId,
                                Quantity = item.ReturnQuantity
                            });
                        }
                        else
                        {
                            warehouseStock.Quantity += item.ReturnQuantity;
                        }
                    }
                    else if (model.DestinationType == "ProductionCenter")
                    {
                        var centerStock = await _context.ProductionCenterStorages
                            .FirstOrDefaultAsync(c => c.ProductionCenterId == model.ToDestinationId && c.ProductId == item.ProductId);

                        if (centerStock == null)
                        {
                            await _context.ProductionCenterStorages.AddAsync(new ProductionCenterStorage
                            {
                                Id = Guid.NewGuid(),
                                ProductionCenterId = model.ToDestinationId,
                                ProductId = item.ProductId,
                                CurrentStock = item.ReturnQuantity,
                                MaxStorageCapacity = 1000
                            });
                        }
                        else
                        {
                            centerStock.CurrentStock += item.ReturnQuantity;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
