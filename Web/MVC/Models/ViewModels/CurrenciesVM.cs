namespace MVC.Models.ViewModels;

public class CurrenciesVM
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public bool Selected { get; set; } = false;
}
