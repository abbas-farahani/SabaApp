using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Dtos.Shop;

public class ProductCatDto
{
    public int Id { get; set; }

    [Display(Name = "دسته بندی والد")]
    public int? ParentId { get; set; } = 0;

    [Required]
    [Display(Name = "نام دسته بندی")]
    public string Label { get; set; }

    [Required]
    [Display(Name = "نامک")]
    public string Slug { get; set; }

    [Display(Name = "توضیحات")]
    public string? Description { get; set; } = "";

    public string? UserId { get; set; }

}
