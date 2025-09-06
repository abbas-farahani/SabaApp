using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVC.Models.ViewModels.User;

namespace MVC.Areas.Dashboard.ViewComponents;

public class UserProfileNavbarVC : ViewComponent
{
    private readonly IUserService _userService;
    protected readonly IMemoryCache _memoryCache;
    public UserProfileNavbarVC(IUserService userService, IMemoryCache memoryCache)
    {
        _userService = userService;
        _memoryCache = memoryCache;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_memoryCache.TryGetValue($"currentUser_{User.Identity.Name}", out User user))
            user = await _userService.GetByUserName(User.Identity.Name);
        var claims = await _userService.GetClaims(user);
        var result = new EditUserVM
        {
            UserName = user.UserName,
            Phone = user.PhoneNumber,
            Email = user.Email,
            FirstName = claims.Any(x => x.Type == "FirstName") ? claims.FirstOrDefault(x => x.Type == "FirstName").Value : "",
            LastName = claims.Any(x => x.Type == "LastName") ? claims.FirstOrDefault(x => x.Type == "LastName").Value : "",
            AvatarPath = claims.Any(x => x.Type == "Avatar") ? claims.FirstOrDefault(x => x.Type == "Avatar").Value : "",
            BirthDate = claims.Any(x => x.Type == "BirthDate") ? DateTime.Parse(claims.FirstOrDefault(x => x.Type == "BirthDate").Value) : null,
        };
        return View("UserProfileNavbar", result);
    }
}
