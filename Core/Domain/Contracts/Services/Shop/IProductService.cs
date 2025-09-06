using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Dtos.Shop;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;

namespace Core.Domain.Contracts.Services.Shop;

public interface IProductService : IBaseService<Product>
{

    Task<List<Product>> GetAllAsync(string lang);
    Task<List<Product>> GetList(int count, string? culture);
    Task<Product> AddAsync(ProductDto productDto);
    Task<Product> GetBySlug(string slug);
    Task<List<Product>> GetOriginalProducts(string defaultLanguage);
    Task<Product> UpdateAsync(ProductDto productDto);
    Task<bool> IsExistBySlug(string slug);
}
