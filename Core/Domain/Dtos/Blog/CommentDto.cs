using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.Dtos.Blog;

public class CommentDto
{
    public int? Id { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public DateTime? LastModified { get; set; } = DateTime.Now;

    [Required]
    public int PostId { get; set; }
    public int ParentId { get; set; } = 0;
    public string UserId { get; set; } = "";

    [Required]
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string Ip { get; set; }
    public string Agent { get; set; }

    [Required]
    public string Content { get; set; }
    public byte Status { get; set; } = 0; //0:Draft - 1:Approve - 2:Edit - 3:Reject - 4:Spam 5:Delete

    public virtual Post Post { get; set; }
    public virtual ICollection<CommentMeta> CommentMetas { get; set; }
}
