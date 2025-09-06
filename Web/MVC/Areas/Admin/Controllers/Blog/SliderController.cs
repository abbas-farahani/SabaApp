using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Admin.Models.ViewModels;
using MVC.Controllers;
using MVC.Models.ViewModels;
using Utilities.HtmlSanitizer;

namespace MVC.Areas.Admin.Controllers.Blog;

[Area("Admin")]
[Authorize]
public class SliderController : BaseController
{
    private readonly ITextProcessingService _textProcessingService;

    public SliderController(IUserService userService, IMemoryCache memoryCache, IOptionService optionService, ITextProcessingService textProcessingService) : base(userService, optionService, memoryCache)
    {
        _textProcessingService = textProcessingService;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sliders = await _optionService.GetAllSliders();
        var result = new List<SliderVM>();
        sliders.Select(x =>
        {
            result.Add(new SliderVM
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                CreationTime = x.CreationTime,
                LastModified = x.LastModified,
                Slides = x.Slides.Select(y => new SlideVM
                {
                    SliderId = x.Id,
                    SliderName = x.Name,
                    Id = y.Id,
                    CreationTime = y.CreationTime,
                    LastModified = y.LastModified,
                    Name = y.Name,
                    Title = y.Title,
                    Url = y.Url,
                    BackgroundType = y.BackgroundType,
                    BackgroundValue = y.BackgroundValue,
                }).ToList(),
            });
            return x;
        }).ToList();

