using Microsoft.AspNetCore.Mvc;

public class ProductionController : Controller
{
    private readonly IProductionService _productionService;

    public ProductionController(IProductionService productionService)
    {
        _productionService = productionService;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Registration()
    {
        var model = await _productionService.GetRegistrationModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registration(ProductionRegistrationFormModel model)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await _productionService.GetRegistrationModelAsync();
            model.ProductionCenters = viewModel.ProductionCenters;
            return View(model);
        }

        try
        {
            await _productionService.RegisterProductionAsync(model);

            TempData["SuccessMessage"] = "Успешно регистрирано производство!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Възникна грешка тъй като капацитетът за продукта е по-малък от исканото добавено количество");
        }

        var retryModel = await _productionService.GetRegistrationModelAsync();
        model.ProductionCenters = retryModel.ProductionCenters;

        return View(model);
    }

    [HttpGet]
    public async Task<JsonResult> GetProducts(Guid centerId)
    {
        var products = await _productionService.GetAllowedProductsAsync(centerId);

        return Json(products);
    }
}