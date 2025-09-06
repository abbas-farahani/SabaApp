using System.Text;
using Core.Domain.Contracts.Services;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Dashboard.Hubs;
using MVC.Areas.Dashboard.Models;
using MVC.Controllers;
using MVC.Models.ViewModels;
using MVC.Tools.Converter;
using Utilities.Email;

namespace MVC.Areas.Dashboard.Controllers;

[Area("Dashboard")]
public class DashboardController : BaseController
{

    #region Injection & Constructor
    private readonly IEmailManagement _emailManager;
    private readonly IViewToStringService _viewToString;
    private readonly IHubContext<CryptoHub> _hubContext;

    public DashboardController(Core.Domain.Contracts.Services.Identity.IUserService userService, IEmailManagement emailManager, IViewToStringService viewToString, IOptionService optionService, IMemoryCache memoryCache, IHubContext<CryptoHub> hubContext) : base(userService, optionService, memoryCache)
    {
        _emailManager = emailManager;
        _viewToString = viewToString;
        _hubContext = hubContext;
    }
    #endregion

    [HttpGet("/dashboard")]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
            return View();
        else
            return RedirectToAction("Login");
    }


    #region Login
    [HttpGet("/Login")]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ViewBag.ReturnUrl = returnUrl;

        if (await _optionService.GetByName("LoginWithPhone") == "true") ViewBag.LoginWithPhone = true;
        else ViewBag.LoginWithPhone = false;

        if (await _optionService.GetByName("AllowRegistration") == "true") ViewBag.AllowRegistration = true;
        else ViewBag.AllowRegistration = false;

        if (await _optionService.GetByName("LoginWithSocial") == "true") ViewBag.LoginWithSocial = true;
        else ViewBag.LoginWithSocial = false;

        if (await _optionService.GetByName("LoginWithGoogle") == "true") ViewBag.LoginWithGoogle = true;
        else ViewBag.LoginWithGoogle = false;

        return View();
    }

    [HttpPost("/Login")]
    public async Task<IActionResult> Login(LoginVM model, CancellationToken cancellationToken, string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ViewBag.ReturnUrl = returnUrl;

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
            TempData["ErrorMessage"] = "حساب کابری شما قفل شده است";
            ModelState.AddModelError(string.Empty, "حساب کابری شما قفل شده است");
            return View(model);
        }

        TempData["ErrorMessage"] = "نام کاربری یا رمز عبور را صحیح نیست";
        ModelState.AddModelError(string.Empty, "تلاش برای ورود نامعتبر میباشد");
        return View();
    }
    #endregion


    #region Register
    [HttpGet("/Register")]
    public async Task<IActionResult> Register()
    {
        ViewBag.IsSent = false;
        if (await _optionService.GetByName("AllowRegistration") == "true") return View();
        else return NotFound();
    }

    [HttpPost("/Register")]
    public async Task<IActionResult> Register(RegisterVM model, CancellationToken cancellationToken)
    {
        if (await _optionService.GetByName("AllowRegistration") == "false") return NotFound();

        //model.Phone = "09124899344";
        if (!ModelState.IsValid) return View();

        var newUser = new User()
        {
            UserName = model.UserName,
            Email = model.Email,
        };

        if (await _optionService.GetByName("MobileRegisteration") == "yes")
        {
            newUser.PhoneNumber = model.Phone;
        }

        var result = await _userService.CreateUser(newUser, model.Password);

        if (!result.Succeeded)
        {
            AddModelError(result.Errors);
            return View();
        }

        var user = await _userService.GetByUserName(model.UserName);
        var userRole = await _userService.AddToRole(user, new List<string> { "Subscriber" });
        if (await _optionService.GetByName("SendEmailConfirmation") == "yes")
        {
            var token = await _userService.GenerateEmailConfirmationToken(user);
            string? callBackUrl = Url.ActionLink("ConfirmEmail", "Account", new { userId = user.Id, token = token },
                Request.Scheme);
            string body = await _viewToString.RenderToStringAsync("_RegisterEmail", callBackUrl);
            await _emailManager.SendEmailAsync(new EmailModel(user.Email, "تایید حساب", body));
            ViewBag.IsSent = true;
        }
        if (await _optionService.GetByName("SendPhoneConfirmation") == "yes")
        {
            var token = await _userService.GenerateTwoFactorToken(user, "Phone");
            return RedirectToAction("ConfirmPhone", new { phone = user.PhoneNumber, token = token });

        }

        TempData["SuccessMessage"] = "حساب کاربری ایجاد شد";
        return RedirectToAction("Index", "Home");
    }
    #endregion

    #region Confirmation Mobile
    [HttpGet("/ConfirmPhone")]
    public async Task<IActionResult> ConfirmPhone(string phone, string token)
    {
        if (phone == null || token == null) return BadRequest();
        ConfirmMobileVM model = new ConfirmMobileVM
        {
            Phone = phone,
            Code = token
        };
        return View();
        //var user = await _userService.GetById(userId);
        //if (user == null) return NotFound();

        //token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //var result = await _userService.ConfirmEmail(user, token);
        //ViewBag.IsConfirmed = result.Succeeded ? true : false;
    }

    [HttpPost("/ConfirmPhone")]
    public async Task<IActionResult> ConfirmPhone(ConfirmMobileVM model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userService.GetByPhone(model.Phone);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "کاربری یافت نشد.");
            return View(model);
        }

        var result = await _userService.ConfirmPhone(user, model.Code);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "کد وارد شده نامعتبر است.");
            return View(model);
        }

        user.PhoneNumberConfirmed = true;
        var update = await _userService.UpdateUser(user);

        return RedirectToAction("Login");
    }
    #endregion

    #region Logout
    [Authorize]
    [HttpPost("/Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut()
    {
        await _userService.SignOutUser();
        return RedirectToAction("Index", "Home");
    }
    #endregion


    #region Confirmation Email
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null) return BadRequest();
        var user = await _userService.GetById(userId);
        if (user == null) return NotFound();

        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userService.ConfirmEmail(user, token);
        ViewBag.IsConfirmed = result.Succeeded ? true : false;
        return View();
    }
    #endregion


    #region Forget Password
    [HttpGet("/reset")]
    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost("/reset")]
    public async Task<IActionResult> ResetPassword(ForgottenPasswordVM model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userService.GetByEmail(model.Email);
        if (user != null)
        {
            var token = await _userService.GetPasswordTokenEncoded(user, cancellationToken);
            string? callBackUrl = Url.ActionLink("newpassword", "home", new { email = user.Email, token = token }, Request.Scheme);
            string body = await _viewToString.RenderToStringAsync("_ResetPasswordEmail", callBackUrl);
            await _emailManager.SendEmailAsync(new EmailModel(user.Email, "بازیابی رمز عبور", body));
        }

        //TODO: Set Alert (پیام برای ایمیل ارسال شد.)
        return View();
    }


    [HttpGet("/newpassword")]
    public IActionResult NewPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token)) return BadRequest();
        ResetPasswordVM model = new ResetPasswordVM
        {
            Email = email,
            Token = token
        };

        return View(model);
    }

    [HttpPost("/newpassword")]
    public async Task<IActionResult> NewPassword(ResetPasswordVM model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userService.GetByEmail(model.Email);
        if (user == null)
        {
            //TODO: Alert user not Found, Mission Failed
            ModelState.AddModelError(string.Empty, "تلاش برای بازیابی رمزعبور ناموفق بود");
            return View(model);
        }
        var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        var result = await _userService.ResetPassword(user, token, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
        }
        TempData["SuccessMessage"] = "رمزعبور با موفقیت بروزرسانی شد";
        return RedirectToAction("/login");
    }
    #endregion


    #region Two Step Authentication
    [HttpGet("/2fa")]
    public IActionResult TwoStepAuth()
    {
        return View();
    }
    #endregion


    #region Remote Validation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExistsUserName(string userName)
    {
        bool result = await _userService.GetAnyByUserName(userName);
        return result ? Json(true) : Json("نام کاربری تکراری است");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExistsEmail(string email)
    {
        bool result = await _userService.GetAnyByEmail(email);
        return result ? Json(true) : Json("ایمیل تکراری است");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExistsPhone(string phone)
    {
        bool result = await _userService.GetAnyByPhone(phone);
        return result ? Json(true) : Json("شماره تلفن تکراری است");
    }
    #endregion


    [AllowAnonymous]
    [HttpGet("/profile/{username}")]
    public async Task<IActionResult> Profile(string username)
    {
        if (string.IsNullOrEmpty(username)) return NotFound();

        var user = await _userService.GetByUserName(username);
        if (user == null) return NotFound();

        return View(user);
    }

}
