using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Core.Domain.Contracts.Services.Identity;
using Microsoft.Extensions.Caching.Memory;
using MVC.Controllers;
using MVC.Models.ViewModels;
using System.Data;
using Core.Domain.Contracts.Services;
using Core.Services;

namespace MVC.Areas.Admin.Controllers.Identity
{
    [Area("Admin")]
    public class PermissionController : BaseController
    {

        #region Injection & Construction
        private readonly IPermissionService _permissionService;
        private readonly ITextProcessingService _textProcessingService;
        public PermissionController(IPermissionService permissionService, IUserService userService, IMemoryCache memoryCache, IOptionService optionService, ITextProcessingService textProcessingService) : base(userService, optionService, memoryCache)
        {
            _permissionService = permissionService;
            _textProcessingService = textProcessingService;
        }
        #endregion

        #region View
        public async Task<IActionResult> Index()
        {
            #region Viewbags
            ViewBag.Title = "دسترسی‌ها";
            ViewBag.CreateBtn = new CustomBtn
            {
                Text = "دسترسی جدید",
                Link = Url.Action("create", "permission", new { area = "admin" }, Request.Scheme),
                Title = "دسترسی جدید",
                Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
            };
            ViewBag.Breadcrumb = new List<AnchorLink>
            {
                new AnchorLink{Text="همه دسترسی‌ها", Title="همه دسترسی‌ها"},
            };
            #endregion
            return View(await _permissionService.GetAllAsync());
        }

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var permission = await _permissionService.GetByIdAsync(id.Value);
        //    if (permission == null) return NotFound();

        //    #region Viewbags
        //    ViewBag.Title = $"دسترسی {permission.Name}";
        //    ViewBag.CreateBtn = new CustomBtn
        //    {
        //        Text = "ویرایش دسترسی",
        //        Link = Url.Action("edit", "permission", new { area = "admin", id = id }, Request.Scheme),
        //        Title = "ویرایش دسترسی",
        //        Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        //    };
        //    ViewBag.Breadcrumb = new List<AnchorLink>
        //    {
        //        new AnchorLink{Text="دسترسی‌ها", Title="دسترسی‌ها", Link=Url.Action("index", "permission", new { area = "admin" }, Request.Scheme)},
        //        new AnchorLink{Text=$"دسترسی {permission.Name}", Title="دسترسی"},
        //    };
        //    #endregion
        //    return View(permission);
        //}
        #endregion


        #region Create
        public IActionResult Create()
        {
            #region Viewbags
            ViewBag.Title = "دسترسی‌ها";
            ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="دسترسی‌ها", Title="دسترسی‌ها", Link=Url.Action("index", "role", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text="دسترسی جدید", Title="دسترسی جدید"},
        };
            #endregion
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddPermissionVM model)
        {
            try
            {
                await _permissionService.AddAsync(new Permission
                {
                    Name = _textProcessingService.SanitizeHtml(model.Name),
                    Description = _textProcessingService.SanitizeHtml(model.Description),
                    CreationTime = DateTime.Now,
                    LastModified = null,
                });
                TempData["SuccessMessage"] = "دسترسی جدید ایجاد شد";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "امکان ایجاد دسترسی وجود ندارد";
                return View(model);
            }

            //var list = new List<string> { "Create", "Read", "Update", "Delete" };
            //if (list.Contains(model.Name))
            //{
            //    TempData["ErrorMessage"] = "نام انتخاب شده مجاز نیست";
            //    return View(model);
            //}

            //model.Name = _textProcessingService.SanitizeHtml(model.Name);
            //model.Description = _textProcessingService.SanitizeHtml(model.Description);
            //if (ModelState.IsValid)
            //{
            //    #region Create
            //    if (model.Create)
            //        await _permissionService.AddAsync(new Permission
            //        {
            //            Name = $"{model.Name}.Create",
            //            Description = model.Description,
            //        });
            //    #endregion

            //    #region Read
            //    if (model.Read)
            //        await _permissionService.AddAsync(new Permission
            //        {
            //            Name = $"{model.Name}.Read",
            //            Description = model.Description,
            //        });
            //    #endregion

            //    #region Update
            //    if (model.Update)
            //        await _permissionService.AddAsync(new Permission
            //        {
            //            Name = $"{model.Name}.Update",
            //            Description = model.Description,
            //        });
            //    #endregion

            //    #region Delete
            //    if (model.Delete)
            //        await _permissionService.AddAsync(new Permission
            //        {
            //            Name = $"{model.Name}.Delete",
            //            Description = model.Description,
            //        });
            //    #endregion

            //    return RedirectToAction(nameof(Index));
            //}

            //#region Viewbags
            //ViewBag.Title = "دسترسی‌ها";
            //ViewBag.Breadcrumb = new List<AnchorLink>
            //{
            //    new AnchorLink{Text="دسترسی‌ها", Title="دسترسی‌ها", Link=Url.Action("index", "role", new { area = "admin" }, Request.Scheme)},
            //    new AnchorLink{Text="دسترسی جدید", Title="دسترسی جدید"},
            //};
            //#endregion
            //return View(model);
        }
        #endregion


        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var permission = await _permissionService.GetByIdAsync(id.Value);
            var result = new EditPermissionVM
            {
                Id = id.Value,
                Name = permission.Name,
                Description = permission.Description,
            };

