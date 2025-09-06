using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Core.Domain.Contracts.Services;
using MVC.Models.ViewModels;
using Utilities.User;
using Core.Domain.Contracts.Services.Identity;
using MVC.Controllers;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Admin.Helpers.Attributes;
using Core.Services;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CategoryController : BaseController
{
    #region Injection & Construction
    private readonly ICategoryService _categoryService;
    private readonly ITextProcessingService _textProcessingService;

    public CategoryController(ICategoryService categoryService, ITextProcessingService textProcessingService, IUserService userService, IMemoryCache memoryCache, IOptionService optionService) : base(userService, optionService, memoryCache)
    {
        _categoryService = categoryService;
        _textProcessingService = textProcessingService;
    }
    #endregion


    #region View
    [Permission("Category.Read")]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();

        #region Viewbags
        ViewBag.Title = "دسته بندی‌ها";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "دسته بندی جدید",
            Link = Url.Action("create", "category", new { area = "admin" }, Request.Scheme),
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
    [Permission("Category.Create")]
    public async Task<IActionResult> Create()
    {
        #region Categories List - Parent Id
        var categories = await _categoryService.GetAllAsync();
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
    [Permission("Category.Create")]
    public async Task<IActionResult> Create(CategoryVM model)
    {
        if (ModelState.IsValid)
        {
            var user = await this.GetCurrentUser(User.Identity.Name);
            if (user == null) return BadRequest();
            var category = new Category
            {
                UserId = user.Id,
                Label = _textProcessingService.SanitizeHtml(model.Label),
                Slug = _textProcessingService.SanitizeHtml(model.Slug),
                Description = _textProcessingService.SanitizeHtml(model.Description),
                ParentId = (model.ParentId != null) ? model.ParentId.Value : 0,

                CreationTime = DateTime.Now
            };
            await _categoryService.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }
        #region Categories List - Parent Id
        var categories = await _categoryService.GetAllAsync();
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
    [Permission("Category.Update")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var category = await _categoryService.GetByIdAsync(id.Value);
        if (category == null) return NotFound();

        var result = new CategoryVM
        {
            Id = category.Id,
            ParentId = category.ParentId,
            Label = category.Label,
            Slug = category.Slug,
            Description = category.Description,
        };

        #region Categpries List - Parent Id
        var categories = await _categoryService.GetAllAsync();
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
    [Permission("Category.Update")]
    public async Task<IActionResult> Edit(int id, CategoryVM model)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        model.Id = id;

        if (ModelState.IsValid)
        {
            try
            {
                category.Label = model.Label;
                category.Slug = model.Slug;
                category.Description = model.Description;
                if (model.ParentId != null) category.ParentId = model.ParentId.Value;
                category.LastModified = DateTime.Now;
                await _categoryService.UpdateAsync(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await CategoryExists(category.Id))) return NotFound();
                else TempData["ErrorMessage"] = "بروز رسانی دسته بندی انجام نشد";
            }
            return RedirectToAction(nameof(Index));
        }
        #region Categpries List - Parent Id
        var categories = await _categoryService.GetAllAsync();
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
    [Permission("Category.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await CategoryExists(id))
        {
            await _categoryService.DeleteAsync(id);
            return Json(new { result = true });

        }
        return Json(new { result = false });
    }

    private async Task<bool> CategoryExists(int id)
    {
        var cat =await _categoryService.GetByIdAsync(id);
        if (cat == null) return false;
        return true;
    }
    #endregion
}
