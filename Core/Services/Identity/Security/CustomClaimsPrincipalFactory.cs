using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts.Repositories.Identity;
using Core.Domain.Contracts.Services.Identity.Security;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Core.Services.Identity.Security;

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>, ICustomClaimsPrincipalFactory
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IRoleRepository _roleRepository;

    public CustomClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<Role> roleManager, IRoleRepository roleRepository, IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _roleRepository = roleRepository;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            var roleEntity = await _roleManager.FindByNameAsync(role);
            if (roleEntity != null)
            {
                if (roleEntity.Name.ToLower() == "admin")
                {
                    identity.AddClaim(new Claim("IsAdmin", "IsAdmin"));
                }
                else
                {
                    // گرفتن مجوزهای نقش
                    var permissions = await _roleRepository.GetPermissionsNameByRole(roleEntity.Id);
                    foreach (var permission in permissions)
                    {
                        identity.AddClaim(new Claim("Permission", permission));
                    }
                }
            }
        }
        var userClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in userClaims)
        {
            identity.AddClaim(claim);
        }
        return identity;
    }
}