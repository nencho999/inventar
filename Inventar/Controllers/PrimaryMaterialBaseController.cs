using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventar.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class PrimaryMaterialBaseController : Controller
    {
        private readonly IPrimaryMaterialBaseService _baseService;

        public PrimaryMaterialBaseController(IPrimaryMaterialBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _baseService.GetDashboardAsync();
            return View(model);
        }

        [HttpGet]
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
        public async Task<IActionResult> Edit(BaseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _baseService.SaveBaseAsync(model);

            TempData["SuccessMessage"] = "The database has been saved successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _baseService.DeleteBaseAsync(id);
            TempData["SuccessMessage"] = "The database has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}