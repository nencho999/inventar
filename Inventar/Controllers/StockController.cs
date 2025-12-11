using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventar.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class StockController : Controller
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
        public async Task<IActionResult> Inventory()
        {
            var model = await _stockService.GetStockLevelsAsync();
            return View(model);
        }

        private async Task PopulateDropdowns(StockTransactionViewModel model)
        {
            model.Bases = await _baseService.GetBasesDropdownAsync();
            model.Materials = await _stockService.GetMaterialsDropdownAsync();
        }
    }
}
