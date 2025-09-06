using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public class UserLogin : IdentityUserLogin<string>
{
    public virtual User User { get; set; }
}
