using System.ComponentModel.DataAnnotations;
using MVC.Resources;

namespace MVC.Models.ViewModels;

public class LoginVM
{
    [Required(ErrorMessageResourceName = "Requirement_Error_UserName", ErrorMessageResourceType = typeof(SharedResources))]
    [Display(Name = "UserName", ResourceType = typeof(SharedResources))]
    [StringLength(200)]
    public string UserName { get; set; }

    //[Required]
    //[EmailAddress]
    //public string Email { get; set; }

    [Required(ErrorMessageResourceName = "Requirement_Error_Password", ErrorMessageResourceType = typeof(SharedResources))]
    [Display(Name = "Password", ResourceType = typeof(SharedResources))]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "RememberMe", ResourceType = typeof(SharedResources))]
    public bool RememberMe { get; set; }
}
