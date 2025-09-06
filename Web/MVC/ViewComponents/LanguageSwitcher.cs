using System;
using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Utilities.Languages;

namespace MVC.ViewComponents;

public class LanguageSwitcher : ViewComponent
{
    private readonly IUserService _userService;
    private readonly IMemoryCache _memoryCache;
    private readonly IOptionService _optionService;
    public LanguageSwitcher(IUserService userService, IMemoryCache memoryCache, IOptionService optionService)
    {
        _userService = userService;
        _memoryCache = memoryCache;
        _optionService = optionService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string option = await _optionService.GetByName("Languages");
        if (string.IsNullOrEmpty(option) || option.ToLower() == "null")
        {
            return View("NotFound"); // Website is not Multilingual
        }
        else
        {
            if (!_memoryCache.TryGetValue($"allLanguages", out List<string> langs))
            {
                langs = JsonConvert.DeserializeObject<List<string>>(option);
                if (langs == null || langs.Count() == 0) return View("LanguageSwitcher");
                _memoryCache.Set("allLanguages", langs);
            }
            var languages = LanguagesUtil.GetLanguages(langs);
            var currentLanguage = Thread.CurrentThread.CurrentCulture.ToString();
            ViewBag.CurrentLanguage = currentLanguage;
            return View("LanguageSwitcher", languages);
        }
    }
}
