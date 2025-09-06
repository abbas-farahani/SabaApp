using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.Shop;

public class Review : BaseEntity
{
    [Required]
    public int ProductId { get; set; }
    public int? parentId { get; set; }
    public string? UserId { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string Ip { get; set; }
    public string Agent { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; }
    public byte Status { get; set; } = 1; //0:Delete - 1:Approve - 2:Edit - 3:Reject - 4:Spam


    public virtual Product Product { get; set; }
    public virtual ICollection<ReviewMeta> ReviewMetas { get; set; }
}
