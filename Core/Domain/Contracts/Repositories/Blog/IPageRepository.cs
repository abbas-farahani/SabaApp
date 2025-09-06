using Core.Domain.Entities;

namespace Core.Domain.Contracts.Repositories;

public interface IPageRepository : IBaseRepository<Page>
{
    Task<List<Page>> GetAllAsync(string lang);
    Task<List<Page>> GetByUserId(string userId);
    Task<Page> GetBySlug(string slug);
    Task<List<Page>> GetOriginalPages(string defaultLanguage);
    Task<bool> IsExistBySlug(string slug);
    Task<bool> IsExistById(int id);

}

