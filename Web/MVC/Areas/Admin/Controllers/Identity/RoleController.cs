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
using Core.Domain.Contracts.Services.Identity;
using Microsoft.AspNetCore.Identity;
using MVC.Controllers;
using Microsoft.Extensions.Caching.Memory;
using MVC.Models.ViewModels;
using System.Threading;
using System.Text.Json;
using MVC.Areas.Admin.Helpers.Attributes;
using Core.Domain.Contracts.Services;

namespace MVC.Areas.Admin.Controllers.Identity;

[Area("Admin")]
[Authorize]
public class RoleController : BaseController
{

    #region Injection & Construction
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    public RoleController(IRoleService roleService, IPermissionService permissionService, IUserService userService,IOptionService optionService, IMemoryCache memoryCache) : base(userService, optionService, memoryCache)
    {
        _roleService = roleService;
        _permissionService = permissionService;
    }
    #endregion


    #region View
    [Permission("Role.Read")]
    public async Task<IActionResult> Index()
    {
        #region Viewbags
        ViewBag.Title = "نقش‌ها";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "نقش جدید",
            Link = Url.Action("create", "role", new { area = "admin" }, Request.Scheme),
            Title = "نقش جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="همه نقش‌ها", Title="همه نقش‌ها"},
        };
        #endregion
        return View(await _roleService.GetAllAsync());
    }

    [Permission("Role.Read")]
    public async Task<IActionResult> Details(string id)
    {
        if (id == null) return NotFound();

        var role = await _roleService.GetByIdAsync(id);
        if (role == null) return NotFound();

        #region Viewbags
        ViewBag.Title = $"نقش {role.Name}";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "ویرایش نقش",
            Link = Url.Action("edit", "role", new { area = "admin", id = id }, Request.Scheme),
            Title = "ویرایش نقش",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="نقش‌ها", Title="نقش‌ها", Link=Url.Action("index", "role", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text=$"نقش {role.Name}", Title="نقش"},
        };
        #endregion
        return View(role);
    }
    #endregion

    #region Create
    [Permission("Role.Create")]
    public async Task<IActionResult> Create()
    {
        #region Viewbags
        ViewBag.Title = "نقش جدید";
        ViewBag.Permissions = (await _permissionService.GetAllAsync()).ToList();
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="نقش‌ها", Title="نقش‌ها", Link=Url.Action("index", "role", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text="نقش جدید", Title="نقش جدید"},
        };
        #endregion
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Role.Create")]
    public async Task<IActionResult> Create(Role role, string permissions)
    {
        //var role = new Role { Name = name };
        if (string.IsNullOrEmpty(role.Name))
        {
            TempData["ErrorMessage"] = "نام نقش اجباری است";
            return View(role);
        }

        if (await _roleService.GetByNameAsync(role.Name) != null)
        {
            TempData["ErrorMessage"] = "نام نقش تکراری است.";
            return View(role);
        }

        var ids = JsonSerializer.Deserialize<List<int>>(permissions);
        var result = await _roleService.AddAsync(role, ids);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "نقش جدید با موفقیت ایجاد شد";
            return RedirectToAction("Index");
        }

        AddModelError(result.Errors);
        TempData["ErrorMessage"] = "افزودن نقش با خطا مواجه شد";

        #region Viewbags
        ViewBag.Title = "نقش‌ها";
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="نقش‌ها", Title="نقش‌ها", Link=Url.Action("index", "role", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text="نقش جدید", Title="نقش جدید"},
        };
        #endregion

        return View(role);
    }
    #endregion

    #region Edit
    [Permission("Role.Update")]
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null) return NotFound();

        var role = await _roleService.GetByIdAsync(id);
        if (role.Name.ToLower() == "admin" || role.Name.ToLower() == "subscriber")
            RedirectToAction(nameof(Index));

        if (role == null) return NotFound();

        #region Viewbags
        ViewBag.Title = "نقش‌ها";
        ViewBag.RoleClaims = await _roleService.GetRoleClaims(role);
        ViewBag.Permissions = (await _permissionService.GetAllAsync()).ToList();
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "نقش جدید",
            Link = Url.Action("create", "role", new { area = "admin" }, Request.Scheme),
            Title = "نقش جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="نقش‌ها", Title="نقش‌ها", Link=Url.Action("index", "role", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text=$"نقش {role.Name}", Title="نقش"},
        };
        #endregion

        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Role.Update")]
    public async Task<IActionResult> Edit(Role model, string permissions)
    {
        if (!(await RoleExists(model.Id))) return NotFound();

        var ids = JsonSerializer.Deserialize<List<int>>(permissions);
        var result = await _roleService.UpdateAsync(model, ids);

        if (result.Succeeded)
            return RedirectToAction("Index");

        AddModelError(result.Errors);

        #region Viewbags
        ViewBag.Title = "نقش‌ها";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "نقش جدید",
            Link = Url.Action("create", "role", new { area = "admin" }, Request.Scheme),
            Title = "نقش جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="نقش‌ها", Title="نقش‌ها", Link=Url.Action("index", "role", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text=$"نقش {model.Name}", Title="نقش"},
        };
        #endregion

        return View(model);
    }
    #endregion

    #region Delete
    // POST: Admin/Role/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Role.Delete")]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return new ObjectResult(new { result = false });
        if (await RoleExists(id))
        {
            await _roleService.DeleteAsync(id);
            return new ObjectResult(new { result = true });
        }
        return Json(new { result = false });
    }

    private async Task<bool> RoleExists(string id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null) return false;
        return true;
    }
    #endregion
}
