using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class AttachmentMetaService : BaseService<AttachmentMeta>, IAttachmentMetaService
{
    private readonly IAttachmentMetaRepository _attachmentRepository;

    public AttachmentMetaService(IAttachmentMetaRepository attachmentRepository) : base(attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
    }
}
