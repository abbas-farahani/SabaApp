using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Dashboard.Models;

public class ResetPasswordVM
{
    [Required]
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

	[Required]
	[Display(Name = "New Password")]
    [Compare(nameof(NewPassword))]
	public string ConfirmPassword { get; set; }

    [Required]
	[EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }
}
