using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Core.Domain.Contracts.Services.Shop;
using Microsoft.AspNetCore.Authorization;
using Core.Domain.Entities;
using MVC.Models.ViewModels;
using Core.Domain.Entities.Shop;

namespace MVC.Controllers;

public class ShopController : BaseController
{
    private readonly IProductService _productService;
    private readonly IProductCatService _productCatService;
    public ShopController(IUserService userService, IMemoryCache memoryCache, IOptionService optionService, IProductService productService, IProductCatService productCatService) : base(userService, optionService, memoryCache)
    {
        _productService = productService;
        _productCatService = productCatService;
    }

    [HttpGet("/products")]
    public async Task<IActionResult> Index()
    {
        string currentCulture = base.GetCurrentCultureName(); //example: output: ar/en/fa-IR

        ViewBag.Title = "محصولات";
        ViewBag.TopBar = false;
        ViewBag.BreadCrumbs = false;
        ViewBag.Products = await _productService.GetList(10, currentCulture);

        if (currentCulture.StartsWith("ar")) return View("Index_ar-SA");
        else if (currentCulture.StartsWith("en")) return View("Index_en-US");
        else return View();
    }

    [HttpGet("/product/{slug}")]
    public async Task<IActionResult> SingleProduct(string slug)
    {

        if (string.IsNullOrEmpty(slug)) return NotFound();
        var product = await _productService.GetBySlug(slug);
        if (product == null) return NotFound();

        var productCulture = product.ProductMetas.FirstOrDefault(x => x.MetaName == "LanguageCode");
        if (productCulture != null)
        {
            string langCode = productCulture.MetaValue;
            string currentCulture = base.GetCurrentCultureName();
            if (langCode.StartsWith(currentCulture))
            {
                var translatesMeta = product.ProductMetas.FirstOrDefault(x => x.MetaName == "Translation");
                if (translatesMeta != null)
                {
                    var translates = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(translatesMeta.MetaValue);
                    var id = translates.FirstOrDefault(x => x.Key.StartsWith(currentCulture)).Value;
                    if (id != null)
                    {
                        if (id != product.Id) product = await _productService.GetByIdAsync(id);
                        return RedirectToAction("SingleProduct", new { slug = product.Slug });
                    }
                }
            }
        }

        #region ViewBags
        ViewBag.Title = product.Title;
        ViewBag.Breadcrumbs = true;
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="فروشگاه", Title="فروشگاه", Link=Url.Action("shop", "index", Request.Scheme)},
            new AnchorLink{Text=product.Title, Title=product.Title},
        };
        #endregion
        return View(product);
    }

    [HttpGet("/shop/categories")]
    public async Task<IActionResult> ShopCategories()
    {
        var cats = await _productCatService.GetAllAsync();
        ViewBag.Title = "دسته بندی محصولات";
        ViewBag.Breadcrumbs = true;
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="فروشگاه", Title="فروشگاه", Link=Url.Action("shop", "index", Request.Scheme)},
            new AnchorLink{Text="دسته بندی محصولات", Title="دسته بندی محصولات"},
        };
        return View(cats);
    }

    [HttpGet("/shop/category/{slug}")]
    public async Task<IActionResult> ShopCategory(string slug)
    {
        if (string.IsNullOrEmpty(slug)) return NotFound();
        var category = await _productCatService.GetBySlug(slug);
        if (category == null) return NotFound();

        ViewBag.Title = category.Label;
        ViewBag.Breadcrumbs = true;
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="فروشگاه", Title="فروشگاه", Link=Url.Action("shop", "index", Request.Scheme)},
            new AnchorLink{Text="دسته بندی محصولات", Title="دسته بندی محصولات", Link=Url.Action("shop", "index", Request.Scheme)},
            new AnchorLink{Text=category.Label, Title=category.Label},
        };
        return View(category);
    }


    [HttpGet("/cart")]
    public async Task<IActionResult> Cart()
    {
        var products = await _productService.GetAllAsync();
        if (products == null) return NotFound();
        ViewBag.Title = "سبدخرید";
        ViewBag.TopBar = false;
        return View();
    }

    [HttpGet("/checkout")]
    public async Task<IActionResult> Checkout()
    {
        return View();
    }
}
