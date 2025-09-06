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

public class PostMetaRepository : BaseRepository<PostMeta>, IPostMetaRepository
{
    public PostMetaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PostMeta> GetByIdAsync(int postId, string metaName)
    {
        return await _context.PostMetas.FirstOrDefaultAsync(x => x.PostId == postId && x.MetaName == metaName);
    }

    public async Task<IEnumerable<PostMeta>> AddAsync(IEnumerable<PostMeta> metas)
    {
        await _context.PostMetas.AddRangeAsync(metas);
        await _context.SaveChangesAsync();
        return metas;
    }
}