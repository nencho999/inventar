using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.BaseViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Inventar.Web.Controllers
{
    public class PrimaryMaterialBaseController : Controller
    {
        private readonly IPrimaryMaterialBaseService _service;

        public PrimaryMaterialBaseController(IPrimaryMaterialBaseService service)
        {
            _service = service;
        }

        //GET: /Base/Index
        public async Task<IActionResult> Index()
        {
            var model = await _service.GetAllBasesAsync();
            return View(model);
        }

        //GET: /Base/Edit/{id}
        public async Task<IActionResult> Edit(Guid? id)
        {
            var model = await _service.GetBaseForEditAsync(id??Guid.Empty);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST: /Base/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BaseEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _service.SaveBaseAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        //POST: /Base/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteBaseAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
