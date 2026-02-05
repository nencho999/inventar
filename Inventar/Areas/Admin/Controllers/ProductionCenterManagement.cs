using Inventar.Areas.Admin.Controllers;
using Inventar.Data;
using Inventar.Services.Data;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels;
using Inventar.Web.ViewModels.ProductionCenter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using static Inventar.Common.Messages.ErrorMessages.ProductionCenter;

public class ProductionCenterManagement : AdminBaseController
{
    private readonly IProductionCenterService _service;
    private readonly IProductService _productService;
    public ProductionCenterManagement(IProductionCenterService service, IProductService productService)
    {
        _service = service;
        _productService = productService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var centers = await _service.GetAllCentersAsync();
        return View(centers);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CenterCreateInputModel();

        var dropdownData = await _productService.GetAllProductsAsync();
        model.Products = dropdownData.Select(p => new ProductDropdownViewModel
        {
            Id = p.Id,
            Name = p.Name
        }).ToList();

        ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CenterCreateInputModel model)
    {
        if (!ModelState.IsValid)
        {
            await ReloadMetadataAsync(model);
            return View(model);
        }

        try
        {
            await _service.AddCenterAsync(GetUserId()!, model);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            await ReloadMetadataAsync(model);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    private async Task ReloadMetadataAsync(CenterCreateInputModel model)
    {
        var products = await _productService.GetAllProductsAsync();
        model.Products = products.Select(p => new ProductDropdownViewModel
        {
            Id = p.Id,
            Name = p.Name
        }).ToList();

        ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid? id)
    {
        try
        {
            var model = await _service.GetCenterForEdittingAsync(GetUserId()!, id);
            var products = await _productService.GetProductsForDropdownAsync();
            model.Products = products.ToList();
            ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
            return View(model);
        }
        catch (ArgumentException ex)
        {
            if (ex.Message == CenterNotFoundErrorMessage)
            {
                return NotFound();
            }
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CenterEditFormModel model)
    {
        if (!ModelState.IsValid)
        {
            await ReloadMetadataAsync(model); // Методът, който пълни модела с продукти отново
            ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
            return View(model);
        }

        try
        {
            await _service.EditCenterAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            // Опционално: Логни грешката или пренасочи потребителя
            ModelState.AddModelError("", "The data was modified by another user. Please reload.");
            await ReloadMetadataAsync(model);
            return View(model);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Delete(Guid? id)
    {
        try
        {
            var model = await _service.GetCenterForDeletingAsync(GetUserId()!, id);
            return View(model);
        }
        catch (ArgumentException ex)
        {
            if (ex.Message == CenterNotFoundErrorMessage)
            {
                return NotFound();
            }
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(CenterDeleteViewModel model)
    {
        try
        {
            await _service.DeleteCenterAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            if (ex.Message == CenterNotFoundErrorMessage)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}