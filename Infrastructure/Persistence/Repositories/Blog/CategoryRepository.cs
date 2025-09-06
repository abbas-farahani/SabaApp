using Core.Domain.Contracts.Repositories;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
	private readonly AppDbContext _context;
	public CategoryRepository(AppDbContext context) : base(context)
	{
		_context = context;
	}

    public async Task<Category> GetBySlug(string slug)
    {
		return await _context.Categories.Where(x => x.Slug == slug)
            .Include(x=>x.Posts)
            .ThenInclude(x=>x.Comments)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _context.Categories.AnyAsync(x => x.Slug == slug);
    }
}