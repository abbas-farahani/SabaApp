using Core.Domain.Entities;

namespace Core.Domain.Contracts.Services;

public interface ICategoryService : IBaseService<Category>
{
    Task<Category> GetBySlug(string slug);
    Task<bool> IsExistBySlug(string slug);
}
