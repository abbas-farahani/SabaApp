using AngleSharp.Io;
using AutoMapper;
using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.HtmlSanitizer;

namespace Core.Services;

public class CommentService : BaseService<Comment>, ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepository, IMapper mapper) : base(commentRepository)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentDto>> GetAllDtoAsync()
    {
        var comments = await _commentRepository.GetAllAsync();
        return _mapper.Map<List<CommentDto>>(comments);
    }
    public async Task<CommentDto> GetById(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> AddDtoAsync(CommentDto commentDto)
    {
        try
        {
            Comment comment = new Comment()
            {
                LastModified = DateTime.Now,
                PostId = commentDto.PostId,
                UserId = string.IsNullOrEmpty(commentDto.UserId) ? "" : commentDto.UserId,
                UserName = commentDto.UserName,
                Email = commentDto.Email,
                Website = commentDto.Website ?? "",
                Content = TextSanitizer.Sanitize(commentDto.Content),
                Status = commentDto.Status,
                Agent = commentDto.Agent,
                Ip = commentDto.Ip
            };
            var result = await _commentRepository.CreateAsync(comment);
            return _mapper.Map<CommentDto>(result);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<CommentDto> UpdateDtoAsync(CommentDto commentDto)
    {
        try
        {
            var comment = new Comment()
            {
                Id = commentDto.Id.Value,
                CreationTime = DateTime.Now,
                PostId = commentDto.PostId,
                ParentId = commentDto.ParentId,
                UserId = commentDto.UserId,
                UserName = commentDto.UserName,
                Email = commentDto.Email,
                Website = commentDto.Website,
                Ip = commentDto.Ip,
                Agent = commentDto.Agent,
                Content = TextSanitizer.Sanitize(commentDto.Content),
                Status = 0,
            };
            var result = await _commentRepository.UpdateAsync(comment);
            return _mapper.Map<CommentDto>(result);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> IsExistById(int id)
    {
        return await _commentRepository.IsExistById(id);
    }
}
