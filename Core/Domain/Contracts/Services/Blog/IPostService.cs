using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services;

public interface IPostService : IBaseService<Post>
{
    Task<List<Post>> GetAllAsync(string lang);
    Task<List<Post>> GetByUserId(string userId, CancellationToken cancellationToken);
    Task<List<Post>> GetList(int count, string? culture); // columns, 
    Task<Post> GetBySlug(string slug, CancellationToken cancellationToken);

    Task<PostDto> GetFullItemByIdAsync(int id);
    Task<List<Post>> GetOriginalProducts(string defaultLanguage);

    Task<bool> IsExistBySlug(string slug);
    Task<Post> AddAsync(PostDto postDto);
    Task<Post> UpdateAsync(PostDto postDto);
}
