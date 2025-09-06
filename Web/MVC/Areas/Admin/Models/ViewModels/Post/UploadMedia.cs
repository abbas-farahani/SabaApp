namespace MVC.Areas.Admin.Models.ViewModels.Post;

public class UploadMedia
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public IFormFile ThumbFile { get; set; }
    public string Description { get; set; }
    public byte Status { get; set; } = 1;
    public DateTime CreationTime { get; set; }
    public DateTime LastModified { get; set; }
}
