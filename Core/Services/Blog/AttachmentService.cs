using AngleSharp.Dom;
using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Utilities.User;

namespace Core.Services;

public class AttachmentService : BaseService<Attachment>, IAttachmentService
{
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ITextProcessingService _textProcessingService;

    public AttachmentService(IAttachmentRepository attachmentRepository, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor, ITextProcessingService textProcessingService) : base(attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
        _webHostEnvironment = webHostEnvironment;
        _contextAccessor = contextAccessor;
        _textProcessingService = textProcessingService;
    }

    public async Task<Attachment> GetBySlug(string slug)
    {
        return await _attachmentRepository.GetBySlug(slug);
    }

    public async Task<object> UploadFile(IFormFile file, int chunkIndex, int totalChunks, StringValues fileName, string scheme, HostString host)
    {
        string rootPath = _webHostEnvironment.WebRootPath;
        string serverPath = Path.Combine(rootPath, "uploads\\" + DateTime.Now.ToString("yyyy-MM-dd"));
        string relativePath = Path.GetRelativePath(rootPath, serverPath);
        string fileExtension = Path.GetExtension(file.FileName);
        string slug = _textProcessingService.GenerateSlug(Path.GetFileNameWithoutExtension(file.FileName));
        fileName = $"{slug}{fileExtension}";
        if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);

        int progress = 0;

        progress = (int)((double)chunkIndex / totalChunks * 100.0);
        string filePathTemp = Path.Combine(rootPath, "uploads\\" + DateTime.Now.ToString("yyyy-MM-dd") + $"\\temp-{fileName}");
        if (!Directory.Exists(filePathTemp)) Directory.CreateDirectory(filePathTemp);

        #region Access Directory to write
        var dirInfo = new DirectoryInfo(filePathTemp);
        var accessDir = dirInfo.GetAccessControl();
        accessDir.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
        dirInfo.SetAccessControl(accessDir);
        #endregion

