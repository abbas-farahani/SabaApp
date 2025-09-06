using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Domain.Mapping;

public class PageDtoMapping : Profile
{
    public PageDtoMapping()
    {
        #region Page → PageDto
        CreateMap<Page, PageDto>()
            // BaseEntity
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))

            // Page Entity
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))

            // UserName
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))

            // Meta Values - با متد کمکی
            .ForMember(dest => dest.SliderId, opt => opt.MapFrom(src => GetMetaInt(src.PageMetas, "Slider")))
            .ForMember(dest => dest.EnableBreadCrumb, opt => opt.MapFrom(src => GetMetaBool(src.PageMetas, "BreadCrumb", true)))
            .ForMember(dest => dest.EnableFooter, opt => opt.MapFrom(src => GetMetaBool(src.PageMetas, "Footer", true)))
            .ForMember(dest => dest.EnableCopyRight, opt => opt.MapFrom(src => GetMetaBool(src.PageMetas, "CopyRight", true)))
            .ForMember(dest => dest.OriginalPage, opt => opt.Ignore()) // Useable for View - Initialize later
            .ForMember(dest => dest.LanguagesList, opt => opt.MapFrom(src => GetMetaString(src.PageMetas, "LanguageCode")))
            .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => GetMetaString(src.PageMetas, "Translations")))

            // Status int → string
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(PostStatus), src.Status)));
        #endregion


        #region PageDto → Page
        CreateMap<PageDto, Page>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0))
            .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime ?? DateTime.Now))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))

            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<PostStatus>(src.Status, true)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.PageMetas, opt => opt.Ignore()); // متاها رو دستی می‌سازیم
        #endregion
    }


    #region Helper Methods for Meta Extraction
    private static string GetMetaString(IEnumerable<PageMeta> metas, string key)
    {
        return metas?.FirstOrDefault(m => m.MetaName == key)?.MetaValue;
    }

    private static bool? GetMetaBool(IEnumerable<PageMeta> metas, string key, bool defaultValue = false)
    {
        var value = GetMetaString(metas, key);
        if (string.IsNullOrEmpty(value)) return defaultValue;
        return value.ToLower() == "true";
    }

    private static int? GetMetaInt(IEnumerable<PageMeta> metas, string key)
    {
        var value = GetMetaString(metas, key);
        if (int.TryParse(value, out int result))
            return result;
        return null;
    }
    #endregion
}
