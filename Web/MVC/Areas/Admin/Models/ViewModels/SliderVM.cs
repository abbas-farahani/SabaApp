namespace MVC.Areas.Admin.Models.ViewModels;

public class SliderVM
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Title { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModified { get; set; }
    public List<SlideVM>? Slides { get; set; }
}
