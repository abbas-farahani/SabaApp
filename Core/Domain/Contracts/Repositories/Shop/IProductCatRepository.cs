using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;

namespace Core.Domain.Contracts.Repositories.Shop;

public interface IProductCatRepository : IBaseRepository<ProductCat>
{
    Task<ProductCat> AddAsync(ProductCat cat);
    Task<ProductCat> GetBySlug(string slug);
    Task<List<ProductCat>> GetByIdAsync(List<int> categoryIds);
    Task<bool> IsExistBySlug(string slug);
}
