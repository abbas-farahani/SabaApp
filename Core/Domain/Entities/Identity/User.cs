using Microsoft.AspNetCore.Identity;


namespace Core.Domain.Entities;

public class User : IdentityUser
{
    public virtual ICollection<UserClaim> Claims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
}
