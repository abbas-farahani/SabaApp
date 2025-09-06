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

public class PageMetaRepository : BaseRepository<PageMeta>, IPageMetaRepository
{
    public PageMetaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PageMeta> GetByIdAsync(int pageId, string metaName)
    {
        return await _context.PageMetas.FirstOrDefaultAsync(x => x.PageId == pageId && x.MetaName == metaName);
    }
}