using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Inventar.Web.ViewModels;

namespace Inventar.Web.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {   
        private readonly IExpenseService _expenseService;
        private readonly IPrimaryMaterialBaseService _baseService;

        public ExpenseController(IExpenseService expenseService, IPrimaryMaterialBaseService baseService)
        {
            _expenseService = expenseService;
            _baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? baseId)
        {
            var model = new ExpenseFormViewModel();

            if (baseId.HasValue)
            {
                model.BaseId = baseId.Value;
            }

            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            await _expenseService.CreateExpenseAsync(model, userId, isAdmin);

            TempData["SuccessMessage"] = "The expense has been registered successfully!";
            return RedirectToAction(nameof(Create));
        }

        private async Task PopulateDropdowns(ExpenseFormViewModel model)
        {
            model.Bases = await _baseService.GetBasesDropdownAsync();
        }
    }
}
