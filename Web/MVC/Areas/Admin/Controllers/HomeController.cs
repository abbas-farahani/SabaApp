using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MVC.Models.ViewModels;
using System.Text;
using MVC.Tools.Converter;
using Utilities.Email;
using Core.Domain.Entities;
using Core.Domain.Contracts.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using MVC.Areas.Admin.Helpers.Attributes;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{

    #region Injection & Constructor
    private readonly IUserService _userService;
    private readonly IEmailManagement _emailManager;
    private readonly IViewToStringService _viewToString;
    public HomeController(IUserService userService, IEmailManagement emailManager, IViewToStringService viewToString)
    {
        _userService = userService;
        _emailManager = emailManager;
        _viewToString = viewToString;
    }
    #endregion

    [HttpGet("/admin")]
    //[Authorize(Roles = "Admin")]
    [Permission("Admin.Read")]
    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
            return View();
        else
            return RedirectToAction("Login");
    }


    #region Login
    [HttpGet("/admin/login")]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("/admin/login")]
    public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "نام کاربری و رمز عبور را وارد کنید";
            return View(model);
        }


        var user = await _userService.GetByUserName(model.UserName);
        if (user == null)
        {
            TempData["ErrorMessage"] = "کاربری با این مشخصات یافت نشد";
            ModelState.AddModelError(string.Empty, "کاربری با این مشخصات یافت نشد");
            return View(model);
        }
        var result = await _userService.SignInUser(model.UserName, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
        else if (result.RequiresTwoFactor)
        {
            return RedirectToAction("LoginWith2fa");
        }
        else if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "حساب کابری شما قفل شده است");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "تلاش برای ورود نامعتبر میباشد");
        return View();
    }
    #endregion


    #region Forget Password
    [HttpGet("/admin/reset")]
    public IActionResult ResetPassword()
    {
        return View();
    }
    #endregion


    #region Change Old Password
    [HttpGet("/admin/newpassword")]
    public IActionResult NewPassword()
    {
        return View();
    }
    #endregion


    #region Two Step Authentication
    [HttpGet("/admin/2fa")]
    public IActionResult TwoStepAuth()
    {
        return View();
    }
    #endregion
}
