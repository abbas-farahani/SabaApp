using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Enums;
using Core.Domain.Entities;
using Core.Domain.Dtos.Blog;

namespace Core.Domain.Mapping;

public class PostDtoMapping : Profile
{
    public PostDtoMapping()
    {
        // Dto → Entity
        CreateMap<PostDto, Post>()
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => Enum.Parse<PostStatus>(src.Status, true))).ReverseMap();

        // Entity → Dto
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => src.Status.ToString().ToLower())).ReverseMap();
    }
}
