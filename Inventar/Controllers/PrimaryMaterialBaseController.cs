using Inventar.Areas.Admin.Controllers;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Base;
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
    }
}