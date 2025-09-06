namespace MVC.Models.ViewModels;

public class CustomBtn
{
    public string Text { get; set; }
    public string Link { get; set; }
    public string? QueryString { get; set; }
    public List<string> ClassList { get; set; } = new List<string>();
    public string Target { get; set; } = "_self";
    public string Title { get; set; }
    public KeenIcon? Icon { get; set; }
    public bool IsModal { get; set; } = false;
    public string? ModalTarget { get; set; }
}

public class KeenIcon
{
    public string IconName { get; set; }
    public int PathCount { get; set; } = 0;
}
