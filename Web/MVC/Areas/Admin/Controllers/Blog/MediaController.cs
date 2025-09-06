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
using Core.Domain.Contracts.Services;
using Microsoft.Extensions.Primitives;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Hosting;
using MVC.Areas.Admin.Helpers.Attributes;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class MediaController : Controller
{
    #region Injection & Construction
    private readonly IAttachmentService _attachmentService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public MediaController(IAttachmentService attachmentService, IWebHostEnvironment webHostEnvironment)
    {
        _attachmentService = attachmentService;
        _webHostEnvironment = webHostEnvironment;
    }
    #endregion


    #region View
    public async Task<IActionResult> Index()
    {
        var attachments = (await _attachmentService.GetAllAsync()).Take(50).ToList();
        return View(attachments);
    }
    #endregion

    #region Create/Upload
    [HttpGet("media/uploader")]
    [Permission("Media.Create")]
    public IActionResult Uploader()
    {
        return View();
    }

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [Route("media/uploader")]
    [Permission("Media.Create")]
    public async Task<IActionResult> Uploader(IFormFile file)
    {
        if (file == null || !User.Identity.IsAuthenticated) return BadRequest("امکان بارگذاری وجود ندارد");
        int chunkIndex = int.Parse(Request.Form["dzchunkindex"]);
        int totalChunks = int.Parse(Request.Form["dztotalchunkcount"]);
        StringValues fileName = Request.Form["dzuuid"];
        var result = await _attachmentService.UploadFile(file, chunkIndex, totalChunks, fileName, Request.Scheme, Request.Host);

        return Ok(result);
    }

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [Route("media/attach")]
    [Permission("Media.Create")]
    public async Task<IActionResult> AttachThumbnail(IFormFile file)
    {
        if (file == null || !User.Identity.IsAuthenticated) return BadRequest("امکان بارگذاری وجود ندارد");
        var result = await _attachmentService.AttachThumbnail(file, Request.Scheme, Request.Host);
        return Ok(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Media.Create")]
    public async Task<IActionResult> Create([Bind("UserId,Title,Path,Description,Status,Id,CreationTime,LastModified")] Attachment attachment)
    {
        if (ModelState.IsValid)
        {
            await _attachmentService.AddAsync(attachment);
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "بارگذاری رسانه با خطا مواجه شد";
        return View(attachment);
    }
    #endregion

    #region Edit
    // GET: Admin/Attachment/Edit/5
    [Permission("Media.Update")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        // TODO: Run Image Cropper
        var attachment = await _attachmentService.GetByIdAsync(id.Value);
        if (attachment == null) return NotFound();
        return View(attachment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Media.Update")]
    public async Task<IActionResult> Edit(int id, [Bind("UserId,Title,Path,Description,Status,Id,CreationTime,LastModified")] Attachment attachment)
    {
        if (id != attachment.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await _attachmentService.UpdateAsync(attachment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await AttachmentExists(attachment.Id))) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(attachment);
    }
    #endregion

    #region Delete
    [Permission("Media.Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var attachment = await _attachmentService.GetByIdAsync(id.Value);
        if (attachment == null) return NotFound();
        return View(attachment);
    }

    // POST: Admin/Attachment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Permission("Media.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _attachmentService.DeleteAsync(id);
        }
        catch (Exception er)
        {
            TempData["ErrorMessage"] = "خطایی هنگام حذف فایل رخ داد";
        }
        TempData["SuccessMessage"] = "فایل حذف شد";
        return RedirectToAction(nameof(Index));
    }
    #endregion


    #region Image (GET|POST)
    //[HttpPost]
    //public async Task<IActionResult> CreateImg(IFormFile upload, CancellationToken cancellationToken)
    //{
    //    ImageKbpost img = null;
    //    AppUserDto currentUser = _memory.Get<AppUserDto>($"currentUser_{User.Identity.Name}");

    //    if (upload != null && upload.Length > 0)
    //    {
    //        var filename = DateTime.Now.ToString("yyyyMMDDHHmmss") + upload.FileName;
    //        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Img", filename);
    //        var stream = new FileStream(path, FileMode.Create);
    //        upload.CopyToAsync(stream);

    //        return new JsonResult(new { path = "/img /" + filename });
    //    }
    //    return RedirectToAction("CreateImg");
    //}

    //[HttpGet]
    //public async Task<IActionResult> ViewImg()
    //{
    //    var dir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Img"));
    //    ViewBag.fileInfo = dir.GetFiles();
    //    //var a= dir.GetFiles();
    //    return View("ViewImg");
    //}

    //[HttpGet]
    //public async Task<IActionResult> DeleteImg(CancellationToken cancellationToken)
    //{
    //    var filename = this.Request.HttpContext.Request.Path.Value.ToString().Split("/")[3];
    //    var directory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Img", filename));
    //    var path = directory.FullName;
    //    if (System.IO.File.Exists(path))
    //    {
    //        System.IO.File.Delete(path);
    //    }
    //    var dir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Img"));
    //    ViewBag.fileInfo = dir.GetFiles();

    //    return View("ViewImg");
    //}

    #endregion


    private async Task<bool> AttachmentExists(int id)
    {
        var attachment = await _attachmentService.GetByIdAsync(id);
        if (attachment != null) return true;
		return false;
	}

}


