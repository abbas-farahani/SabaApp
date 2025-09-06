using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;

namespace Core.Domain.Contracts.Services;

public interface IPageService : IBaseService<Page>
{
    Task<List<PageDto>> GetAllAsync(string lang);
    Task<List<PageDto>> GetByUserId(string userId);
    Task<PageDto> GetById(int id);
    Task<PageDto> GetBySlug(string slug);
    Task<List<PageDto>> GetOriginalPages(string defaultLanguage);
    Task<PageDto> GetFullItemByIdAsync(int id);

    Task<bool> IsExistBySlug(string slug);
    Task<bool> IsExistById(int id);

    Task<PageDto> AddAsync(PageDto pageDto);
    Task<PageDto> UpdateAsync(PageDto pageDto);
}
