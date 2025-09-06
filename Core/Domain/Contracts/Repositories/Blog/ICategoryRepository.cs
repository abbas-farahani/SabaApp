using Core.Domain.Entities;

namespace Core.Domain.Contracts.Repositories;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category> GetBySlug(string slug);
    Task<bool> IsExistBySlug(string slug);
}