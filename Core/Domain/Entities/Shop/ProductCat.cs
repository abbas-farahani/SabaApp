using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities.Shop;

public class ProductCat : BaseEntity
{
    public int ParentId { get; set; } = 0;
    public string UserId { get; set; }

    [Display(Name = "عنوان دسته")]
    public string Label { get; set; }

    [Display(Name = "نامک دسته")]
    public string Slug { get; set; }

    [Display(Name = "توضیحات دسته")]
    public string Description { get; set; }



    public virtual User User { get; set; }
    public virtual ICollection<Product> Products { get; set; }
}