        try
        {
            string chunkPath = Path.Combine(filePathTemp, $"{chunkIndex}.part");
            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // After Last Part uploaded
            if (chunkIndex == totalChunks - 1)
            {
                string finalPath = Path.Combine(serverPath, $"{fileName}");
                if (File.Exists(finalPath))
                {
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    finalPath = Path.Combine(serverPath, $"{uniqueFileName}");
                }
                using (var finalStream = new FileStream(finalPath, FileMode.Create))
                {
                    for (int i = 0; i < totalChunks; i++)
                    {
                        var partPath = Path.Combine(filePathTemp, $"{i}.part");
                        var partBytes = await System.IO.File.ReadAllBytesAsync(partPath);
                        await finalStream.WriteAsync(partBytes, 0, partBytes.Length);
                        System.IO.File.Delete(partPath);
                    }
                }
                Directory.Delete(filePathTemp);
                GC.Collect();
                var userPath = serverPath.Replace("\\", "/");

                var fileUrl = $"{scheme}://{host}/{relativePath.Replace("\\", "/")}/{fileName}";
                long fileSize = await FileSize(fileUrl, finalPath);

                await _attachmentRepository.CreateAsync(new Attachment
                {
                    CreationTime = DateTime.Now,
                    LastModified = null,
                    UserId = _contextAccessor.HttpContext.User.GetUserId(),
                    Title = _textProcessingService.SanitizeHtml(file.FileName),
                    FileName = fileName,
                    FileExtension = fileExtension,
                    FileSize = fileSize,
                    Description = "",
                    Slug = slug,
                    Path = $"/{relativePath.Replace("\\", "/")}/{fileName}",
                    Status = 1,
                });
                return new
                {
                    status = "success",
                    url = fileUrl,
                    fileName = fileName
                };
            }
            return new { status = "success" };
        }
        catch (Exception err)
        {
            if (Directory.Exists(filePathTemp))
                Directory.Delete(filePathTemp, true);
            return new { status = "failed", message = err.Message };
        }
    }

    public async Task<object> UploadFile(IFormFile file, string scheme, HostString host)
    {
        if (file == null) return new { status = "failed", message = "null" };
        string rootPath = _webHostEnvironment.WebRootPath;
        string serverPath = Path.Combine(rootPath, "uploads\\" + DateTime.Now.ToString("yyyy-MM-dd"));
        string relativePath = Path.GetRelativePath(rootPath, serverPath);
        string fileExtension = Path.GetExtension(file.FileName);
        string slug = _textProcessingService.GenerateSlug(Path.GetFileNameWithoutExtension(file.FileName));
        string fileName = $"{slug}{fileExtension}";

        if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);

        #region Access Directory to write
        var dirInfo = new DirectoryInfo(serverPath);
        var accessDir = dirInfo.GetAccessControl();
        accessDir.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
        dirInfo.SetAccessControl(accessDir);
        #endregion

        try
        {
            string filePath = Path.Combine(serverPath, $"{fileName}");

            if (File.Exists(filePath))
            {
                fileName = $"{Guid.NewGuid()}{fileExtension}";
                slug = _textProcessingService.GenerateSlug(Path.GetFileNameWithoutExtension(fileName));
                filePath = Path.Combine(serverPath, $"{fileName}");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            GC.Collect();
            var userPath = serverPath.Replace("\\", "/");

            var fileUrl = $"{scheme}://{host}/{relativePath.Replace("\\", "/")}/{fileName}";
            long fileSize = await FileSize(fileUrl, filePath);

            await _attachmentRepository.CreateAsync(new Attachment
            {
                CreationTime = DateTime.Now,
                LastModified = null,
                UserId = _contextAccessor.HttpContext.User.GetUserId(),
                Title = "",//_textProcessingService.SanitizeHtml(file.FileName),
                FileName = fileName,
                FileExtension = fileExtension,
                FileSize = fileSize,
                Description = "",
                Slug = slug,
                Path = $"/{relativePath.Replace("\\", "/")}/{fileName}",
                Status = 1,
            });
            return new
            {
                status = "success",
                url = fileUrl,
                path = $"/{relativePath.Replace("\\", "/")}/{fileName}",
                fileName = fileName
            };
        }
        catch (Exception err)
        {
            return new { status = "failed", message = err.Message };
        }
    }

    public async Task<object> AttachThumbnail(IFormFile file, string scheme, HostString host)
    {
        string rootPath = _webHostEnvironment.WebRootPath;
        string serverPath = Path.Combine(rootPath, "uploads\\" + DateTime.Now.ToString("yyyy-MM-dd"));
        string relativePath = Path.GetRelativePath(rootPath, serverPath);
        string fileExtension = Path.GetExtension(file.FileName);
        string slug = _textProcessingService.GenerateSlug(Path.GetFileNameWithoutExtension(file.FileName));
        string fileName = $"{slug}{fileExtension}";

        if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);

        #region Access Directory to write
        var dirInfo = new DirectoryInfo(serverPath);
        var accessDir = dirInfo.GetAccessControl();
        accessDir.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
        dirInfo.SetAccessControl(accessDir);
        #endregion

        try
        {
            string filePath = Path.Combine(serverPath, $"{fileName}");

            if (File.Exists(filePath))
            {
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                filePath = Path.Combine(serverPath, $"{uniqueFileName}");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            GC.Collect();
            var userPath = serverPath.Replace("\\", "/");

            var fileUrl = $"{scheme}://{host}/{relativePath.Replace("\\", "/")}/{fileName}";
            long fileSize = await FileSize(fileUrl, filePath);

            await _attachmentRepository.CreateAsync(new Attachment
            {
                CreationTime = DateTime.Now,
                LastModified = null,
                UserId = _contextAccessor.HttpContext.User.GetUserId(),
                Title = _textProcessingService.SanitizeHtml(file.FileName),
                FileName = fileName,
                FileExtension = fileExtension,
                FileSize = fileSize,
                Description = "",
                Slug = slug,
                Path = $"/{relativePath.Replace("\\", "/")}/{fileName}",
                Status = 1,
            });
            return new
            {
                status = "success",
                url = fileUrl,
                fileName = fileName
            };
        }
        catch (Exception err)
        {
            return new { status = "failed", message = "خطایی هنگام بارگذاری تصویر رخ داد" };
            //return new { status = "failed", message = err.Message };
        }
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var attachment = await this.GetByIdAsync(id);
        if (attachment == null) return false;
        if (File.Exists(attachment.Path))
        {
            File.Delete(attachment.Path);
            base.DeleteAsync(id);
            return true;
        }
        return false;
    }

    private async Task<long> FileSize(string url, string path)
    {
        long fileSize = 0;
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength.HasValue)
                {
                    fileSize = response.Content.Headers.ContentLength.Value;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    FileInfo fileInfo = new FileInfo(path);
                    fileSize = fileInfo.Length;
                }
            }
            catch (Exception er)
            {
                return fileSize;
            }
        }
        return fileSize;
    }
}
