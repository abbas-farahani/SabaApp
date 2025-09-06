using AutoMapper;
using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Contracts.Services.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using MVC.Models;
using MVC.Areas.Admin.Models.ViewModels;
using MVC.Resources;
using System.Diagnostics;
using System.Globalization;
using Core.Domain.Dtos.Blog;

namespace MVC.Controllers;

public class HomeController : BaseController
{
    #region Injection & Construction
    private readonly ILogger<HomeController> _logger;
    private readonly IPageService _pageService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IAttachmentService _attachmentService;
    private readonly IPostService _postService;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IMapper _mapper;

    public HomeController(ILogger<HomeController> logger, IOptionService optionService, IPageService pageService, IUserService userService, IMemoryCache memoryCache, IProductService productService, ICategoryService categoryService, IAttachmentService attachmentService, IPostService postService, IMapper mapper) : base(userService, optionService, memoryCache)
    {
        _logger = logger;
        _pageService = pageService;
        _productService = productService;
        _categoryService = categoryService;
        _attachmentService = attachmentService;
        _postService = postService;
        _mapper = mapper;
    }
    #endregion

    public async Task<IActionResult> Index()
    {
        string currentCulture = base.GetCurrentCultureName(); //example: output: ar/en/fa-IR
        string option = await _optionService.GetByName("HomePage"); // output (string): 14 


        //if (!(string.IsNullOrEmpty(option)) && Int32.TryParse(option, out int id) && id > 0)
        //{
        //    var html = await _pageService.GetByIdAsync(id);
        //    var meta = html.PageMetas.FirstOrDefault(x => x.MetaName == "Translation");
        //    if (meta != null)
        //    {
        //        var translates = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(meta.MetaValue);
        //        var indexId = translates.FirstOrDefault(x => x.Key.StartsWith(currentCulture)).Value;
        //        if (indexId != id) html = await _pageService.GetByIdAsync(indexId);
        //        return View(html);
        //    }
        //}
        ViewBag.TopBar = false;
        ViewBag.BreadCrumbs = false;
        ViewBag.Products = await _productService.GetList(10, currentCulture);
        ViewBag.Articles = await _postService.GetList(10, currentCulture);

        if (currentCulture.StartsWith("ar")) return View("Index_ar-SA");
        else if (currentCulture.StartsWith("en")) return View("Index_en-US");
        else return View();
    }

    #region Articles
    [HttpGet("/blog")]
    public async Task<IActionResult> Blog()
    {
        string currentCulture = base.GetCurrentCultureName(); //example: output: ar/en/fa-IR
        var posts = await _postService.GetList(10, currentCulture);
        return View(posts);
    }

    [HttpGet("/article/{slug}")]
    public async Task<IActionResult> SinglePost(string slug, CancellationToken cancellationToken)
    {
        string currentCulture = base.GetCurrentCultureName(); //example: output: ar/en/fa-IR
        var post = await _postService.GetBySlug(slug, cancellationToken);
        if (post == null) return NotFound();
        var meta = post.PostMetas.FirstOrDefault(x => x.MetaName == "Translation");
        if (meta != null)
        {
            var translates = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(meta.MetaValue);
            var indexId = translates.FirstOrDefault(x => x.Key.StartsWith(currentCulture)).Value;
            if (indexId != post.Id) post = await _postService.GetByIdAsync(indexId);
        }
        return View(post);
    }
    #endregion


    #region Static Pages
    [HttpGet("{slug}")]
    public async Task<IActionResult> SinglePage(string slug)
    {
        if (string.IsNullOrEmpty(slug)) return NotFound();
        var dto = await _pageService.GetBySlug(slug);
        var page = _mapper.Map<PageDto, PageVM>(dto);
        if (page == null) return NotFound();

        if (page.LanguagesList != null)
        {
            string currentCulture = base.GetCurrentCultureName();
            if (!page.LanguagesList.StartsWith(currentCulture))
            {
                if (page.Translations != null)
                {
                    var translates = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(page.Translations);
                    var id = translates.FirstOrDefault(x => x.Key.StartsWith(currentCulture)).Value;
                    if (id != null)
                    {
                        dto = await _pageService.GetById(id);
                        page = _mapper.Map<PageDto, PageVM>(dto);
                        return RedirectToAction("SinglePage", new { slug = page.Slug });
                    }
                }
            }
        }

        return View(page);
    }
    #endregion


    #region Attachments
    //[HttpGet("/media/id={id}")]
    //public async Task<IActionResult> SingleMedia(int id)
    //{
    //    var attachment = await _attachmentService.GetByIdAsync(id);
    //    if (attachment == null) return NotFound();
    //    return View(attachment);
    //}

    [HttpGet("/media/{slug}")]
    public async Task<IActionResult> SingleMedia(string slug)
    {
        if (string.IsNullOrEmpty(slug)) return NotFound();

        var attachment = await _attachmentService.GetBySlug(slug);
        if (attachment == null) return NotFound();
        return View(attachment);
    }
    #endregion


    #region Category
    [HttpGet("/categories")]
    public async Task<IActionResult> Categories()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    [HttpGet("/category/{slug}")]
    public async Task<IActionResult> SingleCategory(string slug)
    {
        if (string.IsNullOrEmpty(slug)) return NotFound();

        var category = await _categoryService.GetBySlug(slug);
        if (category == null) return NotFound();
        return View(category);
    }
    #endregion


    public IActionResult ChangeLanguage(string lang)
    {
        if (!string.IsNullOrEmpty(lang))
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }
        else
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fa-IR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa-IR");
            lang = "fa-IR";
        }
        Response.Cookies.Append("Language", lang);
        var str = Request.GetTypedHeaders().Referer.ToString();
        var safeUrl = Uri.EscapeUriString(str);// to fix non-ascii characters
        return Redirect(safeUrl);
    }



    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("/accessdenied")]
    public IActionResult AccessDenied()
    {
        return View("AccessDenied");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
