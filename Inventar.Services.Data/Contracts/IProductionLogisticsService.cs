using Inventar.Web.ViewModels.Logistics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Services.Data.Contracts
{
    public interface IProductionLogisticsService
    {
        Task<IEnumerable<SelectListItem>> GetAllWarehousesAsync();
        Task<List<TransferProductItemViewModel>> GetWarehouseProductStocksAsync(Guid warehouseId);
        Task<bool> ExecuteWarehouseTransferAsync(WarehouseTransferViewModel model);
    }
}
