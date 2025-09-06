using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVC.Models.ViewModels;
using Newtonsoft.Json;

namespace MVC.Controllers;

public class BaseController : Controller
{
    protected readonly IUserService _userService;
    protected readonly IMemoryCache _memorycache;
    protected readonly IOptionService _optionService;

    public BaseController(IUserService userService, IOptionService optionService, IMemoryCache memoryCache)
    {
        _userService = userService;
        _memorycache = memoryCache;
        _optionService = optionService;
    }
    public void AddModelError(IEnumerable<IdentityError> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    public async Task<User> GetCurrentUser(string username)
    {
        if (!_memorycache.TryGetValue($"currentUser_{username}", out User user))
            user = await _userService.GetByUserName(username);
        return user;
    }

    public async Task<string> GetDefaultLanguage()
    {
        if (!_memorycache.TryGetValue("defaultLanguage", out string lang))
        {
            lang = await _optionService.GetByName("DefaultLanguage");
            _memorycache.Set("defaultLanguage", lang);
        }
        return lang;
    }

    public async Task<List<string>> GetLanguages()
    {
        if (!_memorycache.TryGetValue($"allLanguages", out List<string> langs))
        {
            string option = await _optionService.GetByName("Languages");
            langs = JsonConvert.DeserializeObject<List<string>>(option);
            _memorycache.Set("allLanguages", langs);
        }
        return langs;
    }

    public string GetCurrentCultureName()
    {
        return Thread.CurrentThread.CurrentCulture.Name;
    }
}
