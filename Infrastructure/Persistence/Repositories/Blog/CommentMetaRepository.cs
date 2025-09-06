using Core.Domain.Contracts.Repositories;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories;

public class CommentMetaRepository : BaseRepository<CommentMeta>, ICommentMetaRepository
{
    public CommentMetaRepository(AppDbContext context) : base(context)
    {
    }
}