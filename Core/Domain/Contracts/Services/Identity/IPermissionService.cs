using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services.Identity;

public interface IPermissionService : IBaseService<Permission>
{
    Task AddAsync(List<Permission> permissions);
    Task<List<Permission>> GetAllByName(string name);
    Task<List<Permission>> GetAllById(int id);
}
