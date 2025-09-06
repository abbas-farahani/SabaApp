using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels.User;

public class EditUserVM
{
    public string Id { get; set; }

    [MaxLength(100)]
    [Display(Name ="نام کاربری")]
    public string? UserName { get; set; }

    [Display(Name ="نام")]
    public string? FirstName { get; set; }

    [Display(Name ="نام خانوادگی")]
    public string? LastName { get; set; }

    [Display(Name = "تاریخ تولد")]
    public DateTime? BirthDate { get; set; }

    [Display(Name ="تصویر کاربر")]
    public string? AvatarPath { get; set; }

    [MaxLength(30)]
    [Display(Name ="کشور")]
    public string? Country { get; set; }

    [MaxLength(7)]
    [Display(Name ="زبان")]
    public string? Language { get; set; }

    [Phone]
    [MaxLength(11)]
    [Display(Name ="تلفن")]
    public string? Phone { get; set; }

    [EmailAddress]
    [Display(Name ="پست الکترونیک")]
    public string? Email { get; set; }

}
