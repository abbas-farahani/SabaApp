using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Utilities.Languages;

namespace MVC.Areas.Admin.ViewComponents;

public class AdminCurrentUserTopbar : ViewComponent
{
    private readonly IUserService _userService;
    private readonly IOptionService _optionService;
    protected readonly IMemoryCache _memorycache;
    public AdminCurrentUserTopbar(IUserService userService, IMemoryCache memorycache, IOptionService optionService)
    {
        _userService = userService;
        _memorycache = memorycache;
        _optionService = optionService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string model)
    {
        if (!_memorycache.TryGetValue($"currentUser_{User.Identity.Name}", out User user))
        {
            user = await _userService.GetByUserName(User.Identity.Name);
            _memorycache.Set($"currentUser_{User.Identity.Name}", user);
        }

        var allLanguages = LanguagesUtil.GetLanguages();
        var strLangs = await _optionService.GetByName("Languages");
        var langs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(strLangs);
        if (langs != null && langs.Count() > 0)
        {
            ViewData["Languages"] = allLanguages.Where(x => langs.Contains(x.Item3)).ToList();
            ViewData["DefaultLanguage"] = await _optionService.GetByName("DefaultLanguage");
        }

        return View("AdminCurrentUserTopbar", user);
    }
}
