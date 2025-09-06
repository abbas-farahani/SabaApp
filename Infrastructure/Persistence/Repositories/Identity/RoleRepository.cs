using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Repositories.Identity;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories.Identity;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    private readonly RoleManager<Role> _roleManager;
    private readonly AppDbContext _context;
    public RoleRepository(AppDbContext context, RoleManager<Role> roleManager) : base(context)
    {
        _context = context;
        _roleManager = roleManager;
    }
    public async Task<Role> GetByIdAsync(string roleId)
    {
        //var role = await _context.Roles.AsNoTracking().Where(x => x.Id == roleId).Include(x => x.RoleClaims).FirstOrDefaultAsync();
        //return role;
        return await _roleManager.FindByIdAsync(roleId);
    }

    public async Task<Role> GetByNameAsync(string roleName)
    {
        //var role = await _context.Roles.AsNoTracking().Where(x => x.Name == roleName).Include(x => x.RoleClaims).FirstOrDefaultAsync();
        //return role;
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task DeleteAsync(string roleId)
    {
        var role = await this.GetByIdAsync(roleId);
        await RemoveAllClaims(role);
        await _roleManager.DeleteAsync(role);
    }

    public async Task<IdentityResult> AddAsync(Role role, List<Permission> permissions)
    {
        var result = await _roleManager.CreateAsync(role);
        if (result.Succeeded) await AddPemissionClaims(role, permissions);
        return result;
    }

    public async Task<IdentityResult> UpdateAsync(Role role, List<Permission> permissions)
    {
        //_context.ChangeTracker.Clear();
        await RemovePemissionClaims(role);
        await AddPemissionClaims(role, permissions);
        return await _roleManager.UpdateAsync(role);
    }



    public async Task<List<Claim>> GetRoleClaims(Role role)
    {
        return (await _roleManager.GetClaimsAsync(role)).ToList();
    }


    public async Task<List<string>> GetPermissionsNameByRole(string roleId)
    {
        //var role = await _context.Roles.Where(r => r.Id == roleId).Include(x => x.Permissions).FirstOrDefaultAsync();
        var role = await _roleManager.FindByIdAsync(roleId);
        var claims = await _roleManager.GetClaimsAsync(role);
        return claims.Select(x => x.Value).ToList();
    }

    public async Task<List<string>> GetPermissionsNameByRole(Role role)
    {
        await _context.Entry(role).Collection(r => r.Permissions).LoadAsync();
        var m = role.Permissions.Select(x => x.Name).ToList();
        return m;
    }




    private async Task AddPemissionClaims(Role role, List<Permission> permissions)
    {
        foreach (var permission in permissions)
        {
            await _roleManager.AddClaimAsync(role, new Claim("Permission", permission.Name));
        }
    }

    private async Task RemovePemissionClaims(Role role)
    {
        //var existingRoles = _context.ChangeTracker.Entries<Role>()
        //                    .Select(e => e.Entity)
        //                    .ToList();

        //foreach (var mrole in existingRoles)
        //{
        //    _context.Entry(mrole).State = EntityState.Detached;
        //}

        var claims = await _roleManager.GetClaimsAsync(role);
        if (claims == null || claims.Count() == 0) return;
        claims = claims.Where(c => c.Type == "Permission").ToList();
        foreach (var claim in claims)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }
    }

    private async Task RemoveAllClaims(Role role)
    {
        var claims = await _roleManager.GetClaimsAsync(role);
        if (claims == null || claims.Count() == 0) return;
        foreach (var claim in claims)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }
    }
}
