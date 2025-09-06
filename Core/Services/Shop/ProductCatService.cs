using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Repositories.Shop;
using Core.Domain.Contracts.Services.Shop;
using Core.Domain.Dtos.Shop;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;

namespace Core.Services.Shop;

public class ProductCatService : BaseService<ProductCat>, IProductCatService
{
    private readonly IProductCatRepository _repository;

    public ProductCatService(IProductCatRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public Task<ProductCat> AddAsync(ProductCatDto cat)
    {
        try
        {
            var catrgory = new ProductCat
            {
                ParentId = cat.ParentId != null ? cat.ParentId.Value : 0,
                UserId = cat.UserId,
                Label = cat.Label,
                Slug = cat.Slug,
                Description = cat.Description
            };
            return _repository.CreateAsync(catrgory);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<ProductCat>> GetByIdAsync(List<int> categoryIds)
    {
        return await _repository.GetByIdAsync(categoryIds);
    }

    public async Task<ProductCat> GetBySlug(string slug)
    {
        return await _repository.GetBySlug(slug);

    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _repository.IsExistBySlug(slug);
    }
}
