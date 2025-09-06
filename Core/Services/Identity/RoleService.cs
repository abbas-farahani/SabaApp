using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Repositories.Identity;
using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Identity;

public class RoleService : BaseService<Role>, IRoleService
{
    private readonly IRoleRepository _RoleRepository;
    private readonly IPermissionService _permissionService;

    public RoleService(IRoleRepository RoleRepository, IPermissionService permissionService) : base(RoleRepository)
    {
        _RoleRepository = RoleRepository;
        _permissionService = permissionService;
    }

    public async Task DeleteAsync(string roleId)
    {
        await _RoleRepository.DeleteAsync(roleId);
    }

    public override Task<Role> GetByIdAsync(int id)
    {
        return null;
    }

    public async Task<Role> GetByIdAsync(string roleId)
    {
        return await _RoleRepository.GetByIdAsync(roleId);
    }

    public async Task<Role> GetByNameAsync(string roleName)
    {
        return await _RoleRepository.GetByNameAsync(roleName);
    }

    public override Task<Role> AddAsync(Role entity)
    {
        return null;
    }

    public async Task<IdentityResult> AddAsync(Role role, List<int> permissionIds)
    {
        var getPermissions = await _permissionService.GetAllAsync();
        var permissions = getPermissions.Where(x => permissionIds.Contains(x.Id)).ToList();
        //role.Permissions = permissions;
        var addRole = await _RoleRepository.AddAsync(role, permissions);
        return addRole;
    }

    public async Task<IdentityResult> UpdateAsync(Role role, List<int> permissionIds)
    {
        #region Update Role
        var existRole = await this.GetByIdAsync(role.Id);
        existRole.Name = role.Name;
        existRole.Description = role.Description;
        //existRole.Permissions.Clear();
        #endregion

        #region Get Old Permissions
        var getPermissions = await _permissionService.GetAllAsync();
        var permissions = getPermissions.Where(x => permissionIds.Contains(x.Id)).ToList();
        //existRole.Permissions = permissions;
        #endregion

        //getPermissions.Select(x =>
        //{
        //    if (permissionIds.Contains(x.Id))
        //        existRole.Permissions.Add(x);
        //    return x;
        //}).ToList();

        return await _RoleRepository.UpdateAsync(existRole, permissions);
    }

    public async Task<List<Claim>> GetRoleClaims(Role role)
    {
        return await _RoleRepository.GetRoleClaims(role);
    }
}
