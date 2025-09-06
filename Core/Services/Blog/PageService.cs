using AutoMapper;
using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;

namespace Core.Services;

public class PageService : BaseService<Page>, IPageService
{
    private readonly IPageRepository _pageRepository;
    private readonly IMapper _mapper;

    public PageService(IPageRepository pageRepository, IMapper mapper) : base(pageRepository)
    {
        _pageRepository = pageRepository;
        _mapper = mapper;
    }

    public async Task<List<PageDto>> GetAllAsync(string lang)
    {
        var pages = await _pageRepository.GetAllAsync(lang);
        return _mapper.Map<List<Page>, List<PageDto>>(pages);
    }

    public async Task<PageDto> GetBySlug(string slug)
    {
        var page = await _pageRepository.GetBySlug(slug);
        return _mapper.Map<Page, PageDto>(page);
    }

    public async Task<List<PageDto>> GetByUserId(string userId)
    {
        var pages = await _pageRepository.GetByUserId(userId);
        return _mapper.Map<List<Page>, List<PageDto>>(pages);
    }

    public async Task<PageDto> GetById(int id)
    {
        var page = await GetByIdAsync(id);
        return _mapper.Map<Page, PageDto>(page);
    }

    public async Task<PageDto> GetFullItemByIdAsync(int id)
    {
        var page = await _pageRepository.GetByIdAsync(id);
        return _mapper.Map<Page, PageDto>(page);
    }

    public async Task<List<PageDto>> GetOriginalPages(string defaultLanguage)
    {
        var pages = await _pageRepository.GetOriginalPages(defaultLanguage);
        return _mapper.Map<List<Page>, List<PageDto>>(pages);
    }

    public async Task<PageDto> AddAsync(PageDto pageDto)
    {
        try
        {
            #region Page
            Page page = _mapper.Map<PageDto, Page>(pageDto);
            #endregion

            #region Page Metas
            if (pageDto.SliderId != null && pageDto.SliderId.Value != 0)
                page.PageMetas.Add(new PageMeta { MetaName = "Slider", MetaValue = pageDto.SliderId.Value.ToString() });
            if (pageDto.EnableBreadCrumb != null)
                page.PageMetas.Add(new PageMeta { MetaName = "BreadCrumb", MetaValue = pageDto.EnableBreadCrumb.Value ? "true" : "false" });
            if (pageDto.EnableFooter != null)
                page.PageMetas.Add(new PageMeta { MetaName = "Footer", MetaValue = pageDto.EnableFooter.Value ? "true" : "false" });
            if (pageDto.EnableCopyRight != null)
                page.PageMetas.Add(new PageMeta { MetaName = "CopyRight", MetaValue = pageDto.EnableCopyRight.Value ? "true" : "false" });
            if (!string.IsNullOrEmpty(pageDto.LanguagesList))
                page.PageMetas.Add(new PageMeta { MetaName = "LanguageCode", MetaValue = pageDto.LanguagesList });
            #endregion

            var add = await this.AddAsync(page);
            if (add != null) return _mapper.Map<Page, PageDto>(page);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task<PageDto> UpdateAsync(PageDto pageDto)
    {
        try
        {
            #region Page
            Page page = await this.GetByIdAsync(pageDto.Id.Value); //_mapper.Map<PageDto, Page>(pageDto);
            _mapper.Map(pageDto, page); // Import updated properties from DTO to Entity
            #endregion

            #region Page Metas
            page.PageMetas.Clear();
            if (pageDto.SliderId != null && pageDto.SliderId.Value != 0)
                page.PageMetas.Add(new PageMeta { MetaName = "Slider", MetaValue = pageDto.SliderId.Value.ToString() });

            if (pageDto.EnableBreadCrumb != null)
                page.PageMetas.Add(new PageMeta { MetaName = "BreadCrumb", MetaValue = pageDto.EnableBreadCrumb.Value ? "true" : "false" });
            if (pageDto.EnableFooter != null)
                page.PageMetas.Add(new PageMeta { MetaName = "Footer", MetaValue = pageDto.EnableFooter.Value ? "true" : "false" });
            if (pageDto.EnableCopyRight != null)
                page.PageMetas.Add(new PageMeta { MetaName = "CopyRight", MetaValue = pageDto.EnableCopyRight.Value ? "true" : "false" });
            if (!string.IsNullOrEmpty(pageDto.LanguagesList))
                page.PageMetas.Add(new PageMeta { MetaName = "LanguageCode", MetaValue = pageDto.LanguagesList });
            #endregion

            var update = await this.UpdateAsync(page);
            if (update != null) return _mapper.Map<Page, PageDto>(page);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _pageRepository.IsExistBySlug(slug);
    }

    public async Task<bool> IsExistById(int id)
    {
        return await _pageRepository.IsExistById(id);
    }
}
