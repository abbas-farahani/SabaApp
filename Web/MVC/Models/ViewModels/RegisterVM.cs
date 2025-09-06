using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels;

public class RegisterVM
{
    [Required]
    [MaxLength(250)]
    [Remote("ExistsUserName", "Dashboard", "Dashboard", HttpMethod ="Post", AdditionalFields = "__RequestVerificationToken")]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Remote("ExistsEmail", "Dashboard", "Dashboard", HttpMethod ="Post", AdditionalFields = "__RequestVerificationToken")]
    public string Email { get; set; }

    [Phone]
    [Remote("ExistsPhone", "Dashboard", "Dashboard", HttpMethod ="Post", AdditionalFields = "__RequestVerificationToken")]
    public string Phone { get; set; }

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string RePassword { get; set; }
}
