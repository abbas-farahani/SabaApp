using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;

namespace Core.Domain.Contracts.Services.Shop;

public interface IProductMetaService : IBaseService<ProductMeta>
{
    Task<ProductMeta> GetByIdAsync(int productId, string metaName);
    Task<bool> AddTranslationMeta(Product currentProduct);
    Task<bool> UpdateTranslationMeta(Product currentProduct, Product parentProduct);
}
