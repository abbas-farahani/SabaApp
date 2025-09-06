using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Contracts.Services.Shop;
using Core.Domain.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVC.Controllers;
using Microsoft.AspNetCore.Authorization;
using MVC.Areas.Admin.Helpers.Attributes;
using Core.Domain.Entities.Shop;
using Core.Services.Shop;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Models.ViewModels;
using Utilities.User;
using Core.Domain.Dtos.Shop;
using Newtonsoft.Json;
using Utilities.Languages;
using Core.Domain.Entities;
using System;
using Core.Services;

namespace MVC.Areas.Admin.Controllers.Shop;

[Area("Admin")]
[Authorize]
public class ProductsController : BaseController
{
    #region Injection & Construction
    private readonly IProductService _productService;
    private readonly IProductCatService _productCatService;
    private readonly ITextProcessingService _textProcessingService;
    private readonly IProductMetaService _productMetaService;

    public ProductsController(IProductService productService, IProductCatService productCatService, ITextProcessingService textProcessingService, IUserService userService, IMemoryCache memoryCache, IOptionService optionService, IProductMetaService productMetaService) : base(userService, optionService, memoryCache)
    {
        _productService = productService;
        _productCatService = productCatService;
        _textProcessingService = textProcessingService;
        _productMetaService = productMetaService;

    }
    #endregion

