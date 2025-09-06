using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class Permission : BaseEntity
{
    [StringLength(maximumLength:60)]
    public string Name { get; set; }

    [StringLength(maximumLength: 250)]
    public string? Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; }= new List<Role>();
}
