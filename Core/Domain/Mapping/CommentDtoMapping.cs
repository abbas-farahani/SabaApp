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

public class CommentDtoMapping: Profile
{
    public CommentDtoMapping()
    {
        // Dto → Entity
        CreateMap<CommentDto, Comment>().ReverseMap();

        // Entity → Dto
        CreateMap<Comment, CommentDto>().ReverseMap();
    }
}
