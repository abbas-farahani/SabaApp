using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Admin.Models.ViewModels;

namespace MVC.ViewComponents;

public class SliderViewComponent : ViewComponent
{
    private readonly IUserService _userService;
    private readonly IMemoryCache _memoryCache;
    private readonly IOptionService _optionService;
    public SliderViewComponent(IUserService userService, IMemoryCache memoryCache, IOptionService optionService)
    {
        _userService = userService;
        _memoryCache = memoryCache;
        _optionService = optionService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int sliderId)
    {
        var slider = await _optionService.GetSlider(sliderId);
        var result = new SliderVM
        {
            Id = slider.Id,
            Name = slider.Name,
            Title = slider.Title,
            CreationTime = slider.CreationTime,
            LastModified = slider.LastModified,
            Slides = slider.Slides.Select(x => new SlideVM
            {
                Id = x.Id,
                SliderId = slider.Id,
                SliderName = slider.Title,
                Name = x.Name,
                Title = x.Title,
                Description = x.Description,
                CreationTime = x.CreationTime,
                LastModified = x.LastModified,
                Url = x.Url,
                BackgroundType = x.BackgroundType,
                BackgroundValue = x.BackgroundValue
            }).ToList()
        };
        if (result != null && result.Slides.Count() > 0)
            return View("Slider", result);
        else
            return View("NotFound");
    }
}
