using Inventar.Data.Models;
using Inventar.Web.ViewModels.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Services.Data.Contracts
{
    public interface IWarehouseService
    {
        Task<Warehouse> GetByIdAsync(Guid id);
        Task<WarehouseIndexViewModel> GetDashboardDataAsync(string search = null);
        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
        Task<WarehouseFormViewModel> GetForEditAsync(Guid id);
        Task CreateAsync(WarehouseFormViewModel model);
        Task UpdateAsync(WarehouseFormViewModel model);
        Task DeleteAsync(Guid id);
        Task<WarehouseAddExpenseViewModel> GetExpenseFormAsync(Guid warehouseId);
        Task AddExpenseAsync(WarehouseAddExpenseViewModel model, string userId);
    }
}
