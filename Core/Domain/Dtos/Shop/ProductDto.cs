using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities.Shop;

namespace Core.Domain.Dtos.Shop;

public class ProductDto
{
    public Product product { get; set; }
    public string CommentStatus { get; set; } = "close"; // 0:close, 1:open, 2:private
    public string? ThumbPath { get; set; }
    public string? ThumbName { get; set; }
    public List<int>? CategoriesId { get; set; }
    public string Status { get; set; }

    [Display(Name = "سریال SKU")]
    public string SKU { get; set; }

    [Display(Name = "قیمت محصول")]
    public long RegularPrice { get; set; } = 0;

    [Display(Name = "قیمت تخفیف")]
    public long? SalePrice { get; set; }

    [Display(Name = "انقضای زمان تخفیف")]
    public DateTime? DiscountExpiration { get; set; }

    [Display(Name = "موجودی")]
    public int? Quantity { get; set; }

    [Display(Name = "نوع محصول")]
    public string ProductType { get; set; } = "physical"; // 0:virtual, 1:physical, 3:downloadable,

    [Display(Name = "فروش تکی")]
    public bool SoldIndividually { get; set; } = false;

    public List<string>? Gallery { get; set; }

    [Display(Name ="محصول مادر (زبان اصلی)")]
    public int? OriginalProduct { get; set; }

    [Display(Name ="زبان محصول")]
    public string? LanguagesList { get; set; }

}
