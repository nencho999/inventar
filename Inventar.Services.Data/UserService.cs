using Inventar.Data.Models;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Services.Data
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserListViewModel>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserListViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }

            return userList;
        }

        public async Task<UserFormViewModel> GetUserForEditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var userRoles = await _userManager.GetRolesAsync(user);
            var roleId = "";

            if (userRoles.Any())
            {
                var roleName = userRoles.First();
                var role = await _roleManager.FindByNameAsync(roleName);
                roleId = role?.Id;
            }

            return new UserFormViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                RoleId = roleId
            };
        }

        public async Task<bool> CreateUserAsync(UserFormViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUserAsync(UserFormViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return false;

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.UserName = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var newRole = await _roleManager.FindByIdAsync(model.RoleId);
            if (newRole != null)
            {
                await _userManager.AddToRoleAsync(user, newRole.Name);
            }

            return true;
        }

        public async Task<bool> ResetUserPasswordAsync(UserPasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            return result.Succeeded;
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }
    }
}
