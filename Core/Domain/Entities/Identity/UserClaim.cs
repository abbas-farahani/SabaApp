using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public class UserClaim : IdentityUserClaim<string>
{
    public virtual User User { get; set; }
}
