using Core.Domain.Contracts.Repositories;
using Core.Domain.Dtos;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories;

public class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(x => x.PostMetas)
            .Include(x => x.User)
            .Include(x => x.Categories)
            .Include(x => x.Comments)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Post>> GetAllAsync(string lang)
    {
        return await _context.Posts
            .Include(x => x.PostMetas)
            .Where(m => m.PostMetas.Any(l => l.MetaName == "LanguageCode" && l.MetaValue == lang))
            .Include(x => x.User)
            .Include(x => x.Categories)
            .Include(x => x.Comments)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<Post> GetByIdAsync(int id)
    {
        var post = await _context.Posts
            .Include(x => x.PostMetas)
            .Include(x => x.User)
            .Include(x => x.Categories)
            .Include(x => x.Comments)
            //.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return post;
    }

    public async Task<Post> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        return await _context.Posts
            .Include(x => x.PostMetas)
            .Include(x => x.User)
            .Include(x => x.Categories)
            .Include(x => x.Comments)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<List<Post>> GetByUserId(string userId, CancellationToken cancellationToken)
    {
        var posts = await _context.Posts.Where(x => x.UserId == userId.ToString()).Include(x => x.PostMetas).AsNoTracking().ToListAsync(cancellationToken);
        return posts;
    }

    public async Task<List<Post>> GetList(int count, string? culture)
    {
        IQueryable<Post> query = _context.Posts
            .Where(x => x.Status == 1)
            .Include(x => x.PostMetas);

        if (!string.IsNullOrEmpty(culture))
            query = query.Where(x => x.PostMetas.Any(c => c.MetaName == "LanguageCode" && c.MetaValue.StartsWith(culture)));

        return await query.Include(x => x.User)
            .Include(x => x.Categories)
            .Include(x => x.Comments)
            .OrderByDescending(x => x.LastModified)
            .Skip(0)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Post>> GetOriginalProducts(string defaultLanguage)
    {
        return await _context.Posts
            .Include(x => x.PostMetas)
            .Where(x => x.PostMetas.Where(m => m.MetaName == "LanguageCode" && m.MetaValue == defaultLanguage).Any())
            .ToListAsync();
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _context.Posts.AsNoTracking().AnyAsync(x => x.Slug == slug);
    }
}
