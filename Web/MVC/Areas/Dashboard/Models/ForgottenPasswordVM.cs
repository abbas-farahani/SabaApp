using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Dashboard.Models;

public class ForgottenPasswordVM
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
