using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels;

public class CategoryVM
{
    public int Id { get; set; }

    [Display(Name = "دسته بندی والد")]
    public int? ParentId { get; set; } = 0;

    [Required]
    [Display(Name ="نام دسته بندی")]
    public string Label { get; set; }

    [Required]
    [Display(Name ="نامک")]
    public string Slug { get; set; }

    [Display(Name = "توضیحات")]
    public string? Description { get; set; } = "";
}
