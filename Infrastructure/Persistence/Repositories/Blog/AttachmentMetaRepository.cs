using Core.Domain.Contracts.Repositories;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories;

public class AttachmentMetaRepository : BaseRepository<AttachmentMeta>, IAttachmentMetaRepository
{
    public AttachmentMetaRepository(AppDbContext context) : base(context)
    {
    }
}
