using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Admin.Models.ViewModels;

public class SlideVM
{
    public int? Id { get; set; }
    public int? SliderId { get; set; }
    public string? SliderName { get; set; }
    public string? Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModified { get; set; }

    [Url]
    public string? Url { get; set; }
    public string BackgroundType { get; set; }
    public string BackgroundValue { get; set; }
}