            //var result = await _permissionService.GetAllById(id.Value);
            //if (result == null) return NotFound();


            //var permission = new EditPermissionVM
            //{
            //    Id = id.Value,
            //    Name = result.First().Name.Split('.')[0],
            //    Description = result.First().Description,
            //    Create = result.Any(x => x.Name.Split('.')[1] == "Create") ? true : false,
            //    CreateId = result.Any(x => x.Name.Split('.')[1] == "Create") ? result.First(x => x.Name.Split('.')[1] == "Create").Id : null,
            //    Read = result.Any(x => x.Name.Split('.')[1] == "Read") ? true : false,
            //    ReadId = result.Any(x => x.Name.Split('.')[1] == "Read") ? result.First(x => x.Name.Split('.')[1] == "Read").Id : null,
            //    Update = result.Any(x => x.Name.Split('.')[1] == "Update") ? true : false,
            //    UpdateId = result.Any(x => x.Name.Split('.')[1] == "Update") ? result.First(x => x.Name.Split('.')[1] == "Update").Id : null,
            //    Delete = result.Any(x => x.Name.Split('.')[1] == "Delete") ? true : false,
            //    DeleteId = result.Any(x => x.Name.Split('.')[1] == "Delete") ? result.First(x => x.Name.Split('.')[1] == "Delete").Id : null,
            //};

            #region Viewbags
            ViewBag.Title = "دسترسی‌ها";
            ViewBag.CreateBtn = new CustomBtn
            {
                Text = "دسترسی جدید",
                Link = Url.Action("create", "permission", new { area = "admin" }, Request.Scheme),
                Title = "دسترسی جدید",
                Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
            };
            ViewBag.Breadcrumb = new List<AnchorLink>
            {
                new AnchorLink{Text="دسترسی‌ها", Title="دسترسی‌ها", Link=Url.Action("index", "permission", new { area = "admin" }, Request.Scheme)},
                new AnchorLink{Text=$"دسترسی {result.Name}", Title="دسترسی"},
            };
            #endregion
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPermissionVM model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var permission = await _permissionService.GetByIdAsync(id);
                    permission.Name = _textProcessingService.SanitizeHtml(model.Name);
                    permission.Description = _textProcessingService.SanitizeHtml(model.Description);
                    permission.LastModified = DateTime.Now;
                    await _permissionService.UpdateAsync(permission);
                    TempData["SuccessMessage"] = "دسترسی بروز رسانی شد";
                    //#region Create
                    //if (model.Create)
                    //{
                    //    await _permissionService.UpdateAsync(model);
                    //}
                    //#endregion

                    //    #region Read
                    //if (model.Read)
                    //    await _permissionService.AddAsync(new Permission
                    //    {
                    //        Name = $"{model.Name}.Read",
                    //        Description = model.Description,
                    //    });
                    //#endregion

                    //#region Update
                    //if (model.Update)
                    //    await _permissionService.AddAsync(new Permission
                    //    {
                    //        Name = $"{model.Name}.Update",
                    //        Description = model.Description,
                    //    });
                    //#endregion

                    //#region Delete
                    //if (model.Delete)
                    //    await _permissionService.AddAsync(new Permission
                    //    {
                    //        Name = $"{model.Name}.Delete",
                    //        Description = model.Description,
                    //    });
                    //#endregion

                    //await _permissionService.UpdateAsync(permission);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await PermissionExists(id))) return NotFound();
                    else TempData["ErrorMessage"] = "بروز رسانی دسترسی انجام نشد";
                }
                return RedirectToAction(nameof(Index));
            }
            //return View(permission);

            #region Viewbags
            ViewBag.Title = "دسترسی‌ها";
            ViewBag.CreateBtn = new CustomBtn
            {
                Text = "دسترسی جدید",
                Link = Url.Action("create", "permission", new { area = "admin" }, Request.Scheme),
                Title = "دسترسی جدید",
                Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
            };
            ViewBag.Breadcrumb = new List<AnchorLink>
            {
                new AnchorLink{Text="دسترسی‌ها", Title="دسترسی‌ها", Link=Url.Action("index", "permission", new { area = "admin" }, Request.Scheme)},
                new AnchorLink{Text=$"دسترسی {model.Name}", Title="دسترسی"},
            };
            #endregion
            TempData["ErrorMessage"] = "مقادیر وارد شده معتبر نیست";
            return View(model);
        }
        #endregion


        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (await PermissionExists(id))
            {
                await _permissionService.DeleteAsync(id);
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }
        #endregion

        private async Task<bool> PermissionExists(int id)
        {
            var perm = await _permissionService.GetByIdAsync(id);
            if (perm != null) return true;
            return false;
        }
    }
}
