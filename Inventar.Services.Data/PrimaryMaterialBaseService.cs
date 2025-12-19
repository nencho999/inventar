using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels;
using Inventar.Web.ViewModels.Base;
using Inventar.Web.ViewModels.ProductionCenter;
using Microsoft.EntityFrameworkCore;

namespace Inventar.Services.Data
{
    public class PrimaryMaterialBaseService : IPrimaryMaterialBaseService
    {
        private readonly ApplicationDbContext _context;

        public PrimaryMaterialBaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseFormViewModel> GetBaseForEditAsync(Guid id)
        {
            var allMaterials = await _context.Materials.ToListAsync();

            BaseFormViewModel model;

            if (id == Guid.Empty)
            {
                model = new BaseFormViewModel();
            }
            else
            {
                var entity = await _context.PrimaryMaterialBases
                    .Include(b => b.Capacities)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (entity == null) return null;

                model = new BaseFormViewModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Address = entity.Address,
                    Description = entity.Description
                };
            }

            model.Capacities = allMaterials.Select(m => new BaseCapacityViewModel
            {
                MaterialId = m.Id,
                MaterialName = $"{m.Name} ({m.Unit})",
                Limit = id == Guid.Empty ? 0 :
                        _context.Capacities
                            .Where(bc => bc.PrimaryMaterialBaseId == id && bc.MaterialId == m.Id)
                            .Select(bc => bc.CapacityLimit)
                            .FirstOrDefault(),
                IsSelected = (id != Guid.Empty && _context.Capacities.Any(bc => bc.PrimaryMaterialBaseId == id && bc.MaterialId == m.Id))
            }).ToList();

            return model;
        }

        public async Task<DashboardViewModel> GetDashboardAsync()
        {
            var basesEntities = await _context.PrimaryMaterialBases
                .Include(b => b.Expenses)
                .Include(b => b.RecurringExpenses)
                .ToListAsync();

            var baseViewModels = basesEntities.Select(b => new BaseListViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Description = b.Description,
                OneTimeExpenses = b.Expenses.Sum(e => e.Amount),
                MonthlyExpenses = b.RecurringExpenses
                    .Where(re => re.IsActive)
                    .Sum(re => re.Frequency == ExpenseFrequency.Monthly ? re.Amount :
                    re.Frequency == ExpenseFrequency.Weekly ? re.Amount * 4.33m :
                    re.Frequency == ExpenseFrequency.Yearly ? re.Amount / 12m :
                    (re.Frequency == ExpenseFrequency.CustomMonthInterval && re.IntervalMonths > 0)
                    ? re.Amount / (decimal)re.IntervalMonths
                    : 0)
            }).ToList();

            var dashboard = new DashboardViewModel
            {
                TotalBases = baseViewModels.Count,
                TotalOneTimeExpenses = baseViewModels.Sum(x => x.OneTimeExpenses),
                TotalMonthlyExpenses = baseViewModels.Sum(x => x.MonthlyExpenses),
                Bases = baseViewModels
            };

            return dashboard;
        }

        public async Task SaveBaseAsync(BaseFormViewModel model)
        {
            PrimaryMaterialBase entity;

            if (model.Id == Guid.Empty)
            {
                entity = new PrimaryMaterialBase();
                await _context.PrimaryMaterialBases.AddAsync(entity);
            }
            else
            {
                entity = await _context.PrimaryMaterialBases.FindAsync(model.Id);
                if (entity == null) return;
            }

            entity.Name = model.Name;
            entity.Address = model.Address;
            entity.Description = model.Description;

            await _context.SaveChangesAsync();

            var existingCapacities = _context.Capacities.Where(bc => bc.PrimaryMaterialBaseId == entity.Id);
            _context.Capacities.RemoveRange(existingCapacities);

            foreach (var row in model.Capacities.Where(c => c.Limit > 0))
            {
                _context.Capacities.Add(new Capacity
                {
                    PrimaryMaterialBaseId = entity.Id,
                    MaterialId = row.MaterialId,
                    CapacityLimit = row.Limit
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBaseAsync(Guid id)
        {
            var entity = await _context.PrimaryMaterialBases.FindAsync(id);
            if (entity != null)
            {
                _context.PrimaryMaterialBases.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DropdownPairViewModel>> GetBasesDropdownAsync()
        {
            return await _context.PrimaryMaterialBases
                .Select(b => new DropdownPairViewModel
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialDropDownModel>> GetMaterialsDropdownAsync()
        {
            var materials = await _context.Materials
                .AsNoTracking()
                .Select(m => new MaterialDropDownModel
                {
                    Id = m.Id,
                    Name = m.Name,
                })
                .ToListAsync();

            return materials;
        }
    }
}