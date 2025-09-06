using Core.Domain.Contracts.Services;
using Core.Domain.Contracts.Services.Shop;
using Microsoft.AspNetCore.Mvc;

namespace MVC.ViewComponents;

public class ProductsListViewComponent : ViewComponent
{
    private readonly IProductService _productService;

    public ProductsListViewComponent(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productCount = 8)
    {
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        var products = await _productService.GetList(productCount, culture);

        // TODO: Set carousel columns

        return View("productsList", products);
    }
}

