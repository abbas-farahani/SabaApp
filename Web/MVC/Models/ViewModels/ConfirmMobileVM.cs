using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels;

public class ConfirmMobileVM
{
    [Required]
    [MaxLength(11)]
    public string Phone { get; set; }

    [Required]
    [StringLength(6)]
    public string Code { get; set; }
}
