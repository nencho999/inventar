using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Product;
using Microsoft.EntityFrameworkCore;

namespace Inventar.Services.Data
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext dbContext;

        public ProductService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductIndexViewModel>> GetAllProductsAsync()
        {
            return await dbContext.Products
                .Select(p => new ProductIndexViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ProductType = p.ProductType,
                    Package = p.Package,
                    Price = p.Price,
                    Vat = p.Vat,
                    Gain = p.Gain,
                    CentersCount = p.ProductionCenters.Count()
                })
                .ToListAsync();
        }

        public async Task AddProductAsync(ProductFormModel model)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                ProductType = model.ProductType,
                Package = model.Package,
                Price = model.Price,
                Vat = model.Price * 0.20m,
                Gain = model.Gain
            };

            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task<ProductFormModel?> GetProductForEditAsync(Guid id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null) return null;

            return new ProductFormModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ProductType = product.ProductType,
                Package = product.Package,
                Price = product.Price,
                Vat = product.Vat,
                Gain = product.Gain
            };
        }

        public async Task EditProductAsync(ProductFormModel model)
        {
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (product != null)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.ProductType = model.ProductType;
                product.Package = model.Package;
                product.Price = model.Price;
                product.Vat = model.Price * 0.20m;
                product.Gain = model.Gain;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleProductsAsync(List<Guid> ids)
        {
            var productsToDelete = await dbContext.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (productsToDelete.Any())
            {
                dbContext.Products.RemoveRange(productsToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}