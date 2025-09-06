using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Contracts.Repositories.Identity;

public interface IUserClaimRepository
{
    Task<IdentityResult?> AddUserClaim(User user, Claim claim);
    Task<Claim?> GetUserClaim(User user, string name);
    Task<List<Claim>> GetUserClaims(User user);
    Task<IdentityResult?> UpdateUserClaim(User user, Claim claim);
    Task<IdentityResult?> RemoveUserClaim(User user, Claim claim);
}
