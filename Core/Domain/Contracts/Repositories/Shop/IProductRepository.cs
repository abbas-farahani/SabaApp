using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;

namespace Core.Domain.Contracts.Repositories.Shop;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<List<Product>> GetAllAsync(string lang);
    Task<List<Product>> GetList(int count, string? culture);
    Task<Product> GetBySlug(string slug);
    Task<List<Product>> GetOriginalProducts(string defaultLanguage);
    Task<bool> IsExistBySlug(string slug);
}
