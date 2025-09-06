using AutoMapper;
using Core.Domain.Dtos.Blog;
using MVC.Areas.Admin.Models.ViewModels;

namespace MVC.Areas.Admin.Mapping;

public class PageViewModelMapping : Profile
{
    public PageViewModelMapping()
    {
        CreateMap<PageDto, PageVM>().ReverseMap();
    }
}
