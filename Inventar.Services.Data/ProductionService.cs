using Inventar.Data;
using Inventar.Web.ViewModels.Production;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProductionService : IProductionService
{
    private readonly ApplicationDbContext _context;

    public ProductionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegisterProductionAsync(ProductionRegistrationFormModel model)
    {
        var storageEntry = await _context.ProductionCenterStorages
            .FirstOrDefaultAsync(s => s.ProductionCenterId == model.ProductionCenterId
                                   && s.ProductId == model.ProductId);

        if (storageEntry == null)
        {
            throw new InvalidOperationException("Този продукт не може да се съхранява в избрания център.");
        }

        storageEntry.CurrentStock += model.Quantity;

        if (storageEntry.MaxStorageCapacity > 0 && storageEntry.CurrentStock > storageEntry.MaxStorageCapacity)
        {
            throw new InvalidOperationException("Не може да се добавя повече количество, понеже капацитетът не позволява");
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductSelectViewModel>> GetAllowedProductsAsync(Guid centerId)
    {
        return await _context.ProductionCenterStorages
            .Where(s => s.ProductionCenterId == centerId)
            .Select(s => new ProductSelectViewModel
            {
                Id = s.ProductId,
                Name = s.Product.Name
            })
            .ToListAsync();
    }

    public async Task<ProductionRegistrationFormModel> GetRegistrationModelAsync()
    {
        return new ProductionRegistrationFormModel
        {
            ProductionCenters = await _context.ProductionCenters
                .Select(c => new ProductionCentersDropDown
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync()
        };
    }

}