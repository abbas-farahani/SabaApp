using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.Shop;

public class Product : BaseEntity
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "عنوان محصول")]
    [Description("عنوان محصول اجباری است")]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Display(Name = "متن محصول")]
    public string Content { get; set; }

    [Display(Name = "نامک", Description = "از کاراکتر فاصله استفاده نکنید؛ فقط از حروف، اعداد و کاراکتر (_) استفاده شود")]
    public string Slug { get; set; }

    [Display(Name = "وضعیت دیدگاه", Description = "تعیین کنید آیا امکان ثبت دیدگاه برای کاربران وجود دارد یا خیر")]
    public byte CommentStatus { get; set; } = 1; //0:Close - 1:Open - 2:Private

    [Display(Name = "وضعیت انتشار نوشته", Description = "تعیین کنید که آیا مقاله به صورت عمومی منتشر شود یا خیر")]
    public byte Status { get; set; } = 1; //0:Delete - 1:Publish - 2:Edit - 3:Draft - 4:Private(show content by password)

    [Required]
    public int Price { get; set; }

    [Required]
    [Display(Name = "تصویر محصول")]
    public string FeaturedImage { get; set; }



    public virtual User User { get; set; }

    [Display(Name = "بازخورد محصول")]
    public virtual ICollection<Review> Reviews { get; set; }

    public virtual ICollection<ProductMeta> ProductMetas { get; set; }

    [Display(Name = "دسته بندی محصول")]
    public virtual ICollection<ProductCat> ProductCats { get; set; }
}
