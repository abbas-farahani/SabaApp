using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models.ViewModels;

public class UserVM
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Phone { get; set; }

    [Display(Name = "تصویر پروفایل")]
    public string Avatar { get; set; }

    [Display(Name = "استان محل سکونت")]
    public string Province { get; set; }
}
