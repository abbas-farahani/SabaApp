using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services.Identity;

public interface IRoleService : IBaseService<Role>
{
	Task<IdentityResult> AddAsync(Role role, List<int> permissionIds);
	Task<IdentityResult> UpdateAsync(Role role, List<int> permissionIds);
	Task<Role> GetByIdAsync(string roleId);
	Task<Role> GetByNameAsync(string roleName);
    Task DeleteAsync(string roleId);


    Task<List<Claim>> GetRoleClaims(Role role);
}
