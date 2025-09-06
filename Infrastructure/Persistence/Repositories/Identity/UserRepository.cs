using Infra.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts.Repositories.Identity;
using System.Security.Claims;

namespace Infra.Persistence.Repositories.Identity;

public class UserRepository : IUserRepository
{
    #region Injection & Constructor
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;
    public UserRepository(AppDbContext context, UserManager<User> userManager,
        RoleManager<Role> roleManager, SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    #endregion


    #region Methods
    public async Task<IdentityResult> AddToRole(User user, List<string> roles)
    {
        return await _userManager.AddToRolesAsync(user, roles);
    }

    public async Task<IdentityResult> ConfirmEmail(User user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    public async Task<bool> ConfirmPhone(User user, string token)
    {
        return await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", token);
    }

    public async Task<IdentityResult> CreateUser(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> DeleteUser(User user)
    {
        return await _userManager.DeleteAsync(user);
    }

    public async Task<string> GenerateEmailConfirmationToken(User user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public Task<string> GenerateTwoFactorToken(User user, string provider)
    {
        return _userManager.GenerateTwoFactorTokenAsync(user, provider);
    }

    public async Task<List<User>> GetAll(CancellationToken cancellationToken)
    {
        return await _userManager.Users.AsNoTracking().Include(x => x.UserRoles).ThenInclude(x => x.Role).ToListAsync(cancellationToken);
    }

    public async Task<bool> GetAnyByEmail(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> GetAnyByPhone(string phone)
    {
        return await _userManager.Users.AnyAsync(x => x.PhoneNumber == phone);
    }

    public async Task<bool> GetAnyByUserName(string userName)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == userName);
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User> GetById(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<User> GetByPhone(string phone)
    {
        return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
    }

    public async Task<User> GetByUserName(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<IList<Claim>> GetClaims(User user)
    {
        return await _userManager.GetClaimsAsync(user);
    }

    public async Task<string> GetPasswordTokenEncoded(User user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return token;
    }

    public async Task<List<string>> GetRoles(User user)
    {
        return (await _userManager.GetRolesAsync(user)).ToList();
    }

    public async Task RefreshSignIn(User user)
    {
        await _signInManager.RefreshSignInAsync(user);
    }

    public async Task<IdentityResult> RemoveRole(User user, List<string> roles)
    {
        return await _userManager.RemoveFromRolesAsync(user, roles);
    }

    public async Task<IdentityResult> ResetPassword(User user, string token, string newPassword)
    {
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<SignInResult> SignInUser(string userName, string password, bool remember, bool lookout)
    {
        return await _signInManager.PasswordSignInAsync(userName, password, remember, lookout);
    }

    public async Task SignOutUser()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> UpdateUser(User user)
    {
        return await _userManager.UpdateAsync(user);
    }
    #endregion
}
