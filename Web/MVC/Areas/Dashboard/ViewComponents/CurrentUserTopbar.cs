using Core.Domain.Contracts.Services.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace MVC.Areas.Dashboard.ViewComponents;

public class CurrentUserTopbar : ViewComponent
{
    private readonly IUserService _userService;
    protected readonly IMemoryCache _memoryCache;
    public CurrentUserTopbar(IUserService userService, IMemoryCache memoryCache)
    {
        _userService = userService;
        _memoryCache = memoryCache;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_memoryCache.TryGetValue($"currentUser_{User.Identity.Name}", out User user))
            user = await _userService.GetByUserName(User.Identity.Name);
        ViewBag.Claims = (await _userService.GetClaims(user)).ToList();
        return View("CurrentUserTopbar", user);
    }
}
