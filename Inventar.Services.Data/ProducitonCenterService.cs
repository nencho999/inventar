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
            .Select(c => new CenterIndexViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Status,
                Location = c.Location,
                Contact = c.Contact,
                Capacity = c.Capacity,
                Expenses = c.Expenses
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
        bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");

        if (user == null || !isAdmin)
        {
            throw new ArgumentException(AdminAuthorization);
        }

        var center = new ProductionCenter
        {
            Name = model.Name,
            Location = model.Location,
            Capacity = model.Capacity,
            Contact = model.Contact,
            Status = model.Status,
            Expenses = model.Expenses
        };

        foreach (var storage in model.Storages)
        {
            center.StorageCapacities.Add(new ProductionCenterStorage
            {
                MaterialId = storage.MaterialId,
                MaxStorageCapacity = storage.MaxCapacity
            });
        }

        foreach (var expenseModel in model.ExpensesList)
        {
            center.ExpensesList.Add(new ProductionCenterExpense
            {
                Name = expenseModel.Name,
                Amount = expenseModel.Amount,
                Type = expenseModel.Type,
                Date = expenseModel.Type == ExpenseType.OneTime ? expenseModel.Date : null,
                Frequency = expenseModel.Type == ExpenseType.Regular ? expenseModel.Frequency : null,
                EveryXMonths = (expenseModel.Type == ExpenseType.Regular && expenseModel.Frequency == Frequency.CustomMonths)
                                ? expenseModel.EveryXMonths
                                : null
            });
        }

        await dbContext.AddAsync(center);
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
            .Include(c => c.ExpensesList) // Добавяме това!
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
                MaterialId = s.MaterialId,
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
        var center = await dbContext.ProductionCenters
            .Include(c => c.StorageCapacities)
            .Include(c => c.ExpensesList) // Трябва да ги включим, за да ги изчистим
            .FirstOrDefaultAsync(c => c.Id == model.Id);

        if (center == null)
        {
            throw new ArgumentException("Center not found");
        }

        // Обновяване на базови данни
        center.Name = model.Name;
        center.Location = model.Location;
        center.Capacity = model.Capacity;
        center.Contact = model.Contact;
        center.Status = model.Status;
        center.Expenses = model.Expenses;

        center.StorageCapacities.Clear();
        foreach (var s in model.Storages)
        {
            center.StorageCapacities.Add(new ProductionCenterStorage
            {
                MaterialId = s.MaterialId,
                MaxStorageCapacity = s.MaxCapacity
            });
        }

        center.ExpensesList.Clear();
        foreach (var e in model.ExpensesList)
        {
            center.ExpensesList.Add(new ProductionCenterExpense
            {
                Name = e.Name,
                Amount = e.Amount,
                Type = e.Type,
                Date = e.Type == ExpenseType.OneTime ? e.Date : null,
                Frequency = e.Type == ExpenseType.Regular ? e.Frequency : null,
                EveryXMonths = (e.Type == ExpenseType.Regular && e.Frequency == Frequency.CustomMonths)
                                ? e.EveryXMonths
                                : null
            });
        }

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