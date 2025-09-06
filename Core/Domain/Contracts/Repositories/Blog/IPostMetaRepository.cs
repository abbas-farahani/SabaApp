using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Repositories;

public interface IPostMetaRepository : IBaseRepository<PostMeta>
{
    Task<PostMeta> GetByIdAsync(int postId, string metaName);
    Task<IEnumerable<PostMeta>> AddAsync(IEnumerable<PostMeta> metas);
}
