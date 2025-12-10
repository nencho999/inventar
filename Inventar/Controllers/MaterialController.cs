using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventar.Web.Controllers
{
    [Authorize]
    public class MaterialController : Controller
    {
        private readonly IStockService _stockService;

        public MaterialController(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _stockService.GetAllMaterialsListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id.HasValue)
            {
                var materials = await _stockService.GetAllMaterialsListAsync();
                var material = materials.FirstOrDefault(m => m.Id == id.Value);
                if (material == null) return NotFound();
                return View(material);
            }

            return View(new MaterialViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MaterialViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _stockService.SaveMaterialAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _stockService.DeleteMaterialAsync(id);
            }
            catch
            {
                TempData["ErrorMessage"] = "You cannot delete material that already has a movement history!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}