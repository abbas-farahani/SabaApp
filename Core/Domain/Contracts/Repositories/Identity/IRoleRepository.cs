using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Repositories.Identity;

public interface IRoleRepository : IBaseRepository<Role>
{
	Task<IdentityResult> AddAsync(Role role, List<Permission> permissions);
	Task<IdentityResult> UpdateAsync(Role role, List<Permission> permissions);
	Task<Role> GetByIdAsync(string roleId);
	Task<Role> GetByNameAsync(string roleName);
	Task DeleteAsync(string roleId);

	Task<List<Claim>> GetRoleClaims(Role role);

	Task<List<string>> GetPermissionsNameByRole(string roleId);
	Task<List<string>> GetPermissionsNameByRole(Role role);
}
