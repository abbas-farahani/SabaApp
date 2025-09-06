using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts.Repositories.Identity;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Infra.Persistence.Repositories.Identity;

public class UserClaimRepository : IUserClaimRepository
{
    #region Injection & Constructor
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserClaimRepository(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    #endregion


    #region Methods
    public async Task<IdentityResult?> AddUserClaim(User user, Claim claim)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var existingClaim = claims.FirstOrDefault(c => c.Type == claim.Type);
        if (existingClaim != null)
            return await _userManager.AddClaimAsync(user, claim);
        return null;
    }

    public async Task<Claim?> GetUserClaim(User user, string name)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var claim = claims.FirstOrDefault(c => c.Type == name);
        if (claim != null) return claim;
        else return null;
    }

    public async Task<List<Claim>> GetUserClaims(User user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        return claims.ToList();
    }

    public async Task<IdentityResult?> UpdateUserClaim(User user, Claim claim)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var existingClaim = claims.FirstOrDefault(c => c.Type == claim.Type);
        if (existingClaim != null)
            return await _userManager.ReplaceClaimAsync(user, existingClaim, claim);
        else
            return await _userManager.AddClaimAsync(user, claim);
    }

    public async Task<IdentityResult?> RemoveUserClaim(User user, Claim claim)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var existingClaim = claims.FirstOrDefault(c => c.Type == claim.Type);
        if (existingClaim != null)
            return await _userManager.RemoveClaimAsync(user, existingClaim);
        else
            return null;
    }
    #endregion
}
