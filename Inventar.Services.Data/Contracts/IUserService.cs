using Inventar.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Services.Data.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserListViewModel>> GetUsersAsync();
        Task<UserFormViewModel> GetUserForEditAsync(string id);
        Task<bool> CreateUserAsync(UserFormViewModel model);
        Task<bool> UpdateUserAsync(UserFormViewModel model);
        Task<bool> ResetUserPasswordAsync(UserPasswordViewModel model);
        Task DeleteUserAsync(string id);
        Task<List<IdentityRole>> GetAllRolesAsync();
    }
}
