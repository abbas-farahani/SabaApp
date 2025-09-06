using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels;

public class CommentVM
{
    [Required]
    public int PostId { get; set; }
    public int? ParentId { get; set; } = 0;

    public string? UserName { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Url]
    public string? Website { get; set; }
    public string Content { get; set; }
}
