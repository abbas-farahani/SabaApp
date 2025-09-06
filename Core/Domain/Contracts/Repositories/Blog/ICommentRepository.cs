using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Repositories;

public interface ICommentRepository : IBaseRepository<Comment>
{
    Task<bool> IsExistById(int id);

}
