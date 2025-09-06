using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Dtos.Blog;
using MVC.Areas.Admin.Models.ViewModels.Post;

namespace MVC.Areas.Admin.Mapping;

public class PostViewModelMapping : Profile
{
    public PostViewModelMapping()
    {
        // ViewModel → Dto
        CreateMap<PostVM, PostDto>()
            .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => src.Status.ToLower())).ReverseMap();

        // Dto → ViewModel
        CreateMap<PostDto, PostVM>()
            .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => src.Status.ToLower())).ReverseMap();
    }
}
