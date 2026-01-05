using Inventar.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inventar.Web.ViewModels.SalesPoint;
using Inventar.Services.Data.Contracts;
using Inventar.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SalesPointController : Controller
    {
        private readonly ISalesPointService _service;

        public SalesPointController(ISalesPointService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string search)
        {
            var model = await _service.GetIndexDataAsync(search);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _service.GetFormForCreateAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesPointFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _service.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.GetFormForEditAsync(id);
            if (model == null) return NotFound();
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SalesPointFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            await _service.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddExpense(Guid id)
        {
            var model = await _service.GetExpenseFormAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(SalesPointAddExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _service.AddExpenseAsync(model);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _service.GetDeleteDetailsAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}