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

public class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _context.Comments
            .Include(x=>x.CommentMetas)
            .Include(x=>x.Post)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<Comment> GetByIdAsync(int id)
    {
        return await _context.Comments
            .Include(x => x.CommentMetas)
            .Include(x => x.Post)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> IsExistById(int id)
    {
        return await _context.Comments.AsNoTracking().AnyAsync(c => c.Id == id);
    }
}