using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Shop;
using Microsoft.AspNetCore.Mvc;

namespace MVC.ViewComponents;

public class ArticlesListViewComponent : ViewComponent
{
    private readonly IPostService _postService;

    public ArticlesListViewComponent(IPostService postService)
    {
        _postService = postService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int postCount = 8)
    {
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        var articles = await _postService.GetList(postCount, culture);

        // TODO: Set carousel columns

        return View("ArticlesList", articles);
    }
}
