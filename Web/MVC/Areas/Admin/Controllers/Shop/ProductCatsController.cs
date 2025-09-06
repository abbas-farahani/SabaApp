using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVC.Controllers;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Admin.Helpers.Attributes;
using MVC.Models.ViewModels;
using Core.Domain.Contracts.Services.Shop;
using Core.Services.Shop;
using Core.Domain.Entities.Shop;
using Utilities.User;
using Core.Domain.Dtos.Shop;

namespace MVC.Areas.Admin.Controllers.Shop;

[Area("Admin")]
[Authorize]
public class ProductCatsController : BaseController
{
    #region Injection & Construction
    private readonly IProductCatService _productCatService;
    private readonly ITextProcessingService _textProcessingService;

    public ProductCatsController(IProductCatService productCatService, ITextProcessingService textProcessingService, IUserService userService, IMemoryCache memoryCache, IOptionService optionService) : base(userService, optionService, memoryCache)
    {
        _productCatService = productCatService;
        _textProcessingService = textProcessingService;
    }
    #endregion


    #region View
    [Permission("ProductCat.Read")]
    public async Task<IActionResult> Index()
    {
        var categories = await _productCatService.GetAllAsync();

        #region Viewbags
        ViewBag.Title = "دسته بندی‌ها";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "دسته بندی جدید",
            Link = Url.Action("create", "productcats", new { area = "admin" }, Request.Scheme),
            Title = "دسته بندی جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="دسته بندی‌ها", Title="دسته بندی‌ها"},
        };
        #endregion
        return View(categories.ToList());
    }
    #endregion

    #region Create
    // GET: Admin/Category/Create
    [Permission("ProductCat.Create")]
    public async Task<IActionResult> Create()
    {
        #region Categories List - Parent Id
        var categories = await _productCatService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "بدون والد", Selected = true } // Default Item
        };
        selectListItems.AddRange(categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
        }));
        ViewData["ParentId"] = selectListItems;
        #endregion
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("ProductCat.Create")]
    public async Task<IActionResult> Create(ProductCatDto model)
    {
        if (ModelState.IsValid)
        {
            model.UserId = User.GetUserId();
            model.Label = _textProcessingService.SanitizeHtml(model.Label);
            model.Slug = _textProcessingService.SanitizeHtml(model.Slug);
            model.Description = _textProcessingService.SanitizeHtml(model.Description);
            model.ParentId = (model.ParentId != null) ? model.ParentId : 0;

            await _productCatService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }
        #region Categories List - Parent Id
        var categories = await _productCatService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "بدون والد", Selected = true } // Default Item
        };
        selectListItems.AddRange(categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
        }));
        ViewData["ParentId"] = selectListItems;
        #endregion
        return View(model);
    }
    #endregion

    #region Edit
    [Permission("ProductCat.Update")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var category = await _productCatService.GetByIdAsync(id.Value);
        if (category == null) return NotFound();

        var result = new ProductCatDto
        {
            Id = category.Id,
            ParentId = category.ParentId,
            Label = category.Label,
            Slug = category.Slug,
            Description = category.Description,
        };

        #region Categpries List - Parent Id
        var categories = await _productCatService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "بدون والد" } // Default Item
        };
        selectListItems.AddRange(categories.Where(x => x.Id != id.Value).Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
            Selected = (c.Id == id) ? true : false
        }));
        ViewData["ParentId"] = selectListItems;
        #endregion
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("ProductCat.Update")]
    public async Task<IActionResult> Edit(int id, ProductCatDto model)
    {
        var category = await _productCatService.GetByIdAsync(id);
        if (category == null) return NotFound();
        model.Id = id;

        if (ModelState.IsValid)
        {
            try
            {
                category.Label = _textProcessingService.SanitizeHtml(model.Label);
                category.Slug = _textProcessingService.SanitizeHtml(model.Slug);
                category.Description = _textProcessingService.SanitizeHtml(model.Description);
                if (model.ParentId != null) category.ParentId = model.ParentId.Value;
                category.LastModified = DateTime.Now;
                await _productCatService.UpdateAsync(category);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!(await CategoryExists(category.Id))) return NotFound();
                else TempData["ErrorMessage"] = "بروز رسانی دسته بندی انجام نشد";
            }
            return RedirectToAction(nameof(Index));
        }
        #region Categpries List - Parent Id
        var categories = await _productCatService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "بدون والد" } // Default Item
        };
        selectListItems.AddRange(categories.Where(x => x.Id != id).Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
            Selected = (c.Id == id) ? true : false
        }));
        ViewData["ParentId"] = selectListItems;
        #endregion
        return View(category);
    }
    #endregion

    #region Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("ProductCat.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await CategoryExists(id))
        {
            await _productCatService.DeleteAsync(id);
            return Json(new { result = true });

        }
        return Json(new { result = false });
    }

    private async Task<bool> CategoryExists(int id)
    {
        var cat = await _productCatService.GetByIdAsync(id);
        if (cat == null) return false;
        return true;
    }
    #endregion
}
