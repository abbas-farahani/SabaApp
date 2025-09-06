using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class Category : BaseEntity
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
    public virtual ICollection<Post> Posts { get; set; }
}
