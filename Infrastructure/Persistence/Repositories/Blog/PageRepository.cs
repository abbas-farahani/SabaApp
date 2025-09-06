using Core.Domain.Contracts.Repositories;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories;

public class PageRepository : BaseRepository<Page>, IPageRepository
{
    private readonly AppDbContext _context;
    public PageRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Page>> GetAllAsync(string lang)
    {
        return await _context.Pages
            .Include(x => x.PageMetas)
            .Where(m => m.PageMetas.Any(l => l.MetaName == "LanguageCode" && l.MetaValue == lang))
            .Include(x => x.User)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Page>> GetAllAsync()
    {
        return await _context.Pages
            .Include(x => x.PageMetas)
            .Include(x => x.User)
            .ToListAsync();
    }

    public override async Task<Page> GetByIdAsync(int id)
    {
        var post = await _context.Pages
            .Where(x => x.Id == id)
            .Include(x => x.PageMetas)
            .Include(x => x.User)
            .FirstOrDefaultAsync();
        return post;
    }

    public async Task<Page> GetBySlug(string slug)
    {
        return await _context.Pages
            .Include(x => x.PageMetas)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<List<Page>> GetByUserId(string userId)
    {
        return await _context.Pages
            .Where(x => x.UserId == userId.ToString())
            .Include(x => x.PageMetas)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<List<Page>> GetOriginalPages(string defaultLanguage)
    {
        return await _context.Pages
            .Include(x => x.PageMetas)
            .Where(x => x.PageMetas.Where(m => m.MetaName == "LanguageCode" && m.MetaValue == defaultLanguage).Any())
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _context.Pages.AnyAsync(x => x.Slug == slug);
    }

    public async Task<bool> IsExistById(int id)
    {
        return await _context.Pages.AnyAsync(x => x.Id == id);
    }
}