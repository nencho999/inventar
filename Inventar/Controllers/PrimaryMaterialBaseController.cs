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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _baseService.DeleteBaseAsync(id);
            TempData["SuccessMessage"] = BaseDeletedSuccessfully;
            return RedirectToAction(nameof(Index));
        }
    }
}