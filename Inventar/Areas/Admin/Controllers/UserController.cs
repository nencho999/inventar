using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await GetRolesDropdown();

            return View(new UserFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("Password", "Password is required for new users.");
                    ViewBag.Roles = await GetRolesDropdown();
                    return View(model);
                }

                var success = await _userService.CreateUserAsync(model);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error creating user. Email might be taken.");
            }

            ViewBag.Roles = await GetRolesDropdown();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _userService.GetUserForEditAsync(id);
            if (model == null) return NotFound();

            ViewBag.Roles = await GetRolesDropdown();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var success = await _userService.UpdateUserAsync(model);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error updating user.");
            }

            ViewBag.Roles = await GetRolesDropdown();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await _userService.GetUserForEditAsync(id);
            if (user == null) return NotFound();

            var model = new UserPasswordViewModel
            {
                UserId = id,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _userService.ResetUserPasswordAsync(model);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error resetting password.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<SelectList> GetRolesDropdown()
        {
            var roles = await _userService.GetAllRolesAsync();

            return new SelectList(roles, "Id", "Name");
        }
    }
}
