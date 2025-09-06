using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class CommentMetaService : BaseService<CommentMeta>, ICommentMetaService
{
    private readonly ICommentMetaRepository _commentMetaRepository;

    public CommentMetaService(ICommentMetaRepository commentMetaRepository) : base(commentMetaRepository)
    {
        _commentMetaRepository = commentMetaRepository;
    }
}
