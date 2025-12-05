using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventar.Data;
using Inventar.Data.Models;
using Inventar.Models;
using Inventar.Services.Data.Contracts;
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

        public async Task<IEnumerable<PrimaryMaterialBase>> GetAllBasesAsync<T>()
        {
            return await _context.PrimaryMaterialBases
                .Include(b => b.Capacities)
                .ToListAsync();
        }

        public async Task<PrimaryMaterialBase?> GetBaseByIdAsync(Guid id)
        {
            return await _context.PrimaryMaterialBases
                .Include(b => b.Capacities)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddBaseAsync(PrimaryMaterialBase newBase)
        {
            _context.PrimaryMaterialBases.Add(newBase);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBaseAsync(PrimaryMaterialBase baseToUpdate)
        {
            _context.Entry(baseToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBaseAsync(Guid id)
        {
            var baseToDelete = _context.PrimaryMaterialBases.FindAsync(id);
            if (baseToDelete != null)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
