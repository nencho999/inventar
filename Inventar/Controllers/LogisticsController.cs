using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Logistics;
using Microsoft.AspNetCore.Mvc;

namespace Inventar.Web.Controllers
{
    public class LogisticsController : Controller
    {
        private readonly IProductionLogisticsService _logisticsService;

        public LogisticsController(IProductionLogisticsService logisticsService)
            => _logisticsService = logisticsService;

        [HttpGet]
        public async Task<IActionResult> WarehouseTransfer()
        {
            var model = new WarehouseTransferViewModel
            {
                Warehouses = await _logisticsService.GetAllWarehousesAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetStocks(Guid warehouseId)
        {
            var stocks = await _logisticsService.GetWarehouseProductStocksAsync(warehouseId);
            return Json(stocks);
        }

        [HttpPost]
        public async Task<IActionResult> WarehouseTransfer(WarehouseTransferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Warehouses = await _logisticsService.GetAllWarehousesAsync();
                return View(model);
            }

            var result = await _logisticsService.ExecuteWarehouseTransferAsync(model);
            if (result) return RedirectToAction("Index", "Production");

            ModelState.AddModelError("", "Грешка при трансфера. Проверете наличностите.");
            model.Warehouses = await _logisticsService.GetAllWarehousesAsync();
            return View(model);
        }
    }
}
