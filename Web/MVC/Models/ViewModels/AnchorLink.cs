namespace MVC.Models.ViewModels
{
	public class AnchorLink
	{
		public string Text { get; set; }
		public string Link { get; set; } = "#";
		public string Title { get; set; }
		public string Target { get; set; } = "_self";
	}
}
