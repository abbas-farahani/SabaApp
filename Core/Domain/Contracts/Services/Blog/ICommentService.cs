using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services;

public interface ICommentService : IBaseService<Comment>
{
    Task<IEnumerable<CommentDto>> GetAllDtoAsync();
    Task<CommentDto> GetById(int id);
    Task<CommentDto> AddDtoAsync(CommentDto commentDto);
    Task<CommentDto> UpdateDtoAsync(CommentDto commentDto);
    Task<bool> IsExistById(int id);
}
