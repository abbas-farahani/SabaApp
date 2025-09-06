using Core.Domain.Contracts.Repositories.Identity;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Identity;

public class UserService : IUserService
{
    #region Injection & Constructor
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    public UserService(IUserRepository userRepository, IUserClaimRepository userClaimRepository)
    {
        _userRepository = userRepository;
        _userClaimRepository = userClaimRepository;
    }
    #endregion


    #region Methods
    public async Task<IdentityResult> AddToRole(User user, List<string> roles)
    {
        return await _userRepository.AddToRole(user, roles);
    }

    public async Task<IdentityResult> ConfirmEmail(User user, string token)
    {
        return await _userRepository.ConfirmEmail(user, token);
    }

    public async Task<bool> ConfirmPhone(User user, string token)
    {
        return await _userRepository.ConfirmPhone(user, token);
    }

    public async Task<IdentityResult> CreateUser(User user, string password)
    {
        var result = await _userRepository.CreateUser(user, password);
        return result;
    }

    public async Task<IdentityResult> DeleteUser(User user)
    {
        return await _userRepository.DeleteUser(user);
    }

    public async Task<string> GenerateEmailConfirmationToken(User user)
    {
        var token = await _userRepository.GenerateEmailConfirmationToken(user);
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    }

    public async Task<string> GenerateTwoFactorToken(User user, string provider)
    {
        return await _userRepository.GenerateTwoFactorToken(user, provider);
    }

    public async Task<List<User>> GetAll(CancellationToken cancellationToken)
    {
        return await _userRepository.GetAll(cancellationToken);
    }

    public async Task<bool> GetAnyByEmail(string email)
    {
        return await _userRepository.GetAnyByEmail(email);
    }

    public async Task<bool> GetAnyByPhone(string phone)
    {
        if (phone.Length > 0) return await _userRepository.GetAnyByPhone(phone);
        else return true;
    }

    public async Task<bool> GetAnyByUserName(string userName)
    {
        return await _userRepository.GetAnyByUserName(userName);
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _userRepository.GetByEmail(email);
    }

    public async Task<User> GetById(string userId)
    {
        return await _userRepository.GetById(userId);
    }

    public async Task<User> GetByPhone(string phone)
    {
        return await _userRepository.GetByPhone(phone);
    }

    public async Task<User> GetByUserName(string userName)
    {
        return await _userRepository.GetByUserName(userName);
    }

    public async Task<IList<Claim>> GetClaims(User user)
    {
        var add = await _userClaimRepository.AddUserClaim(user, new Claim("Avatar", "/media/avatars/300-11.jpg"));
        return await _userRepository.GetClaims(user);
    }

    public async Task<string> GetPasswordTokenEncoded(User user, CancellationToken cancellationToken)
    {
        return await _userRepository.GetPasswordTokenEncoded(user);
    }

    public async Task<List<string>> GetRoles(User user)
    {
        return await _userRepository.GetRoles(user);
    }

    public async Task RefreshSignIn(User user)
    {
        await _userRepository.RefreshSignIn(user);
    }

    public async Task<IdentityResult> RemoveRole(User user, List<string> roles)
    {
        return await _userRepository.RemoveRole(user, roles);
    }

    public async Task<IdentityResult> ResetPassword(User user, string token, string newPassword)
    {
        return await _userRepository.ResetPassword(user, token, newPassword);
    }

    public async Task<SignInResult> SignInUser(string userName, string password, bool remember, bool lookout)
    {
        return await _userRepository.SignInUser(userName, password, remember, lookout);
    }

    public async Task SignOutUser()
    {
        await _userRepository.SignOutUser();
    }

    public async Task<IdentityResult> UpdateUser(User user)
    {
        return await _userRepository.UpdateUser(user);
    }

    public async Task<IdentityResult> UpdateUserWithClaims(User user, List<Claim> claims)
    {
        var updateUser = await this.UpdateUser(user);
        if (updateUser != null && updateUser.Succeeded)
        {
            var result = await _userClaimRepository.UpdateUserClaim(user, claims.FirstOrDefault());
            if (result.Succeeded)
            {
                foreach (var claim in claims)
                {
                    await _userClaimRepository.UpdateUserClaim(user, claim);
                }
            }
            return result;
        }
        return updateUser;
    }
    #endregion
}
