using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Core.Domain.Contracts.Services;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Admin.Models.ViewModels;
using MVC.Areas.Admin.Models.ViewModels.Options;
using Utilities.Languages;


namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OptionsController : Controller
{
    private readonly IOptionService _optionService;
    private readonly ITextProcessingService _textProcessingService;
    private readonly ICategoryService _categoryService;
    private readonly IPostService _postService;
    private readonly IPageService _pageService;
    private readonly IAttachmentService _attachmentService;
    protected readonly IMemoryCache _memorycache;

    public OptionsController(IOptionService optionService, ITextProcessingService textProcessingService, ICategoryService categoryService, IPostService postService, IPageService pageService, IAttachmentService attachmentService, IMemoryCache memoryCache)
    {
        _optionService = optionService;
        _textProcessingService = textProcessingService;
        _categoryService = categoryService;
        _postService = postService;
        _pageService = pageService;
        _attachmentService = attachmentService;
        _memorycache = memoryCache;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        GeneralVM model = new GeneralVM();
        var dict = new Dictionary<string, string>();
        var list = new List<string> { "SiteName", "SiteTitle", "SiteDescription", "AllowRegistration", "MobileRegisteration", "SiteFavIcon", "SiteLogo", "SendEmailConfirmation", "SendPhoneConfirmation", "HomePage", "CryptoApiToken", "IsMultilingual", "Languages", "DefaultLanguage" };
        //var list = typeof(GeneralVM).GetProperties().Select(p => p.Name).ToList();
        var options = await _optionService.prepareByNames(list);

        var props = typeof(GeneralVM).GetProperties();

        foreach (var option in options)
        {
            var prop = props.FirstOrDefault(p => p.Name == option.OptionName && p.CanWrite);
            if (prop != null)
            {
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(model, option.OptionValue);
                else if (prop.PropertyType == typeof(bool) && Boolean.TryParse(option.OptionValue, out bool myBool))
                    prop.SetValue(model, myBool);
                else if (prop.PropertyType == typeof(IFormFile))
                {
                    var p = prop.Name;
                    props.First(x => x.Name.ToLower() == $"{prop.Name}Path".ToLower()).SetValue(model, option.OptionValue);
                }
            }
        }
        model.Languages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(options.FirstOrDefault(x => x.OptionName == "Languages").OptionValue);

        ViewData["Languages"] = LanguagesUtil.GetLanguages();
        ViewData["DefaultLanguage"] = await _optionService.GetByName("DefaultLanguage");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(GeneralVM model)
    {
        var options2 = await _optionService.GetAllAsync();
        var options = new List<Option>();
        var props = typeof(GeneralVM).GetProperties();
        foreach (var prop in props)
        {
            // فقط پراپرتی‌هایی که مقدارشون قابل تبدیل به رشته باشه رو در نظر بگیر
            if (prop.PropertyType == typeof(string))
            {
                var name = prop.Name;
                var value = prop.GetValue(model)?.ToString();

                if (!string.IsNullOrEmpty(value))
                {
                    options.Add(new Option
                    {
                        OptionName = name,
                        OptionValue = value
                    });
                }
            }
            else
            {
                string name = "";
                switch (prop.PropertyType.Name.ToLower())
                {
                    case "boolean":
                        name = prop.Name;
                        var boolVal = prop.GetValue(model)?.ToString().ToLower();
                        options.Add(new Option
                        {
                            OptionName = name,
                            OptionValue = boolVal
                        });
                        break;

                    case "iformfile":
                        name = prop.Name;
                        var iFormVal2 = prop.GetValue(model);
                        if (iFormVal2 == null) iFormVal2 = Request.Form.Files[name];
                        var upload = await _attachmentService.UploadFile(iFormVal2 as IFormFile, Request.Scheme, Request.Host);
                        var json = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(upload));
                        if (json["status"] == "success")
                            options.Add(new Option { OptionName = name, OptionValue = json["path"] });
                        break;

                    case "list`1":
                        name = prop.Name;
                        var listVal = prop.GetValue(model);
                        var str = Newtonsoft.Json.JsonConvert.SerializeObject(listVal);
                        options.Add(new Option
                        {
                            OptionName = name,
                            OptionValue = str
                        });
                        break;

                    default:
                        break;
                }
            }
        }
        try
        {
            await _optionService.UpdateAsync(options);
            TempData["SuccessMessage"] = "بروزرسانی انجام شد";

            _memorycache.Remove("defaultLanguage");
            _memorycache.Remove("allLanguages");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "بروزرسانی با خطا مواجه شد";
            return View(model);
        }
    }

    public async Task<IActionResult> Optimize()
    {
        var list = new List<string> { "SeoKeywords", "SeoOgLocale", "SeoOgType", "SeoOgTitle", "SeoOgUrl", "SeoOgSiteName" };
        return View();
    }

    public async Task<IActionResult> Shop()
    {
        return View();
    }


    [HttpPost("/generateslug")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SlugGenerator(string type, string title, int id = 0, string slug = "")
    {
        // TODO: Update function for edit Slug (Edit methods)
        if (string.IsNullOrEmpty(title)) return null;
        if (title.Length < 3) return null; //return Json(new { slug = "" });
        title = _textProcessingService.SanitizeHtml(title);

        if (id != 0)
        {

        }
        else
        {
            if (!(string.IsNullOrEmpty(slug))) slug = _textProcessingService.SanitizeHtml(slug);

            if (type != "tax" && type != "post") return null;
            int num = 0;
            bool key = false;
            while (!key)
            {
                if (string.IsNullOrEmpty(slug)) slug = _textProcessingService.GenerateSlug(title);
                else slug = _textProcessingService.GenerateSlug(slug);
                if (num != 0)
                {
                    if (slug.Length == 60) slug = $"{slug.Substring(0, slug.Length - 2)}-{num}";
                    else if (slug.Length == 59) slug = $"{slug.Substring(0, slug.Length - 1)}-{num}";
                    else if (slug.Length <= 58) slug = $"{slug}-{num}";
                }
                key = await CheckSlug(slug, type);
                num++;
            }
        }
        return Json(new { slug = slug });
    }

    private async Task<bool> CheckSlug(string slug, string type)
    {
        bool result = false;
        switch (type)
        {
            case "tax":
                result = await _categoryService.IsExistBySlug(slug);
                break;

            case "post":
            case "page":
                var post = await _postService.IsExistBySlug(slug);
                var page = await _pageService.IsExistBySlug(slug);
                if (post || page) result = true;
                else result = false;
                break;

            case "attach":
                result = await _categoryService.IsExistBySlug(slug);
                break;

            default:
                break;
        }
        return !result;
    }

    private async Task<bool> GetSlug(string slug, string type)
    {
        bool result = false;
        switch (type)
        {
            case "tax":
                result = await _categoryService.IsExistBySlug(slug);
                break;

            case "post":
            case "page":
                var post = await _postService.IsExistBySlug(slug);
                var page = await _pageService.IsExistBySlug(slug);
                if (post || page) result = true;
                else result = false;
                break;

            case "attach":
                result = await _categoryService.IsExistBySlug(slug);
                break;

            default:
                break;
        }
        return result;
    }
}