        #region Viewbags
        ViewBag.Title = "اسلایدرها";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "اسلایدر جدید",
            Link = Url.Action("createslider", "slider", new { area = "admin" }, Request.Scheme),
            Title = "اسلایدر جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="اسلایدرها", Title="اسلایدرها"},
        };
        #endregion

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var slider = await _optionService.GetSlider(id);
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

        #region Viewbags
        ViewBag.Title = $"اسلایدر {slider.Title}";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "اسلاید جدید",
            Link = Url.Action("create", "slider", new { area = "admin" }, Request.Scheme),
            QueryString = $"id={id}",
            Title = "اسلاید جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="اسلایدرها", Title="اسلایدرها"},
        };
        #endregion

        return View(result);
    }

    #region Sliders
    #region Create Slider
    [HttpGet]
    public async Task<IActionResult> CreateSlider()
    {
        #region Viewbags
        ViewBag.Title = "اسلایدر جدید";
        #endregion

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSlider(SliderVM model)
    {
        if (ModelState.IsValid)
        {
            var slider = new SliderDto
            {
                Title = TextSanitizer.Sanitize(model.Title.Trim()),
                Name = _textProcessingService.SanitizeHtml(model.Name),
                //Name = string.IsNullOrEmpty(model.Name) ? TextSanitizer.Sanitize(model.Title.Trim().Replace(" ", "-")) : TextSanitizer.Sanitize(model.Name.Trim().Replace(" ", "-")),
            };

            var result = await _optionService.UpdateSlider(slider);
            TempData["SuccessMessage"] = "اسلایدر جدید ایجاد شد";
            return RedirectToAction("index");
        }
        TempData["ErrorMessage"] = "هنگام ایجاد اسلایدر خطایی رخ داد";

        #region Viewbags
        ViewBag.Title = "اسلایدر جدید";
        #endregion

        return View(model);
    }
    #endregion

    #region Edit Slider
    [HttpGet]
    public async Task<IActionResult> EditSlider(int id)
    {
        var slider = await _optionService.GetSlider(id);
        var result = new SliderVM
        {
            Name = slider.Name,
            Title = slider.Title
        };

        #region Viewbags
        ViewBag.Title = "ویرایش اسلایدر";
        #endregion

        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSlider(SliderVM model)
    {
        if (model.Id == null || model.Id == 0) return NotFound();
        if (ModelState.IsValid)
        {
            var slider = await _optionService.GetSlider(model.Id);
            if (slider != null)
            {
                slider.Name = _textProcessingService.SanitizeHtml(model.Name);
                slider.Title = model.Title;
                slider.LastModified = DateTime.Now;
            }

            var result = await _optionService.UpdateSlider(slider);
            TempData["SuccessMessage"] = "اسلایدر جدید ایجاد شد";
            return RedirectToAction("index");
        }
        TempData["ErrorMessage"] = "خطایی رخ داد";

        #region Viewbags
        ViewBag.Title = "ویرایش اسلایدر";
        #endregion

        return View(model);
        //return View();
    }
    #endregion

    #region Delete Slider
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSlider(int id)
    {
        var slider = await _optionService.GetSlider(id);
        if (slider != null)
        {
            await _optionService.DeleteSlider(id);
            return Json(new { result = true });
        }
        return Json(new { result = false });
    }
    #endregion
    #endregion












    #region Slides
    #region Create
    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        if (id == 0 || id == null) return NotFound();
        ViewBag.SliderId = id;
        var slider = await _optionService.GetSlider(id);
        var result = new SlideVM
        {
            Id = (slider.Slides != null && slider.Slides.Count() > 0) ? slider.Slides.Max(x => x.Id) + 1 : 1,
            SliderId = id,
            SliderName = slider.Title,
            Name = "",
            Title = "",
            Description = "",
            CreationTime = DateTime.Now,
            Url = "",
            BackgroundType = "",
            BackgroundValue = "",
        };

        #region Viewbags
        ViewBag.Title = "اسلاید جدید";
        #endregion

        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SlideVM model)
    {
        if (ModelState.IsValid)
        {
            var slider = await _optionService.GetSlider(model.SliderId.Value);
            if (slider != null)
            {
                var slide = new SlideDto
                {
                    Name = _textProcessingService.SanitizeHtml(model.Name),
                    Title = TextSanitizer.Sanitize(model.Title),
                    Url = TextSanitizer.SanitizePath(model.Url),
                    Description = TextSanitizer.SanitizePath(model.Description),
                    BackgroundType = TextSanitizer.Sanitize(model.BackgroundType),
                    BackgroundValue = TextSanitizer.SanitizePath(model.BackgroundValue)
                };
                slider.Slides.Add(slide);
                var result = await _optionService.UpdateSlider(slider);
                TempData["SuccessMessage"] = "اسلاید جدید اضافه شد";
                return RedirectToAction("index");
            }
        }
        TempData["ErrorMessage"] = "خطایی رخ داد";

        #region Viewbags
        ViewBag.Title = "اسلاید جدید";
        #endregion

        return View(model);
    }
    #endregion

    #region Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id, int sliderId)
    {
        if (sliderId == 0 || id == 0) return NotFound();
        var slider = await _optionService.GetSlider(sliderId);
        var slide = slider.Slides.FirstOrDefault(x => x.Id == id);
        var result = new SlideVM
        {
            Id = (slider.Slides != null && slider.Slides.Count() > 0) ? slider.Slides.Max(x => x.Id) + 1 : 1,
            SliderId = id,
            Name = "",
            Title = "",
            Description = "",
            CreationTime = DateTime.Now,
            Url = "",
            BackgroundType = "",
            BackgroundValue = "",
        };

        #region Viewbags
        ViewBag.Title = "ویرایش اسلاید";
        #endregion

        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SlideVM model)
    {
        if (model.SliderId == null || model.SliderId == 0) return NotFound();
        if (ModelState.IsValid)
        {
            var slider = await _optionService.GetSlider(model.SliderId.Value);
            if (slider != null)
            {
                var slide = new SlideDto
                {
                    Name = _textProcessingService.SanitizeHtml(model.Name),
                    Title = TextSanitizer.Sanitize(model.Title),
                    Url = TextSanitizer.SanitizePath(model.Url),
                    Description = TextSanitizer.SanitizePath(model.Description),
                    BackgroundType = TextSanitizer.Sanitize(model.BackgroundType),
                    BackgroundValue = TextSanitizer.SanitizePath(model.BackgroundValue)
                };
                slider.Slides.Add(slide);
                var result = await _optionService.UpdateSlider(slider);
                TempData["SuccessMessage"] = "اسلاید بروزرسانی شد";
                return RedirectToAction("index");
            }

        }
        TempData["ErrorMessage"] = "بروزرسانی انجام نشد";

        #region Viewbags
        ViewBag.Title = "ویرایش اسلاید";
        #endregion

        return View(model);
    }
    #endregion

    #region Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slideId)
    {
        var ids = slideId.Split("-").ToList();
        var slider = await _optionService.GetSlider(Int32.Parse(ids[0]));
        if (slider != null)
        {
            await _optionService.DeleteSlide(Int32.Parse(ids[0]), Int32.Parse(ids[1]));
            return Json(new { result = true });
        }
        return Json(new { result = false });
    }
    #endregion
    #endregion

}
