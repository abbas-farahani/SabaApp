using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class Post : BaseEntity
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "عنوان نوشته")]
    [Description("عنوان نوشته اجباری است")]
    public string Title { get; set; }

    [Display(Name = "متن نوشته", Description = "")]
    public string Content { get; set; }

    [Display(Name = "نامک", Description = "از کاراکتر فاصله استفاده نکنید؛ فقط از حروف، اعداد و کاراکتر (_) استفاده شود")]
    public string Slug { get; set; }

    [Display(Name = "وضعیت دیدگاه", Description = "تعیین کنید آیا امکان ثبت دیدگاه برای کاربران وجود دارد یا خیر")]
    public byte CommentStatus { get; set; } = 1; //0:Close - 1:Open - 2:Private

    [Display(Name = "وضعیت انتشار نوشته", Description = "تعیین کنید که آیا مقاله به صورت عمومی منتشر شود یا خیر")]
    public byte Status { get; set; } = 1; //0:Delete - 1:Publish - 2:Edit - 3:Draft - 4:Private(show content by password) - 5:Scheduled




    public virtual User User { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<PostMeta> PostMetas { get; set; } = new List<PostMeta>();
    public virtual ICollection<Category> Categories { get; set; }
}
