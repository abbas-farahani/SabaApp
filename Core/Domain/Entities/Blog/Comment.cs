using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class Comment : BaseEntity
{
    [Required]
    public int PostId { get; set; }
    public int ParentId { get; set; } = 0;
    public string UserId { get; set; } = "";

    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string Website { get; set; }
    public string Ip { get; set; }
    public string Agent { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; }
    public byte Status { get; set; } = 1; //0:Delete - 1:Approve - 2:Edit - 3:Reject - 4:Spam


    public virtual Post Post { get; set; }
    public virtual ICollection<CommentMeta> CommentMetas { get; set; }
}
