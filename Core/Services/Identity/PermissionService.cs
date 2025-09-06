using Core.Domain.Contracts.Repositories.Identity;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Identity;

public class PermissionService : BaseService<Permission>, IPermissionService
{
	private readonly IPermissionRepository _permissionRepository;
    public PermissionService(IPermissionRepository permissionRepository) : base(permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task AddAsync(List<Permission> permissions)
    {
        var newItems = new List<Permission>();
        var all = await this.GetAllAsync();
        foreach (var permission in permissions)
        {
            if(!all.Any(x => x.Name == permission.Name))
            {
                newItems.Add(permission);
            }
        }
        if (newItems.Count > 0)
        {
            await _permissionRepository.AddAsync(newItems);
        }
    }

    public async Task<List<Permission>> GetAllById(int id)
    {
        var permission = await this.GetByIdAsync(id);
        var name = permission.Name.Split('.')[0];
        return await this.GetAllByName(name);
    }

    public async Task<List<Permission>> GetAllByName(string name)
    {
        return await _permissionRepository.GetAllByName(name);
    }
}
