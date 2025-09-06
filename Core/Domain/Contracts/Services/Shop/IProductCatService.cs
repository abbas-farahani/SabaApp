using Core.Domain.Dtos.Shop;
using Core.Domain.Entities.Shop;

namespace Core.Domain.Contracts.Services.Shop;

public interface IProductCatService : IBaseService<ProductCat>
{
    Task<ProductCat> AddAsync(ProductCatDto cat);
    Task<ProductCat> GetBySlug(string slug);
    Task<List<ProductCat>> GetByIdAsync(List<int> categoryIds);
    Task<bool> IsExistBySlug(string slug);
}
