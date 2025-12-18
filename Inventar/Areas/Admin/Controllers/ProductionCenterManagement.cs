using Inventar.Areas.Admin.Controllers;
using Inventar.Data;
using Inventar.Services.Data.Contracts;
using Inventar.Web.ViewModels.ProductionCenter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Inventar.Common.Messages.ErrorMessages.ProductionCenter;

public class ProductionCenterManagement : AdminBaseController
{
    private readonly IProductionCenterService _service;
    private readonly IPrimaryMaterialBaseService _materialService;
    public ProductionCenterManagement(IProductionCenterService service, IPrimaryMaterialBaseService materialService)
    {
        _service = service;
        _materialService = materialService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var centers = await _service.GetAllCentersAsync();
        return View(centers);
    }
    [HttpGet]
    public async Task <IActionResult> Create()
    {
        var model = new CenterCreateInputModel();
        var dropdownData = await _materialService.GetMaterialsDropdownAsync();
        model.Materials = dropdownData.ToList();

        ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(CenterCreateInputModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
            return View(model);
        }

        try
        {
            await _service.AddCenterAsync(GetUserId()!, model);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid? id)
    {
        try
        {
            var model = await _service.GetCenterForEdittingAsync(GetUserId()!, id);
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
            ViewBag.CenterStatuses = await _service.GetCenterStatusSelectListAsync();
            return View(model);
        }

        try
        { 
            await _service.EditCenterAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
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