    #region View
    [Permission("Product.Read")]
    public async Task<IActionResult> Index()
    {
        string defaultLanguage = await base.GetDefaultLanguage();
        var products = await _productService.GetAllAsync(defaultLanguage);

        #region Viewbags
        ViewBag.Title = "محصولات";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "محصول جدید",
            Link = Url.Action("create", "products", new { area = "admin" }, Request.Scheme),
            Title = "محصول جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="محصولات", Title="محصولات"},
        };
        ViewData["DefaultLanguage"] = await base.GetDefaultLanguage();
        var langs = await base.GetLanguages();
        ViewData["Languages"] = LanguagesUtil.GetLanguages(langs);
        #endregion
        return View(products);
    }
    #endregion

    #region Create
    // GET: Admin/Category/Create
    [Permission("Product.Create")]
    public async Task<IActionResult> Create()
    {
        #region Categories List - Parent Id
        var categories = await _productCatService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>();
        selectListItems.AddRange(categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
        }));
        ViewData["CategoriesId"] = selectListItems;
        #endregion

        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var originalProducts = await _productService.GetOriginalProducts(defaultLang);
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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Product.Create")]
    public async Task<IActionResult> Create(ProductDto model)
    {
        var user = await this.GetCurrentUser(User.Identity.Name);
        if (user == null) return BadRequest();

        model.product.UserId = User.GetUserId();
        model.product.Description = string.IsNullOrEmpty(model.product.Description) ? "" : model.product.Description;
        model.product.Content = string.IsNullOrEmpty(model.product.Content) ? "" : model.product.Content;
        TryValidateModel(model);
        ModelState.Remove("product.Reviews");
        ModelState.Remove("product.ProductMetas");
        ModelState.Remove("product.ProductCats");
        ModelState.Remove("product.User");
        ModelState.Remove("product.UserId");

        if (ModelState.IsValid)
        {
            //var lang = Request.Form["LanguagesList"]; //"zh-CN"
            var result = await _productService.AddAsync(model);
            if (result != null)
            {
                if (model.OriginalProduct != null)
                {
                    var parentProduct = await _productService.GetByIdAsync(model.OriginalProduct.Value);
                    var res = await _productMetaService.UpdateTranslationMeta(result, parentProduct);
                    if (!res) TempData["ErrorMessage"] = "محصول جدید ایجاد شد \n ثبت ترجمه محصول ناموفق بود";
                }
                else // Current product is original item
                {
                    var res = await _productMetaService.AddTranslationMeta(result);
                    if (!res) TempData["ErrorMessage"] = "نوشته جدید ایجاد شد \n ثبت ترجمه نوشته ناموفق بود";
                }
                TempData["SuccessMessage"] = "محصول جدید ایجاد شد";
                return RedirectToAction(nameof(Index));
            }
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

        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var originalProducts = await _productService.GetOriginalProducts(defaultLang);
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
        TempData["ErrorMessage"] = "محصول جدید ایجاد نشد";
        return View(model);
    }
    #endregion

    #region Edit
    [Permission("Product.Update")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _productService.GetByIdAsync(id.Value);
        if (product == null) return NotFound();

        var result = new ProductDto
        {
            product = product,
            CommentStatus = product.CommentStatus.ToString(),
            ThumbPath = product.FeaturedImage,
            ThumbName = "", //product.FeaturedImage.Split('/').LastOrDefault(),
            CategoriesId = product.ProductCats.Select(c => c.Id).ToList(),
            Status = (product.Status == 1) ? "published" : (product.Status == 3) ? "draft" : (product.Status == 4) ? "private" : (product.Status == 5) ? "scheduled" : "draft",
            SKU = product.ProductMetas.Any(x => x.MetaName == "SKU") ? product.ProductMetas.First(x => x.MetaName == "SKU").MetaValue : "",
            RegularPrice = product.ProductMetas.Any(x => x.MetaName == "RegularPrice") ? long.Parse(product.ProductMetas.First(x => x.MetaName == "RegularPrice").MetaValue) : 0,
            SalePrice = product.ProductMetas.Any(x => x.MetaName == "SalePrice") ? long.Parse(product.ProductMetas.First(x => x.MetaName == "SalePrice").MetaValue) : null,
            DiscountExpiration = product.ProductMetas.Any(x => x.MetaName == "DiscountExpiration") ? DateTime.Parse(product.ProductMetas.First(x => x.MetaName == "DiscountExpiration").MetaValue) : null,
            Quantity = product.ProductMetas.Any(x => x.MetaName == "Quantity") ? Int32.Parse(product.ProductMetas.First(x => x.MetaName == "Quantity").MetaValue) : null,
            ProductType = product.ProductMetas.Any(x => x.MetaName == "ProductType") ? product.ProductMetas.First(x => x.MetaName == "ProductType").MetaValue : "physical",
            SoldIndividually = product.ProductMetas.Any(x => x.MetaName == "SoldIndividually") && product.ProductMetas.First(x => x.MetaName == "SoldIndividually").MetaValue == "true" ? true : false,
            OriginalProduct = product.ProductMetas.Any(x => x.MetaName == "OriginalProduct") ? Int32.Parse(product.ProductMetas.First(x => x.MetaName == "OriginalProduct").MetaValue) : null,
            LanguagesList = product.ProductMetas.Any(x => x.MetaName == "LanguageCode") ? product.ProductMetas.First(x => x.MetaName == "LanguageCode").MetaValue : "",
        };

        #region Categories List - Parent Id
        var categories = await _productCatService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>();
        selectListItems.AddRange(categories.Where(x => x.Id != id.Value).Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
            Selected = (c.Id == id) ? true : false
        }));
        ViewData["CategoriesId"] = selectListItems;
        #endregion
        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var orgProducts = await _productService.GetOriginalProducts(defaultLang);
        orgProducts.Remove(product);
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
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Product.Update")]
    public async Task<IActionResult> Edit(int id, ProductDto model)
    {

        model.product.Id = id;
        model.product.UserId = User.GetUserId();
        model.product.Description = string.IsNullOrEmpty(model.product.Description) ? "" : model.product.Description;
        model.product.Content = string.IsNullOrEmpty(model.product.Content) ? "" : model.product.Content;
        ModelState.Clear();
        TryValidateModel(model);
        ModelState.Remove("product.Reviews");
        ModelState.Remove("product.ProductMetas");
        ModelState.Remove("product.ProductCats");
        ModelState.Remove("product.User");
        ModelState.Remove("product.UserId");

        if (ModelState.IsValid)
        {
            try
            {
                var result = await _productService.UpdateAsync(model);
                if (result != null)
                {
                    var parentProduct = await _productService.GetByIdAsync(model.OriginalProduct.Value);
                    var res = await _productMetaService.UpdateTranslationMeta(result, parentProduct);
                    if (!res) TempData["ErrorMessage"] = "محصول جدید ایجاد شد \n ثبت ترجمه محصول ناموفق بود";
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!(await ProductExists(id))) return NotFound();
                else TempData["ErrorMessage"] = "بروز رسانی محصول انجام نشد";
            }
            return RedirectToAction(nameof(Index));
        }
        #region Categories List - Parent Id
        var categories = await _productCatService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>();
        selectListItems.AddRange(categories.Where(x => x.Id != id).Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Label,
            Selected = (c.Id == id) ? true : false
        }));
        ViewData["CategoriesId"] = selectListItems;
        #endregion
        #region Original(Main) Products List (Default Lang)
        var defaultLang = await base.GetDefaultLanguage();
        var orgProducts = await _productService.GetOriginalProducts(defaultLang);
        var currentProduct = await _productService.GetByIdAsync(id);
        orgProducts.Remove(currentProduct);
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
        return View(model);
    }
    #endregion

    #region Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Product.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ProductExists(id))
        {
            await _productService.DeleteAsync(id);
            return Json(new { result = true });
        }
        return Json(new { result = false });
    }

    private async Task<bool> ProductExists(int id)
    {
        var cat = await _productService.GetByIdAsync(id);
        if (cat == null) return false;
        return true;
    }
    #endregion

}
