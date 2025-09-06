using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;

namespace Core.Domain.Dtos.Blog;

public class SlideDto
{
    public int Id { get; set; } = 0;
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public DateTime? LastModified { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    [Url]
    public string? Url { get; set; }
    public string BackgroundType { get; set; }
    public string BackgroundValue { get; set; }
}