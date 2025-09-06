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
using MVC.Areas.Admin.Helpers.Attributes;
using Core.Domain.Contracts.Services;
using MVC.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Core.Domain.Contracts.Services.Identity;
using MVC.Models.ViewModels;
using Core.Services;
using System.Threading;
using MVC.Areas.Admin.Models.ViewModels;
using Utilities.User;
using Ganss.Xss;
using Microsoft.Extensions.Hosting;
using Core.Domain.Dtos.Blog;
using Utilities.Languages;
using Core.Domain.Entities.Shop;
using Utilities.HtmlSanitizer;
using AutoMapper;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class PageController : BaseController
{
    #region Injection & Construction
    private readonly IPageService _pageService;
    private readonly IPageMetaService _pageMetaService;
    private readonly ITextProcessingService _textProcessingService;
    private readonly IMapper _mapper;
    public PageController(IUserService userService, IMemoryCache memoryCache, IOptionService optionService, IPageService pageService, IPageMetaService pageMetaService, ITextProcessingService textProcessingService, IMapper mapper) : base(userService, optionService, memoryCache)
    {
        _pageService = pageService;
        _pageMetaService = pageMetaService;
        _textProcessingService = textProcessingService;
        _mapper = mapper;
    }
    #endregion


    #region View
    // GET: Admin/Page
    [Permission("Page.Read")]
    public async Task<IActionResult> Index()
    {
        string defaultLanguage = await base.GetDefaultLanguage();
        var dto = await _pageService.GetAllAsync(defaultLanguage);
        var pages = _mapper.Map<List<PageDto>, List<PageVM>>(dto);

        #region Viewbags
        ViewBag.Title = "برگه‌ها";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "برگه جدید",
            Link = Url.Action("create", "page", new { area = "admin" }, Request.Scheme),
            Title = "برگه جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="برگه‌ها", Title="برگه‌ها"},
        };
        ViewData["DefaultLanguage"] = defaultLanguage;
        var langs = await base.GetLanguages();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        return View(pages);
    }
    #endregion


    #region Create
    [HttpGet] // GET: Admin/Page/Create
    [Permission("Page.Create")]
    public async Task<IActionResult> Create()
    {
        #region Viewbags
        #region Original(Main) Pages List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var originalPages = await _pageService.GetOriginalPages(defaultLang);
        List<SelectListItem> originalsListItems = new List<SelectListItem>();
        originalsListItems.AddRange(originalPages.Select(c => new SelectListItem
        {
            Value = c.Id.Value.ToString(),
            Text = c.Title,
        }));
        ViewData["OriginalIds"] = originalsListItems;
        #endregion

        #region Languages
        var langs = await base.GetLanguages();
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        ViewBag.Title = "برگه جدید";
        ViewBag.Title = "برگه‌ها";
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="برگه‌ها", Title="برگه‌ها"},
        };
        #endregion

        return View();
    }

    // POST: Admin/Page/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Page.Create")]
    public async Task<IActionResult> Create(PageVM model)
    {
        model.UserId = User.GetUserId();
        model.Title = TextSanitizer.Sanitize(model.Title);
        model.Content = _textProcessingService.SanitizeHtml(model.Content);
        model.Slug = TextSanitizer.Sanitize(model.Slug);
        model.CreationTime = DateTime.Now;
        ModelState.Clear();
        TryValidateModel(model);

        if (ModelState.IsValid)
        {
            PageDto pageDto = _mapper.Map<PageVM, PageDto>(model);
            var result = await _pageService.AddAsync(pageDto);
            if (result != null)
            {
                #region Add/Update Translations meta
                if (model.OriginalPage != null)
                {
                    var parentProduct = await _pageService.GetById(model.OriginalPage.Value);
                    var res = await _pageMetaService.UpdateTranslationMeta(result, parentProduct);
                    if (!res) TempData["ErrorMessage"] = "محصول جدید ایجاد شد \n ثبت ترجمه محصول ناموفق بود";
                }
                else // Current page is original item
                {
                    var res = await _pageMetaService.AddTranslationMeta(result);
                    if (!res) TempData["ErrorMessage"] = "نوشته جدید ایجاد شد \n ثبت ترجمه نوشته ناموفق بود";
                }
                #endregion 

                TempData["SuccessMessage"] = "برگه جدید ایجاد شد";
                return RedirectToAction(nameof(Index));
            }
        }

        #region Original(Main) Pages List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var originalPages = await _pageService.GetOriginalPages(defaultLang);
        List<SelectListItem> originalsListItems = new List<SelectListItem>();
        originalsListItems.AddRange(originalPages.Select(c => new SelectListItem
        {
            Value = c.Id.Value.ToString(),
            Text = c.Title,
        }));
        ViewData["OriginalIds"] = originalsListItems;
        #endregion

        #region Languages
        var langs = await base.GetLanguages();
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        ViewBag.Title = "برگه جدید";
        TempData["ErrorMessage"] = "اطلاعات وارد شده صحبح نیست";
        return View(model);
    }
    #endregion


    #region Edit
    [HttpGet] // GET: Admin/Page/Edit/5
    [Permission("Page.Update")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var page = await _pageService.GetById(id.Value);
        if (page == null) return NotFound();

        string defaultLanguage = await base.GetDefaultLanguage();
        var translations = new Dictionary<string, int>();
        if (page.Translations != null) translations = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(page.Translations);

        PageVM result = _mapper.Map<PageVM>(page);
        result.OriginalPage = translations.Any(x => x.Key == defaultLanguage) ? translations.FirstOrDefault(x => x.Key == defaultLanguage).Value : null;

        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var allLangs = await _pageService.GetOriginalPages(defaultLang);
        List<SelectListItem> originalsListItems = new List<SelectListItem>();
        originalsListItems.AddRange(allLangs.Where(x => x.Id != page.Id).Select(c => new SelectListItem
        {
            Value = c.Id.Value.ToString(),
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

    [HttpPost] // POST: Admin/Page/Edit/5
    [ValidateAntiForgeryToken]
    [Permission("Page.Update")]
    public async Task<IActionResult> Edit(int id, PageVM model) // TODO: PageDto => PageVM
    {
        if (id == 0) return NotFound();
        var isExist = await _pageService.IsExistById(id);
        if (isExist == null) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                model.Title = TextSanitizer.Sanitize(model.Title);
                model.Content = _textProcessingService.SanitizeHtml(model.Content);
                model.Slug = TextSanitizer.Sanitize(model.Slug);
                model.UserId = User.GetUserId();
                model.LastModified = DateTime.Now;

                PageDto pageDto = _mapper.Map<PageDto>(model);

                var result = await _pageService.UpdateAsync(pageDto);
                if (result != null && model.OriginalPage != null)
                {
                    var parentProduct = await _pageService.GetById(model.OriginalPage.Value);
                    var res = await _pageMetaService.UpdateTranslationMeta(result, parentProduct);
                    if (!res) TempData["ErrorMessage"] = "محصول جدید ایجاد شد \n ثبت ترجمه محصول ناموفق بود";
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!(await _pageService.IsExistById(id))) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(await _userService.GetAll(default), "Id", "Id", model.UserId);
        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var orgProducts = await _pageService.GetOriginalPages(defaultLang);
        orgProducts.Remove(orgProducts.Find(x => x.Id == id));
        var originalProducts = orgProducts;
        List<SelectListItem> originalsListItems = new List<SelectListItem>();
        originalsListItems.AddRange(originalProducts.Select(c => new SelectListItem
        {
            Value = c.Id.Value.ToString(),
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
    #endregion


    #region Delete
    // GET: Admin/Page/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Page.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _pageService.IsExistById(id))
        {
            await _pageService.DeleteAsync(id);
            return Json(new { result = true });
        }
        return Json(new { result = false });
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
