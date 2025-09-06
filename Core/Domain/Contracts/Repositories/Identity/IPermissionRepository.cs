using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Repositories.Identity;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task AddAsync(List<Permission> permissions);
    Task<List<Permission>> GetAllByName(string name);
}
