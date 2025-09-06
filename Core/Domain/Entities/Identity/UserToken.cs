using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public class UserToken : IdentityUserToken<string>
{
    public virtual User User { get; set; }
}