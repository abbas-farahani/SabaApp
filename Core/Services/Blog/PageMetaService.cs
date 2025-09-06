using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;

namespace Core.Services;

public class PageMetaService : BaseService<PageMeta>, IPageMetaService
{
    private readonly IPageMetaRepository _pageMetaRepository;

    public PageMetaService(IPageMetaRepository pageMetaRepository) : base(pageMetaRepository)
    {
        _pageMetaRepository = pageMetaRepository;
    }

    public async Task<PageMeta> GetByIdAsync(int pageId, string metaName)
    {
        return await _pageMetaRepository.GetByIdAsync(pageId, metaName);
    }

    public async Task<bool> AddTranslationMeta(PageDto currentPage)
    {
        try
        {
            var dictionary = new Dictionary<string, int>();
            dictionary.Add(currentPage.LanguagesList, currentPage.Id.Value);

            var newMeta = new PageMeta
            {
                PageId = currentPage.Id.Value,
                MetaName = "Translation",
                MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary)
            };
            var result = await AddAsync(newMeta);
            if (result == null) return false;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateTranslationMeta(PageDto currentPage, PageDto parentPage)
    {
        try
        {
            var dictionary = new Dictionary<string, int>();
            //var parentPage = await _productService.GetByIdAsync(parentPageId);
            if (parentPage.Translations != null)
            {
                dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(parentPage.Translations);
                dictionary.Add(currentPage.LanguagesList, currentPage.Id.Value);
            }
            else
            {
                dictionary.Add(parentPage.LanguagesList, parentPage.Id.Value);
                dictionary.Add(currentPage.LanguagesList, currentPage.Id.Value);
            }

            var ids = dictionary.Values.ToList();
            foreach (var id in ids)
            {
                var translationMeta = await GetByIdAsync(id, "Translation");
                if (translationMeta != null)
                {
                    translationMeta.MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
                    var result = await UpdateAsync(translationMeta);
                    if (result == null) return false;
                }
                else
                {
                    var newMeta = new PageMeta
                    {
                        PageId = id,
                        MetaName = "Translation",
                        MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary)
                    };
                    var result = await AddAsync(newMeta);
                    if (result == null) return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
