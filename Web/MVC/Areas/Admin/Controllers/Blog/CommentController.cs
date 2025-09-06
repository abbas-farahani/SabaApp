using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.Eventing.Reader;
using MVC.Areas.Admin.Helpers.Attributes;
using Core.Domain.Contracts.Services;
using Core.Services;
using MVC.Models.ViewModels;
using Core.Domain.Dtos.Blog;
using Utilities.User;
using Utilities.HtmlSanitizer;
using MVC.Controllers;
using Core.Services.Identity;
using Microsoft.Extensions.Caching.Memory;
using Core.Domain.Contracts.Services.Identity;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CommentController : BaseController
{
    private readonly ICommentService _commentService;
    private readonly IPostService _postService;

    public CommentController(IOptionService optionService, IUserService userService, IMemoryCache memoryCache, ICommentService commentService, IPostService postService) : base(userService, optionService, memoryCache)
    {
        _commentService = commentService;
        _postService = postService;
    }

    #region View
    [Permission("Comment.Read")]
    public async Task<IActionResult> Index(int? id)
    {
        var comments = await _commentService.GetAllDtoAsync();
        return View(comments);
    }

    // GET: Admin/Comment/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var comment = await _commentService.GetById(id.Value);
        if (comment == null) return NotFound();

        return View(comment);
    }
    #endregion

    #region Create
    [AllowAnonymous]
    [HttpPost("/newcomment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommentVM model)
    {
        var post = await _postService.GetByIdAsync(model.PostId);
        var user = new User();
        // CommentStatus: 0:Close - 1:Open - 2:Private
        if (post.CommentStatus == 0 || (post.CommentStatus == 2 && !User.Identity.IsAuthenticated))
            return Ok(new { result = false, message = "امکان ثبت دیدگاه وجود ندارد" });

        if (User.Identity.IsAuthenticated)
        {
            user = string.IsNullOrEmpty(model.UserName) ? default : await base.GetCurrentUser(model.UserName);
            model.Name = User.Identity.Name;
            model.Email = user.Email;
        }
        ModelState.Remove("Website");
        ModelState.Clear();
        TryValidateModel(model);
        if (ModelState.IsValid)
        {
            var comment = new CommentDto()
            {
                CreationTime = DateTime.Now,
                PostId = model.PostId,
                ParentId = model.ParentId == null ? 0 : model.ParentId.Value,
                UserId = (User.Identity.IsAuthenticated ? User.GetUserId() : ""),
                UserName = User.Identity.IsAuthenticated ? user.UserName : model.Name,
                Email = model.Email,
                Website = model.Website,
                Ip = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Agent = Request.Headers.UserAgent,
                Content = TextSanitizer.Sanitize(model.Content),
                Status = 0,
            };
            var result = await _commentService.AddDtoAsync(comment);
            if (result == null) return Ok(new { result = false, message = "ثبت دیدگاه با خطا مواجه شد" });
            return Ok(new { result = true });
        }
        return Ok(new { result = false, message = "اطلاعات فرم را به درستی تکمیل کنید" });
    }
    #endregion

    #region Edit
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var comment = await _commentService.GetById(id.Value);
        if (comment == null) return NotFound();

        return View(comment);
    }

    // POST: Admin/Comment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CommentDto model)
    {
        if (id != model.Id) return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                _commentService.UpdateDtoAsync(model);
                TempData["SuccessMessage"] = "دیدگاه بروزرسانی شد";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (await _commentService.IsExistById(id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        TempData["ErrorMessage"] = "بروزرسانی انجام نشد";
        return View(model);
    }
    #endregion

    #region Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Comment.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _commentService.IsExistById(id))
        {
            await _commentService.DeleteAsync(id);
            return Json(new { result = true });
        }
        return Json(new { result = false });
    }
    #endregion
}
