using Microsoft.AspNetCore.Mvc;

namespace Inventar.Web.Controllers
{
    public class PrimaryMaterailBaseCotroller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
