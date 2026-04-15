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
        [HttpGet]
        public async Task<IActionResult> CenterToWarehouse()
        {
            // Трябва да имаш метод, който връща обекти за центровете и складовете
            var model = await _logisticsService.GetTransferModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CenterToWarehouse(ProductionTransferFormModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await _logisticsService.GetTransferModelAsync();
                model.ProductionCenters = viewModel.ProductionCenters;
                model.Warehouses = viewModel.Warehouses;
                return View(model);
            }

            try
            {
                bool success = await _logisticsService.RegisterTransferAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Продукцията е преместена успешно!";
                    return RedirectToAction("Index", "Production");
                }
                ModelState.AddModelError("", "Недостатъчна наличност в центъра.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Грешка при трансфера.");
            }

            var retryModel = await _logisticsService.GetTransferModelAsync();
            model.ProductionCenters = retryModel.ProductionCenters;
            model.Warehouses = retryModel.Warehouses;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CenterToSalesPoint()
        {
            var model = await _logisticsService.GetSalesPointTransferModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CenterToSalesPoint(CenterToSalesPointFormModel model)
        {
            if (!ModelState.IsValid)
            {
                var refreshModel = await _logisticsService.GetSalesPointTransferModelAsync();
                model.ProductionCenters = refreshModel.ProductionCenters;
                model.SalesPoints = refreshModel.SalesPoints;
                return View(model);
            }

            bool success = await _logisticsService.RegisterSalesPointTransferAsync(model);

            if (success)
            {
                TempData["SuccessMessage"] = "Stock successfully transferred to Sales Point!";
                return RedirectToAction("Index", "Production");
            }

            ModelState.AddModelError("", "Insufficient stock in the production center.");

            var retryModel = await _logisticsService.GetSalesPointTransferModelAsync();
            model.ProductionCenters = retryModel.ProductionCenters;
            model.SalesPoints = retryModel.SalesPoints;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> WarehouseToSalesPoint()
        {
            var model = new WarehouseToSalesPointViewModel
            {
                Warehouses = await _logisticsService.GetAllWarehousesAsync(),
                SalesPoints = await _logisticsService.GetAllSalesPointsAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> WarehouseToSalesPoint(WarehouseToSalesPointViewModel model)
        {
            if (await _logisticsService.ExecuteWarehouseToSalesPointTransferAsync(model))
            {
                return RedirectToAction("Index", "Production");
            }
            model.Warehouses = await _logisticsService.GetAllWarehousesAsync();
            model.SalesPoints = await _logisticsService.GetAllSalesPointsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SalesActivity()
        {
            var model = new SalesActivityViewModel
            {
                SalesPoints = await _logisticsService.GetAllSalesPointsAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesPointStocks(Guid salesPointId)
        {
            var stocks = await _logisticsService.GetSalesPointProductsAsync(salesPointId);
            return Json(stocks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalesActivity(SalesActivityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SalesPoints = await _logisticsService.GetAllSalesPointsAsync();
                return View(model);
            }

            var result = await _logisticsService.RegisterSalesActivityAsync(model);

            if (result)
            {
                TempData["SuccessMessage"] = "Продажбите бяха регистрирани успешно!";
                return RedirectToAction("Index", "Production");
            }

            ModelState.AddModelError("", "Грешка при регистрация на продажбите. Проверете наличностите.");
            model.SalesPoints = await _logisticsService.GetAllSalesPointsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ReturnActivity()
        {
            var model = new SalesPointReturnViewModel
            {
                SalesPoints = await _logisticsService.GetAllSalesPointsAsync(),
                Warehouses = await _logisticsService.GetAllWarehousesAsync(),
                ProductionCenters = await _logisticsService.GetAllProductionCentersAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnActivity(SalesPointReturnViewModel model)
        {
            if (model.ToDestinationId == Guid.Empty || string.IsNullOrEmpty(model.DestinationType))
            {
                ModelState.AddModelError("", "Моля, изберете дестинация (Склад или Център).");
            }

            var success = await _logisticsService.RegisterSalesPointReturnAsync(model);

            if (success)
            {
                TempData["SuccessMessage"] = "Връщането на стока е регистрирано успешно!";
                return RedirectToAction("Index", "Production");
            }

            ModelState.AddModelError("", "Грешка при връщане. Проверете наличностите.");

            return View(model);
        }
    }
}
