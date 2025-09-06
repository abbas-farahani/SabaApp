using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;

namespace Core.Domain.Contracts.Repositories.Shop;

public interface IProductMetaRepository : IBaseRepository<ProductMeta>
{
    Task<ProductMeta> GetByIdAsync(int productId, string metaName);
}
