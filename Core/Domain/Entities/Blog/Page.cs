using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities; 

public class Page : BaseEntity
{
    [Required]
    public string UserId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    public string Content { get; set; }
    [Required]
    public string Slug { get; set; }
    public byte Status { get; set; } = 1; //0:Delete - 1:Publish - 2:Edit - 3:Draft - 4:Private(show content by password)



    public virtual User User { get; set; }
    public virtual ICollection<PageMeta> PageMetas { get; set; }
}
