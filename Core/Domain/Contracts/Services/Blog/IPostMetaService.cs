using Core.Domain.Entities;
using Core.Domain.Entities.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services;

public interface IPostMetaService : IBaseService<PostMeta>
{
    Task<PostMeta> GetByIdAsync(int postId, string metaName);
    Task<IEnumerable<PostMeta>> AddAsync(IEnumerable<PostMeta> metas);
    Task<bool> AddTranslationMeta(Post currentPost);
    Task<bool> UpdateTranslationMeta(Post currentPost, Post parentPost);
}
