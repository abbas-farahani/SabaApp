using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Contracts.AppServices;
using Core.Domain.Contracts.Services.Shop;

namespace Core.AppServices;

public class ShopAppService : IShopAppService
{
    private readonly IProductService _productService;
    private readonly IProductCatService _productCatService;
    public ShopAppService(IProductService productService, IProductCatService productCatService)
    {
        _productService = productService;
        _productCatService = productCatService;
    }
}
