namespace MVC.Models.ViewModels;

public class EditPermissionVM : AddPermissionVM
{
    public int Id { get; set; }
    public int? CreateId { get; set; } = 0;
    public int? ReadId { get; set; } = 0;
    public int? UpdateId { get; set; } = 0;
    public int? DeleteId { get; set; } = 0;
}