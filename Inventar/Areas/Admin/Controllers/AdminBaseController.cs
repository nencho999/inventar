using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public abstract class AdminBaseController : Controller
    {
        protected bool IsUserAuthenticated()
        {
            return this.User.Identity?.IsAuthenticated ?? false;
        }
        protected string? GetUserId()
        {
            string? userId = null;
            bool isAuthenticated = this.IsUserAuthenticated();
            if (isAuthenticated)
            {
                userId = this.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return userId;
        }
    }
}
