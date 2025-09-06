using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MVC.Models.ViewModels;
using System.Text;
using MVC.Tools.Converter;
using Utilities.Email;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Core.Domain.Contracts.Services.Identity;
using MVC.Controllers;
using MVC.Models.ViewModels.User;
using MVC.Models.ViewModels.Role;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Areas.Admin.Models.ViewModels;
using Core.Services;
using Core.Domain.Contracts.Services;

namespace MVC.Areas.Admin.Controllers.Identity;

[Area("Admin")]
//[Authorize(Roles = "Admin")]
public class UserController : BaseController
{
    #region Injection & Constructor
    private readonly IRoleService _roleService;
    private readonly IEmailManagement _emailManager;
    private readonly IViewToStringService _viewToString;
    public UserController(IRoleService roleService, IEmailManagement emailManager, IViewToStringService viewToString, IUserService userService, IMemoryCache memoryCache, IOptionService optionService) : base(userService, optionService, memoryCache)
    {
        _roleService = roleService;
        _emailManager = emailManager;
        _viewToString = viewToString;
    }
    #endregion


    #region View
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAll(cancellationToken);

        #region Viewbags
        ViewBag.Title = "مدیریت کاربران";
        ViewBag.CreateBtn = new CustomBtn
        {
            Text = "کاربر جدید",
            Link = Url.Action("register", "user", new { area = "admin" }, Request.Scheme),
            Title = "کاربر جدید",
            Icon = new KeenIcon { IconName = "plus", PathCount = 0 },
        };
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="کاربران", Title="مدیریت کاربران"},
        };
        #endregion
        return View(users);
    }

    #endregion

    #region Register
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        // TODO: Select list of roles

        #region Roles List - Parent Id
        var roles = await _roleService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>();
        selectListItems.AddRange(roles.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
        }));
        ViewData["RolesId"] = selectListItems;
        #endregion

        #region Viewbags
        ViewBag.Title = "کاربر جدید";
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="کاربران", Title="کاربران", Link=Url.Action("index", "user", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text="کاربر جدید", Title="کاربر جدید"},
        };
        #endregion
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.CreateUser(new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.Phone
            }, model.Password);

            if (!result.Succeeded)
            {
                AddModelError(result.Errors);
                TempData["ErrorMessage"] = "امکان ایجاد کاربر وجود ندارد";
                return View(model);
            }

            var user = await _userService.GetByUserName(model.UserName);

            var token = await _userService.GenerateEmailConfirmationToken(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callBackUrl = Url.ActionLink("ConfirmEmail", "Account", new { userId = user.Id, token },
                Request.Scheme);
            string body = await _viewToString.RenderToStringAsync("_RegisterEmail", callBackUrl);
            await _emailManager.SendEmailAsync(new EmailModel(user.Email, "تایید حساب", body));

            ViewBag.IsSent = true;
            TempData["SuccessMessage"] = "کاربر جدید ایجاد شد";
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = "امکان ایجاد کاربر وجود ندارد";

        #region Roles List - Parent Id
        var roles = await _roleService.GetAllAsync();
        List<SelectListItem> selectListItems = new List<SelectListItem>();
        selectListItems.AddRange(roles.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
        }));
        ViewData["RolesId"] = selectListItems;
        #endregion

        #region Viewbags
        ViewBag.Title = "کاربر جدید";
        ViewBag.Breadcrumb = new List<AnchorLink>
        {
            new AnchorLink{Text="کاربران", Title="کاربران", Link=Url.Action("index", "user", new { area = "admin" }, Request.Scheme)},
            new AnchorLink{Text="کاربر جدید", Title="کاربر جدید"},
        };
        #endregion
        return View(model);
    }
    #endregion

    #region Edit
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var x = "sedjamalllashoor@gmail.com";
        var y = x.ToUpper();
        var user = await _userService.GetById(id);
        if (user == null)
            return NotFound();

        #region Claims
        var claims = await _userService.GetClaims(user);

        var model = new UserVM
        {
            Id = user.Id,
            UserName = user.UserName,
            Phone = user.PhoneNumber,
            Province = claims.FirstOrDefault(c => c.Type == "Province")?.Value,
            Avatar = claims.FirstOrDefault(c => c.Type == "Avatar")?.Value,
        };
        #endregion

        var rolesList = (await _roleService.GetAllAsync()).Select(r => new RolesVM()
        {
            Id = r.Id,
            roleName = r.Name
        })
        .ToList();

        ViewBag.Roles = rolesList;
        ViewBag.UserRoles = await _userService.GetRoles(user);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserVM model, List<string> SelectedRoles)
    {
        var user = await _userService.GetById(model.Id);
        if (user == null) return NotFound();
        user.UserName = model.UserName;
        user.PhoneNumber = model.Phone;
        var result = await _userService.UpdateUser(user);

        var userRoles = await _userService.GetRoles(user);
        await _userService.RemoveRole(user, userRoles);
        await _userService.AddToRole(user, SelectedRoles);

        if (result.Succeeded)
        {
            await _userService.RefreshSignIn(user);
            return RedirectToAction("index");
        }

        AddModelError(result.Errors);

        var Model = new EditUserVM()
        {
            Id = user.Id,
            UserName = user.UserName,
            Phone = user.PhoneNumber,
        };

        var rolesList = (await _roleService.GetAllAsync()).Select(r => new RolesVM()
        {
            Id = r.Id,
            roleName = r.Name
        })
            .ToList();

        ViewBag.Roles = rolesList;
        ViewBag.UserRoles = await _userService.GetRoles(user);

        return View(Model);
    }
    #endregion

    #region Logout
    [HttpPost]
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

    #region Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userService.GetById(id);
        if (user == null)
            return NotFound();
        await _userService.DeleteUser(user);

        return RedirectToAction("Index");
    }
    #endregion
}
