using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Repositories.Shop;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Shop;

public class ProductCatRepository : BaseRepository<ProductCat>, IProductCatRepository
{
    private readonly AppDbContext _context;
    public ProductCatRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ProductCat> AddAsync(ProductCat cat)
    {
        await _context.ProductCats.AddAsync(cat);
        await _context.SaveChangesAsync();
        return cat;
    }

    public async Task<List<ProductCat>> GetByIdAsync(List<int> categoryIds)
    {
        return await _context.ProductCats.Where(x => categoryIds.Contains(x.Id)).ToListAsync();
    }

    public async Task<ProductCat> GetBySlug(string slug)
    {
        return await _context.ProductCats.Where(x => x.Slug == slug)
            .Include(x => x.Products)
            .ThenInclude(x => x.Reviews)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _context.ProductCats.AnyAsync(x => x.Slug == slug);
    }
}
