using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Dtos.Blog;

public class PostDto
{
    //public Post post { get; set; }

    public int? Id { get; set; }
    public DateTime? CreationTime { get; set; }
    public DateTime? LastModified { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Slug { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<PostMeta>? PostMetas { get; set; }
    public ICollection<Category>? Categories { get; set; }

    public string UserId { get; set; }
    public string UserName { get; set; }
    public string CommentStatus { get; set; } = "open"; // 0:close, 1:open, 2:private
    public string? ThumbPath { get; set; }
    public string? ThumbName { get; set; }
    public List<int>? CategoriesId { get; set; }
    public string Status { get; set; }

    [Display(Name = "مقاله مادر (زبان اصلی)")]
    public int? OriginalArticle { get; set; }

    [Display(Name = "زبان مقاله")]
    public string? LanguagesList { get; set; }
}
