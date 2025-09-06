using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Core.Domain.Contracts.Services;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using MVC.Models.ViewModels;
using Core.Domain.Contracts.Services.Identity;
using MVC.Areas.Admin.Models.ViewModels.Post;
using Utilities.User;
using MVC.Controllers;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Admin.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using Core.Domain.Dtos.Blog;
using Utilities.HtmlSanitizer;
using Utilities.Languages;
using Core.Domain.Entities.Shop;
using Microsoft.Extensions.Hosting;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class PostController : BaseController
{
    #region Injection & Construction
    private readonly IPostService _postService;
    private readonly IPostMetaService _postMetaService;
    private readonly ICategoryService _categoryService;
    private List<string> _metaKeys;
    public PostController(IPostService postService, ICategoryService categoryService, IUserService userService, IMemoryCache memoryCache, IOptionService optionService, IPostMetaService postMetaService) : base(userService, optionService, memoryCache)
    {
        _postService = postService;
        _postMetaService = postMetaService;
        _categoryService = categoryService;
        _metaKeys = new List<string>() { "ThumbPath" };
    }
    #endregion


    #region View
    [Permission("Post.Read")]
    public async Task<IActionResult> Index()
    {
        string defaultLanguage = await base.GetDefaultLanguage();
        var posts = await _postService.GetAllAsync(defaultLanguage);

        #region Viewbags
        ViewBag.Title = "نوشته‌ها";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "نوشته جدید",
            Link = Url.Action("create", "post", new { area = "admin" }, Request.Scheme),
            Title = "نوشته جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="نوشته‌ها", Title="نوشته‌ها"},
        };
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        var langs = await base.GetLanguages();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        return View(posts);
    }
    #endregion

    #region Create
    // GET: Admin/Posts/Create
    [Permission("Post.Create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new PostVM(); // {Important} create new object of PostVM because we have properties called "new" in model 
        ViewBag.Title = "نوشته جدید";

        #region Categories List - Parent Id
        var categories = await _categoryService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>
        {
            //new SelectListItem { Value = "", Text = "بدون والد", Selected = true } // Default Item
        };
        selectListItems.AddRange(categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
        }));
        ViewData["CategoriesId"] = selectListItems;
        #endregion

        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var originalProducts = await _postService.GetOriginalProducts(defaultLang);
        List<SelectListItem> originalsListItems = new List<SelectListItem>();
        originalsListItems.AddRange(originalProducts.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Title,
        }));
        ViewData["OriginalIds"] = originalsListItems;
        #endregion

        #region Languages
        var langs = await base.GetLanguages();
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion

        return View(model);
    }

    // POST: Admin/Posts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Post.Create")]
    public async Task<IActionResult> Create(PostVM model, CancellationToken cancellationToken)
    {
        var defaultLang = await base.GetDefaultLanguage();

        model.UserId = User.GetUserId();
        var post = new PostDto
        {
            CreationTime = DateTime.Now,
            UserId = model.UserId,
            Title = model.Title,
            Content = model.Content,
            Slug = model.Slug,
            CommentStatus = model.CommentStatus,
            Status = model.Status,
            ThumbPath = model.ThumbPath,
            CategoriesId = model.CategoriesId,
            LanguagesList = string.IsNullOrEmpty(model.LanguagesList) ? defaultLang : model.LanguagesList,
        };

        ModelState.Remove("UserId");
        model.UserId = post.UserId;
        TryValidateModel(model);

        if (ModelState.IsValid)
        {
            var newItem = await _postService.AddAsync(post);
            if (newItem != null)
            {
                if (model.OriginalArticle != null)
                {
                    var parentPost = await _postService.GetByIdAsync(model.OriginalArticle.Value);
                    var res = await _postMetaService.UpdateTranslationMeta(newItem, parentPost);
                    if (!res) TempData["ErrorMessage"] = "نوشته جدید ایجاد شد \n ثبت ترجمه نوشته ناموفق بود";
                }
                else
                {
                    var pos = await _postService.GetByIdAsync(newItem.Id);
                    var res = await _postMetaService.AddTranslationMeta(pos);
                    if (!res) TempData["ErrorMessage"] = "نوشته جدید ایجاد شد \n ثبت ترجمه نوشته ناموفق بود";
                }
                TempData["SuccessMessage"] = "نوشته جدید ایجاد شد";
                return RedirectToAction(nameof(Index));
            }

        }
        ViewBag.Title = "نوشته جدید";
        ViewData["CategoriesId"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "Label", model.Categories);
        #region Original(Main) posts List (Default Lang)
        var originalProducts = await _postService.GetOriginalProducts(defaultLang);
        List<SelectListItem> originalsListItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "بدون والد", Selected = true } // Default Item
        };
        originalsListItems.AddRange(originalProducts.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Title,
        }));
        ViewData["OriginalIds"] = originalsListItems;
        #endregion

        #region Languages
        var langs = await base.GetLanguages();
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        TempData["ErrorMessage"] = "اطلاعات وارد شده صحبح نیست";
        return View(model);
    }
    #endregion

    #region Edit
    // GET: Admin/Posts/Edit/5
    [Permission("Post.Update")]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id == null) return NotFound();

        PostDto post = await _postService.GetFullItemByIdAsync(id.Value);
        if (post == null) return NotFound();

        var result = new PostVM
        {
            Id = post.Id,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            Slug = post.Slug,
            CommentStatus = post.CommentStatus,
            Status = post.Status,
            CreationTime = post.CreationTime.Value,
            LastModified = post.LastModified,
            ThumbPath = post.ThumbPath,
            ThumbName = post.ThumbName,

            CategoriesId = post.Categories != null ? post.Categories.Select(x => x.Id).ToList() : new List<int>(),
        };
        ViewBag.Title = $"ویرایش نوشته {post.Title}";
        ViewData["CategoriesId"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "Label", result.CategoriesId);
        //ViewData["UserId"] = new SelectList(await _userService.GetAll(cancellationToken), "Id", "Id", post.UserId);
        #region Original(Main) posts List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var originalProducts = await _postService.GetOriginalProducts(defaultLang);
        List<SelectListItem> originalsListItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "بدون والد", Selected = true } // Default Item
        };
        originalsListItems.AddRange(originalProducts.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Title,
        }));
        ViewData["OriginalIds"] = originalsListItems;
        #endregion

        #region Languages
        var langs = await base.GetLanguages();
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        return View(result);
    }

    // POST: Admin/Posts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Post.Update")]
    public async Task<IActionResult> Edit(int id, PostVM model)
    {
        model.UserId = User.GetUserId();
        ModelState.Clear();
        TryValidateModel(model);

        if (ModelState.IsValid)
        {
            PostDto post = new PostDto
            {
                Id = id,
                UserId = model.UserId,
                Title = TextSanitizer.Sanitize(model.Title),
                Content = model.Content,
                Slug = TextSanitizer.Sanitize(model.Slug),
                CommentStatus = model.CommentStatus,
                Status = model.Status,
                ThumbPath = model.ThumbPath,
                CategoriesId = model.CategoriesId
            };

            try
            {
                var result = await _postService.UpdateAsync(post);
                if (result != null && model.OriginalArticle != null)
                {
                    var parentPost = await _postService.GetByIdAsync(model.OriginalArticle.Value);
                    var res = await _postMetaService.UpdateTranslationMeta(result, parentPost);
                    if (!res) TempData["ErrorMessage"] = "مقاله جدید ایجاد شد \n ثبت ترجمه مقاله ناموفق بود";
                }
                if (result != null)
                {
                    TempData["SuccessMessage"] = "نوشته بروزرسانی شد";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!(await PostExists(id))) return NotFound();
                else throw;
            }
        }
        ViewBag.Title = $"ویرایش نوشته {model.Title}";
        ViewData["CategoriesId"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "Label", model.CategoriesId);
        ViewData["UserId"] = new SelectList(await _userService.GetAll(default), "Id", "Id", model.UserId);
        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var orgProducts = await _postService.GetOriginalProducts(defaultLang);
        var curentPost = await _postService.GetByIdAsync(id);
        orgProducts.Remove(curentPost);
        var originalProducts = orgProducts;
        List<SelectListItem> originalsListItems = new List<SelectListItem>();
        originalsListItems.AddRange(originalProducts.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Title,
        }));
        ViewData["OriginalIds"] = originalsListItems;
        #endregion
        #region Languages
        var langs = await base.GetLanguages();
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        TempData["ErrorMessage"] = "بروزرسانی انجام نشد";
        return View(model);
    }
    #endregion

    #region Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Post.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await PostExists(id))
        {
            await _postService.DeleteAsync(id);
            return Json(new { result = true });
        }
        return Json(new { result = false });
    }

    private async Task<bool> PostExists(int id)
    {
        return await _postService.GetByIdAsync(id) != null ? true : false;
    }
    #endregion

    private byte GetStatusValue(string status)
    {
        if (status == "published") return 1;
        else if (status == "draft") return 3;
        else if (status == "private") return 4;
        else if (status == "scheduled") return 5;
        else return 3;
    }
}
