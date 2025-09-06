using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class Role : IdentityRole
{
    [StringLength(maximumLength: 250)]
    [Display(Name = "توضیحات نقش")]
    public string? Description { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    public virtual ICollection<Permission> Permissions { get; set; }
}
