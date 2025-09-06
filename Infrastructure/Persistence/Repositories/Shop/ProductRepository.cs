using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts.Repositories.Shop;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Shop;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }


    public async override Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(x => x.ProductCats)
            .Include(x => x.ProductMetas)
            .Include(x => x.User)
            .Include(x => x.Reviews)
            .ToListAsync();
    }

    public async override Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.Where(x => x.Id == id)
            .Include(x => x.ProductCats)
            .Include(x => x.ProductMetas)
            .Include(x => x.User)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetAllAsync(string lang)
    {
        return await _context.Products
            .Include(x => x.ProductMetas)
            .Where(m => m.ProductMetas.Any(l => l.MetaName == "LanguageCode" && l.MetaValue == lang))
            .Include(x => x.ProductCats)
            .Include(x => x.User)
            .Include(x => x.Reviews)
            .ToListAsync();
    }

    public async Task<Product> GetBySlug(string slug)
    {
        return await _context.Products.Where(x => x.Slug == slug)
            .Include(x => x.ProductCats)
            .Include(x => x.ProductMetas)
            .Include(x => x.User)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetOriginalProducts(string defaultLanguage)
    {
        return await _context.Products
            .Include(x => x.ProductMetas)
            .Where(x => x.ProductMetas.Where(m => m.MetaName == "LanguageCode" && m.MetaValue == defaultLanguage).Any())
            .ToListAsync();
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _context.Products.AnyAsync(x => x.Slug == slug);
    }

    public async Task<List<Product>> GetList(int count, string? culture)
    {
        var products = await _context.Products
            .Where(x => x.Status == 1)
            .Include(x => x.ProductMetas)
            .Where(x => x.ProductMetas.Any(c => c.MetaName == "LanguageCode" && c.MetaValue.StartsWith(culture)))
            .Include(x => x.User)
            .Include(x => x.ProductCats)
            .Include(x => x.Reviews)
            .OrderByDescending(x => x.LastModified)
            .Skip(0)
            .Take(count)
            .ToListAsync();

        IQueryable<Product> query = _context.Products
            .Where(x => x.Status == 1)
            .Include(x => x.ProductMetas);

        if (!string.IsNullOrEmpty(culture))
            query = query.Where(x => x.ProductMetas.Any(c => c.MetaName == "LanguageCode" && c.MetaValue.StartsWith(culture)));

        return await query.Include(x => x.User)
            .Include(x => x.ProductCats)
            .Include(x => x.Reviews)
            .OrderByDescending(x => x.LastModified)
            .Skip(0)
            .Take(count)
            .ToListAsync();
    }
}
