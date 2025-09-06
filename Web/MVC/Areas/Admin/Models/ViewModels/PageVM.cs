using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVC.Areas.Admin.Models.ViewModels;

public class PageVM
{
    #region Base Entity
    public int? Id { get; set; }
    public DateTime? CreationTime { get; set; }
    public DateTime? LastModified { get; set; }
    #endregion


    #region Page Entity
    public string? UserId { get; set; }
    [Required]
    [Display(Name = "عنوان برگه")]
    [Description("عنوان برگه اجباری است")]
    public string Title { get; set; }
    [Display(Name = "متن برگه", Description = "")]
    public string Content { get; set; }
    [Display(Name = "نامک")]
    public string Slug { get; set; }
    #endregion

    [Display(Name = "نویسنده")]
    public string UserName { get; set; }
    [Display(Name ="اسلایدر")]
    public int? SliderId { get; set; }
    [Display(Name ="نمایش مسیر پیمایش")]
    public bool? EnableBreadCrumb { get; set; }
    [Display(Name ="نمایش فوتر")]
    public bool? EnableFooter { get; set; }
    [Display(Name ="نمایش کپی رایت")]
    public bool? EnableCopyRight { get; set; }
    [Display(Name = "وضعیت انتشار برگه")]
    public string Status { get; set; }
    [Display(Name = "برگه مادر (زبان اصلی)")]
    public int? OriginalPage { get; set; }

    [Display(Name = "زبان برگه")]
    public string? LanguagesList { get; set; }
    [Display(Name = "ترجمه‌ها")]
    public string? Translations { get; set; }
}
