using System.Security.Claims;
using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MVC.Controllers;
using MVC.Models.ViewModels;
using MVC.Models.ViewModels.User;

namespace MVC.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize]
public class SettingsController : BaseController
{
    #region Injection & Constructor
    public SettingsController(Core.Domain.Contracts.Services.Identity.IUserService userService, IMemoryCache memoryCache, IOptionService optionService) : base(userService, optionService, memoryCache)
    {
    }
    #endregion

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!_memorycache.TryGetValue($"currentUser_{User.Identity.Name}", out User user))
            user = await _userService.GetByUserName(User.Identity.Name);
        if (user == null) return NotFound();
        var claims = await _userService.GetClaims(user);

        var model = new EditUserVM
        {
            Id = user.Id,
            UserName = user.UserName,
            Phone = user.PhoneNumber,
            Email = user.Email,
            FirstName = claims.Any(c => c.Type == "FirstName") ? claims.FirstOrDefault(c => c.Type == "FirstName")?.Value : "",
            LastName = claims.Any(c => c.Type == "LastName") ? claims.FirstOrDefault(c => c.Type == "LastName")?.Value : "",
            Country = claims.Any(c => c.Type == "Country") ? claims.FirstOrDefault(c => c.Type == "Country")?.Value : "",
            Language = claims.Any(c => c.Type == "Language") ? claims.FirstOrDefault(c => c.Type == "Language")?.Value : "",
            BirthDate = claims.Any(c => c.Type == "BirthDate") ? DateTime.Parse(claims.FirstOrDefault(c => c.Type == "Language")?.Value) : null,
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(EditUserVM model)
    {
        if (model.UserName != User.Identity.Name) NotFound();

        var user = await _userService.GetByUserName(User.Identity.Name);

        if (user == null) return NotFound();

        if (ModelState.IsValid)
        {
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;

            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(model.FirstName)) claims.Add(new Claim("FirstName", model.FirstName));
            if (!string.IsNullOrEmpty(model.LastName)) claims.Add(new Claim("LastName", model.LastName));
            if (!string.IsNullOrEmpty(model.Country)) claims.Add(new Claim("Country", model.Country));
            if (!string.IsNullOrEmpty(model.Language)) claims.Add(new Claim("Language", model.Language));
            if (model.BirthDate != null) claims.Add(new Claim("BirthDate", model.BirthDate.ToString()));
            var update = await _userService.UpdateUserWithClaims(user, claims);
        }
        #region Viewbags
        ViewBag.Title = "تنظیمات";
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="تنظیمات", Title="تنظیمات"},
        };
        #endregion
        return View(model);
    }
}
