using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.BaseViewModels;
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

        public async Task<IEnumerable<BaseListViewModel>> GetAllBasesAsync()
        {
            return await _context.PrimaryMaterialBases
                .Include(b => b.Capacities)
                .Select(b => new BaseListViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    Address = b.Address,
                    CapacitySummary = string.Join(", ", b.Capacities.Select(c => $"{c.Quantity} {c.Unit} {c.Type}"))
                })
                .ToListAsync();
        }

        public async Task<BaseEditViewModel?> GetBaseForEditAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new BaseEditViewModel
                {
                    Id = Guid.Empty,
                    Capacities = new List<CapacityViewModel>{new CapacityViewModel()}
                };
            }

            var entity = await _context.PrimaryMaterialBases
                .Include(b => b.Capacities)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (entity == null) return null;

            return new BaseEditViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address,
                Description = entity.Description,
                Capacities = entity.Capacities.Select(c => new CapacityViewModel
                {
                    Id = c.Id,
                    Type = c.Type,
                    Quantity = c.Quantity,
                    Unit = c.Unit
                }).ToList()
            };
        }

        public async Task SaveBaseAsync(BaseEditViewModel model)
        {
            if (model.Id == Guid.Empty)
            {
                var newBase = new PrimaryMaterialBase()
                {
                    Name = model.Name,
                    Address = model.Address,
                    Description = model.Description,
                    Capacities = model.Capacities.Select(c => new Capacity
                    {
                        Type = c.Type,
                        Quantity = c.Quantity,
                        Unit = c.Unit
                    }).ToList()
                };
                await _context.PrimaryMaterialBases.AddAsync(newBase);
            }
            else
            {
                var entity = await _context.PrimaryMaterialBases
                    .Include(b => b.Capacities)
                    .FirstOrDefaultAsync(b => b.Id == model.Id);

                if (entity == null) throw new InvalidOperationException("Base not found.");
                entity.Name = model.Name;
                entity.Address = model.Address;
                entity.Description = model.Description;

                var capacitiesToRemove = entity.Capacities
                    .Where(c => !model.Capacities.Any(mc => mc.Id == c.Id && mc.Id != Guid.Empty))
                    .ToList();

                _context.Capacities.RemoveRange(capacitiesToRemove);

                foreach (var capModel in model.Capacities)
                {
                    if (capModel.Id != Guid.Empty)
                    {
                        var existingCapacity = entity.Capacities.FirstOrDefault(c => c.Id == capModel.Id);
                        if (existingCapacity != null)
                        {
                            existingCapacity.Type = capModel.Type;
                            existingCapacity.Quantity = capModel.Quantity;
                            existingCapacity.Unit = capModel.Unit;
                        }
                    }
                    else if (!string.IsNullOrEmpty(capModel.Type))
                    {
                        entity.Capacities.Add(new Capacity
                        {
                            Type = capModel.Type,
                            Quantity = capModel.Quantity,
                            Unit = capModel.Unit,
                            PrimaryMaterialBaseId = entity.Id
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
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
    }
}
