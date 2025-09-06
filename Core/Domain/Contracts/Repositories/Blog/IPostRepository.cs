using Core.Domain.Dtos;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Repositories;

public interface IPostRepository : IBaseRepository<Post>
{
    Task<List<Post>> GetAllAsync(string lang);
    Task<List<Post>> GetByUserId(string userId, CancellationToken cancellationToken);
    Task<List<Post>> GetList(int count, string? culture);
    Task<Post> GetBySlug(string slug, CancellationToken cancellationToken);
    Task<List<Post>> GetOriginalProducts(string defaultLanguage);
    Task<bool> IsExistBySlug(string slug);
}
