using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public class RoleClaim : IdentityRoleClaim<string>
{
    public virtual Role Role { get; set; }
}
