using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.ProductionCenter;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using static Inventar.Common.Messages.ErrorMessages.Admin;
using static Inventar.Common.Messages.ErrorMessages.ProductionCenter;

public class ProductionCenterService : IProductionCenterService
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IStringLocalizer _localizer;

    public ProductionCenterService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,
                                    IStringLocalizerFactory factory)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        _localizer = factory.Create("CommonMessages", "Inventar.Web");
    }
    public async Task<IEnumerable<CenterIndexViewModel>> GetAllCentersAsync()
    {
        var centers = await dbContext.ProductionCenters
            .Include(c => c.ExpensesList)
            .Select(c => new CenterIndexViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Status,
                Location = c.Location,
                Contact = c.Contact,
                Capacity = c.Capacity,
                Expenses = c.Expenses,
                ExpensesList = c.ExpensesList.Select(e => new ExpenseInputModel
                {
                    Amount = e.Amount,
                    Type = e.Type
                }).ToList(),

            })
            .ToListAsync();

        return centers;
    }

    public async Task<ProductionCenter> FindCenterByIdAsync(Guid? id)
    {
        if (id.HasValue)
        {
            var center = await dbContext.ProductionCenters
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (center == null)
            {
                throw new ArgumentException(CenterNotFoundErrorMessage);
            }
            return center;
        }
        else
        {
            throw new ArgumentException(CenterCannotBeNullErrorMessage);
        }
    }

    public async Task<bool> AddCenterAsync(string userId, CenterCreateInputModel model)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "Admin"))
        {
            throw new ArgumentException("Admin authorization required.");
        }

        var center = new ProductionCenter
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Location = model.Location,
            Capacity = model.Capacity,
            Contact = model.Contact,
            Status = model.Status,
            Expenses = model.Expenses
        };

        if (model.Storages != null)
        {
            foreach (var storage in model.Storages)
            {
                if (storage.MaterialId != Guid.Empty)
                {
                    center.StorageCapacities.Add(new ProductionCenterStorage
                    {
                        ProductionCenterId = center.Id,
                        ProductId = storage.MaterialId,
                        MaxStorageCapacity = storage.MaxCapacity
                    });
                }
            }
        }

        foreach (var e in model.ExpensesList)
        {
            center.ExpensesList.Add(new ProductionCenterExpense
            {
                Id = Guid.NewGuid(),
                Name = e.Name,
                Amount = e.Amount,
                Type = e.Type,
                Date = e.Type == ExpenseType.OneTime ? e.Date : null,
                Frequency = e.Type == ExpenseType.Regular ? e.Frequency : null,
                EveryXMonths = (e.Type == ExpenseType.Regular && e.Frequency == Frequency.CustomMonths)
                               ? e.EveryXMonths : null
            });
        }

        await dbContext.ProductionCenters.AddAsync(center);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<CenterEditFormModel> GetCenterForEdittingAsync(string userId, Guid? id)
    {
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
        if (user == null || !isAdmin)
        {
            throw new ArgumentException(AdminAuthorization);
        }

        var center = await dbContext.ProductionCenters
            .Include(c => c.StorageCapacities)
            .Include(c => c.ExpensesList)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (center == null) throw new ArgumentException("Center not found");

        var model = new CenterEditFormModel
        {
            Id = center.Id,
            Name = center.Name,
            Location = center.Location,
            Capacity = center.Capacity,
            Contact = center.Contact,
            Status = center.Status,
            Expenses = center.Expenses,
            Storages = center.StorageCapacities.Select(s => new StorageInputModel
            {
                MaterialId = s.ProductId,
                MaxCapacity = s.MaxStorageCapacity
            }).ToList(),
            ExpensesList = center.ExpensesList.Select(e => new ExpenseInputModel
            {
                Name = e.Name,
                Amount = e.Amount,
                Type = e.Type,
                Date = e.Date,
                Frequency = e.Frequency,
                EveryXMonths = e.EveryXMonths
            }).ToList()
        };

        return model;
    }

    public async Task<bool> EditCenterAsync(CenterEditFormModel model)
    {
        // 1. Зареждаме центъра със свързаните му обекти
        var center = await dbContext.ProductionCenters
            .Include(c => c.StorageCapacities)
            .Include(c => c.ExpensesList)
            .FirstOrDefaultAsync(c => c.Id == model.Id);

        if (center == null) throw new ArgumentException("Center not found");

        // 2. Обновяваме основните свойства на центъра
        center.Name = model.Name;
        center.Location = model.Location;
        center.Capacity = model.Capacity;
        center.Contact = model.Contact;
        center.Expenses = model.Expenses;
        center.Status = model.Status;

        if (center.StorageCapacities.Any())
        {
            dbContext.ProductionCenterStorages.RemoveRange(center.StorageCapacities);
        }

        if (center.ExpensesList.Any())
        {
            dbContext.ProductionCenterExpenses.RemoveRange(center.ExpensesList);
        }

        // КРИТИЧНО: Записваме изтриването СЕГА. 
        // Това предотвратява Concurrency грешката, като казва на базата, че тези редове са премахнати.
        await dbContext.SaveChangesAsync();

        // 4. ВТОРА СТЪПКА: Добавяме новите записи от формата
        // Добавяме ги директно към DbSet, за да избегнем проблеми с навигационните свойства
        if (model.Storages != null)
        {
            foreach (var s in model.Storages)
            {
                await dbContext.ProductionCenterStorages.AddAsync(new ProductionCenterStorage
                {
                    Id = Guid.NewGuid(), // Нов ключ
                    ProductionCenterId = center.Id,
                    ProductId = s.MaterialId,
                    MaxStorageCapacity = (double)s.MaxCapacity
                });
            }
        }

        if (model.ExpensesList != null)
        {
            foreach (var e in model.ExpensesList)
            {
                await dbContext.ProductionCenterExpenses.AddAsync(new ProductionCenterExpense
                {
                    Id = Guid.NewGuid(), // Нов ключ
                    ProductionCenterId = center.Id,
                    Name = e.Name,
                    Amount = e.Amount,
                    Type = e.Type,
                    Date = e.Type == ExpenseType.OneTime ? e.Date : null,
                    Frequency = e.Type == ExpenseType.Regular ? e.Frequency : null,
                    EveryXMonths = (e.Type == ExpenseType.Regular && e.Frequency == Frequency.CustomMonths)
                                   ? e.EveryXMonths : null
                });
            }
        }

        // 5. Финален запис на новите данни
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<CenterDeleteViewModel> GetCenterForDeletingAsync(string userId, Guid? id)
    {
        var user = await userManager.FindByIdAsync(userId);
        bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
        if (user == null || !isAdmin)
        {
            throw new ArgumentException(AdminAuthorization);
        }

        var center = await FindCenterByIdAsync(id);

        var model = new CenterDeleteViewModel
        {
            Id = center.Id,
            Name = center.Name,
            Location = center.Location,
        };

        return model;
    }

    public async Task<bool> DeleteCenterAsync(CenterDeleteViewModel model)
    {
        var center = await dbContext.ProductionCenters.FindAsync(model.Id);

        if (center == null)
        {
            throw new ArgumentException(CenterNotFoundErrorMessage);
        }

        dbContext.ProductionCenters.Remove(center);
        await dbContext.SaveChangesAsync();

        return true;
    }
    public async Task<IEnumerable<SelectListItem>> GetCenterStatusSelectListAsync(CenterStatus? selectedStatus = null)
    {
        var statusList = Enum.GetValues(typeof(CenterStatus))
                             .Cast<CenterStatus>()
                             .Select(s => new SelectListItem
                             {
                                 Value = ((int)s).ToString(),
                                 Text = _localizer[s.ToString()],
                                 Selected = selectedStatus.HasValue && selectedStatus.Value == s
                             }).ToList();

        statusList.Insert(0, new SelectListItem
        {
            Value = "",
            Text = _localizer["--- Select Center Status ---"],
            Disabled = true,
            Selected = !selectedStatus.HasValue
        });

        return statusList;
    }
}