using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;
using MVC.Models.ViewModels;

namespace MVC.Areas.Admin.Models.ViewModels.Post;

public class PostVM : Core.Domain.Entities.Post
{
    public new int? Id { get; set; }
    public new string CommentStatus { get; set; } // 0:close, 1:open, 2:private
    public new List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
    public List<int>? CategoriesId { get; set; }
    public string Status { get; set; }
    public string? ThumbPath { get; set; }
    public string? ThumbName { get; set; }
    public List<UploadMedia>? UploadMedias { get; set; }

    [Display(Name = "مقاله مادر (زبان اصلی)")]
    public int? OriginalArticle { get; set; }

    [Display(Name = "زبان مقاله")]
    public string? LanguagesList { get; set; }

    public new User? User { get; set; }
}
