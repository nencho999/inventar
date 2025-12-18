using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Stock;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Inventar.Areas.Admin.Controllers;

namespace Inventar.Web.Areas.Admin.Controllers
{
    public class StockController : AdminBaseController
    {
        private readonly IStockService _stockService;
        private readonly IPrimaryMaterialBaseService _baseService;

        public StockController(IStockService stockService, IPrimaryMaterialBaseService baseService)
        {
            _stockService = stockService;
            _baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> RecordMovement()
        {
            var model = new StockTransactionViewModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordMovement(StockTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);

                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _stockService.RecordTransactionAsync(model, userId);

                TempData["SuccessMessage"] = model.IsAcquisition
                    ? "Stock added successfully!"
                    : "Stock removed successfully!";

                return RedirectToAction(nameof(RecordMovement));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving: " + ex.Message);
                await PopulateDropdowns(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Inventory(Guid? baseId, Guid? materialId, string search)
        {
            var stockData = await _stockService.GetStockLevelsAsync(baseId, materialId, search);

            var bases = await _baseService.GetBasesDropdownAsync();
            var materials = await _stockService.GetMaterialsDropdownAsync();

            var model = new InventoryFilterViewModel
            {
                StockLevels = stockData,
                BaseId = baseId,
                MaterialId = materialId,
                SearchTerm = search,
                Bases = bases,
                Materials = materials
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var model = await _stockService.GetLastTransactionsAsync();
            return View(model);
        }

        private async Task PopulateDropdowns(StockTransactionViewModel model)
        {
            model.Bases = await _baseService.GetBasesDropdownAsync();
            model.Materials = await _stockService.GetMaterialsDropdownAsync();
        }
    }
}
