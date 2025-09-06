using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.Dtos.Blog;

public class PageDto
{
    #region Base Entity
    public int? Id { get; set; }
    public DateTime? CreationTime { get; set; }
    public DateTime? LastModified { get; set; }
    #endregion

    #region Page Entity
    [Required]
    public string UserId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    public string Content { get; set; }
    [Required]
    public string Slug { get; set; }
    #endregion


    public string UserName { get; set; }
    public int? SliderId { get; set; }
    public bool? EnableBreadCrumb { get; set; }
    public bool? EnableFooter { get; set; }
    public bool? EnableCopyRight { get; set; }
    public string Status { get; set; }
    public int? OriginalPage { get; set; }
    public string? LanguagesList { get; set; }
    public string? Translations { get; set; }
}
