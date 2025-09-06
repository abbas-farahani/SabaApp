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

public class AttachmentRepository : BaseRepository<Attachment>, IAttachmentRepository
{
    private readonly AppDbContext _context;
    public AttachmentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Attachment> GetBySlug(string slug)
    {
        return await _context.Attachments.Where(x => x.Slug == slug).FirstOrDefaultAsync();
    }
}