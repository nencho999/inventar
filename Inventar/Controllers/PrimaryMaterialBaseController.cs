using System.Security.Claims;
using Inventar.Areas.Admin.Controllers;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Base;
using Inventar.Web.ViewModels.PrimaryBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using static Inventar.Common.Messages.SuccessMessages.PrimaryMaterialBase;

namespace Inventar.Web.Controllers
{
    [Authorize]
    public class PrimaryMaterialBaseController : Controller
    {
        private readonly IPrimaryMaterialBaseService _baseService;
        private readonly IStockService _stockService;

        public PrimaryMaterialBaseController(IPrimaryMaterialBaseService baseService, IStockService stockService)
        {
            _baseService = baseService;
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _baseService.GetDashboardAsync();
            var stockData = await _stockService.GetStockLevelsAsync();
            model.TotalInventoryValue = stockData.Sum(x => x.TotalValue);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _baseService.GetBaseForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(BaseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _baseService.SaveBaseAsync(model);

            TempData["SuccessMessage"] = BaseSavedSuccessfully;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var baseModel = await _baseService.GetBaseForEditAsync(id);
            if (baseModel == null) return NotFound();

            var model = new BaseFormViewModel
            {
                Id = baseModel.Id,
                Name = baseModel.Name,
                Address = baseModel.Address,
                Description = baseModel.Description
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _baseService.DeleteBaseAsync(id);

            TempData["SuccessMessage"] = "Base deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Expenses(string type = "All")
        {
            var model = await _baseService.GetBaseExpensesByTypeAsync(type);

            ViewBag.PageTitle = type == "Monthly" ? "Monthly Recurring Expenses (Bases)" :
                type == "OneTime" ? "One-Time Expenses (Bases)" : "All Base Expenses";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddExpense(Guid id)
        {
            var model = await _baseService.GetExpenseFormAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(PrimaryBaseAddExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _baseService.AddExpenseAsync(model, userId);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
}
}