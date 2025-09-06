using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models.ViewModels.Options;

public class GeneralVM
{
    [Display(Name = "نام سایت")]
    public string SiteName { get; set; } = "";

    [Display(Name = "عنوان سایت")]
    public string SiteTitle { get; set; } = "";

    [Display(Name = "توضیحات معرفی سایت")]
    public string SiteDescription { get; set; } = "";

    [Display(Name = "فعالسازی عضویت")]
    public bool AllowRegistration { get; set; } = false;

    [Display(Name = "آیکون وب سایت")]
    public IFormFile SiteFavIcon { get; set; }

    [Display(Name = "آیکون وب سایت")]
    public string SiteFavIconPath { get; set; } = "";

    [Display(Name = "لوگو(نمایه) سایت")]
    public IFormFile SiteLogo { get; set; }

    [Display(Name = "لوگو(نمایه) سایت")]
    public string SiteLogoPath { get; set; } = "";

    [Display(Name = "فعالسازی عضویت با موبایل")]
    public bool MobileRegisteration { get; set; } = false;

    [Display(Name = "نیاز به ارسال تاییدیه ایمیل است؟")]
    public bool SendEmailConfirmation { get; set; } = false;

    [Display(Name = "نیاز به تایید شماره تلفن است؟")]
    public bool SendPhoneConfirmation { get; set; } = false;

    [Display(Name = "کد توکن سرویس کریپتو")]
    public string CryptoApiToken { get; set; } = "";

    [Display(Name ="زبان وب سایت")]
    public List<string>? Languages { get; set; }

    [Display(Name = "زبان پیشفرض وب سایت")]
    public string DefaultLanguage { get; set; } = "fa-IR";

    [Display(Name = "آیا وب سایت چند زبانه است؟")]
    public bool IsMultilingual { get; set; } = false;
}
