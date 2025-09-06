using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels;

public class AddPermissionVM
{
    [StringLength(maximumLength: 62)]
    [Display(Name ="نام دسترسی")]
    public string Name { get; set; }
    public bool Create { get; set; }
    public bool Read { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }

    [Display(Name ="توضیحات دسترسی")]
    public string? Description { get; set; }
}
