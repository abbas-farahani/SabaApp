using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Core.Domain.Entities;
using System.Security.Claims;

namespace Core.Domain.Contracts.Services.Identity;

public interface IUserService
{
    Task<List<User>> GetAll(CancellationToken cancellationToken);
    Task<User> GetById(string userId);
    Task<User> GetByUserName(string userName);
    Task<User> GetByEmail(string email);
    Task<User> GetByPhone(string phone);
    Task<bool> GetAnyByUserName(string userName);
    Task<bool> GetAnyByEmail(string email);
    Task<bool> GetAnyByPhone(string phone);
    Task<List<string>> GetRoles(User user);
    Task<IList<Claim>> GetClaims(User user);
    Task<string> GetPasswordTokenEncoded(User user, CancellationToken cancellationToken);
    Task<IdentityResult> ResetPassword(User user, string token, string newPassword);
    Task<SignInResult> SignInUser(string userName, string password, bool remember, bool lookout);
    Task RefreshSignIn(User user);
    Task SignOutUser();
	Task<IdentityResult> CreateUser(User user, string password);
    Task<string> GenerateEmailConfirmationToken(User user);
    Task<string> GenerateTwoFactorToken(User user, string provider);

    Task<IdentityResult> ConfirmEmail(User user, string token);
    Task<bool> ConfirmPhone(User user, string token);
    Task<IdentityResult> UpdateUser(User user);
    Task<IdentityResult> UpdateUserWithClaims(User user, List<Claim> claims);

    Task<IdentityResult> AddToRole(User user, List<string> roles);
	Task<IdentityResult> RemoveRole(User user, List<string> roles);
    Task<IdentityResult> DeleteUser(User user);
}
