using Microsoft.AspNetCore.Mvc.Rendering;

namespace Utilities.HtmlHelpers;

/// <summary>
/// Source: https://stackoverflow.com/questions/18248547/get-controller-and-action-name-from-within-controller
/// </summary>
public static class HtmlRequestHelper
{
	public static string Id(this IHtmlHelper htmlHelper)
	{
		var routeValues = htmlHelper.ViewContext.RouteData.Values;

		if (routeValues.ContainsKey("id"))
			return (string)routeValues["id"];
		else if (htmlHelper.ViewContext.RouteData.Values["id"] != null)
			return htmlHelper.ViewContext.RouteData.Values["id"].ToString();

		return string.Empty;
	}

	public static string Area(this IHtmlHelper htmlHelper)
	{
		var routeValues = htmlHelper.ViewContext.RouteData.Values;

		if (routeValues.ContainsKey("area"))
			return (string)routeValues["area"];

		return string.Empty;
	}

	public static string Controller(this IHtmlHelper htmlHelper)
	{
		var routeValues = htmlHelper.ViewContext.RouteData.Values;

		if (routeValues.ContainsKey("controller"))
			return (string)routeValues["controller"];

		return string.Empty;
	}

	public static string Action(this IHtmlHelper htmlHelper)
	{
		var routeValues = htmlHelper.ViewContext.RouteData.Values;

		if (routeValues.ContainsKey("action"))
			return (string)routeValues["action"];

		return string.Empty;
	}
}