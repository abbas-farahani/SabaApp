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

public class ProductMetaRepository : BaseRepository<ProductMeta>, IProductMetaRepository
{
    private readonly AppDbContext _context;
    public ProductMetaRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ProductMeta> GetByIdAsync(int productId, string metaName)
    {
        return await _context.ProductMetas.FirstOrDefaultAsync(x => x.ProductId == productId && x.MetaName == metaName);
    }

}
