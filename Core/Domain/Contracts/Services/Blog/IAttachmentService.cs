using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Core.Domain.Contracts.Services;

public interface IAttachmentService : IBaseService<Attachment>
{
    Task<Attachment> GetBySlug(string slug);
    Task<object> UploadFile(IFormFile file, int chunkIndex, int totalChunks, StringValues fileName, string scheme, HostString host);
    Task<object> UploadFile(IFormFile file, string scheme, HostString host);
    Task<object> AttachThumbnail(IFormFile file, string scheme, HostString host);

}
