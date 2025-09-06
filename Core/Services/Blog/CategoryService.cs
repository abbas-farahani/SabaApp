using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Entities;

namespace Core.Services;

public class CategoryService : BaseService<Category>, ICategoryService
{
	private readonly ICategoryRepository _categoryRepository;

	public CategoryService(ICategoryRepository categoryRepository) : base(categoryRepository)
	{
		_categoryRepository = categoryRepository;
	}

    public async Task<Category> GetBySlug(string slug)
    {
        return await _categoryRepository.GetBySlug(slug);
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _categoryRepository.IsExistBySlug(slug);
    }
}