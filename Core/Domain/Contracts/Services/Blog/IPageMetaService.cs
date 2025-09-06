using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;

namespace Core.Domain.Contracts.Services;

public interface IPageMetaService : IBaseService<PageMeta>
{
    Task<PageMeta> GetByIdAsync(int pageId, string metaName);
    Task<bool> AddTranslationMeta(PageDto currentPage);
    Task<bool> UpdateTranslationMeta(PageDto currentPage, PageDto parentPage);

}
