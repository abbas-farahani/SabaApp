using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.Shop;

public class Coupon : BaseEntity
{
    [Required]
    public string Name { get; set; }
    
    public string? Label { get; set; }

    [Required]
    public int Amount { get; set; }

    public string? Description { get; set; }

    public DateTime? ExpireDate { get; set; }

    public int? Limitation { get; set; }

    [Required]
    public byte Type { get; set; } // 0:Percentage - 1:Fixed Price - 2:Combine - 3:Formula

}
