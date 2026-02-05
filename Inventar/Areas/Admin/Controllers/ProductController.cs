using Inventar.Areas.Admin.Controllers;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class ProductController : AdminBaseController
{
    private readonly IProductService productService;

    public ProductController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await productService.GetAllProductsAsync();
        return View(model);
    }
    [HttpGet]
    public IActionResult Create() => View(new ProductFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await productService.AddProductAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var model = await productService.GetProductForEditAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await productService.EditProductAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSelected(List<Guid> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
        {
            return RedirectToAction(nameof(Index));
        }

        await productService.DeleteMultipleProductsAsync(selectedIds);
        return RedirectToAction(nameof(Index));
    }
}
