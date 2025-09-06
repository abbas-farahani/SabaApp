using Core.Domain.Entities.Shop;
using Core.Domain.Contracts.Services.Shop;
using Core.Domain.Contracts.Repositories.Shop;
using Core.Domain.Dtos.Shop;
using Newtonsoft.Json;

namespace Core.Services.Shop;

public class ProductService : BaseService<Product>, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCatService _productCatService;
    private readonly IProductMetaService _productMetaService;

    public ProductService(IProductRepository productRepository, IProductCatService productCatService, IProductMetaService productMetaService) : base(productRepository)
    {
        _productRepository = productRepository;
        _productCatService = productCatService;
        _productMetaService = productMetaService;
    }

    public async Task<Product> GetBySlug(string slug)
    {
        return await _productRepository.GetBySlug(slug);
    }

    public async Task<Product> AddAsync(ProductDto productDto)
    {
        #region Product Metas
        var metas = new List<ProductMeta>();
        if (!string.IsNullOrEmpty(productDto.SKU)) metas.Add(new ProductMeta { MetaName = "SKU", MetaValue = productDto.SKU });
        metas.Add(new ProductMeta { MetaName = "RegularPrice", MetaValue = productDto.RegularPrice.ToString() });
        if (productDto.SalePrice != null) metas.Add(new ProductMeta { MetaName = "SalePrice", MetaValue = productDto.SalePrice.Value.ToString() });
        if (productDto.DiscountExpiration != null) metas.Add(new ProductMeta { MetaName = "DiscountExpiration", MetaValue = productDto.DiscountExpiration.ToString() });
        if (productDto.Quantity != null) metas.Add(new ProductMeta { MetaName = "Quantity", MetaValue = productDto.Quantity.Value.ToString() });
        metas.Add(new ProductMeta { MetaName = "ProductType", MetaValue = productDto.ProductType });
        if (productDto.SoldIndividually) metas.Add(new ProductMeta { MetaName = "SoldIndividually", MetaValue = "true" });
        else metas.Add(new ProductMeta { MetaName = "SoldIndividually", MetaValue = "false" });
        if (productDto.Gallery != null && productDto.Gallery.Count() > 0) metas.Add(new ProductMeta { MetaName = "Gallery", MetaValue = JsonConvert.SerializeObject(productDto.Gallery) });
        if ( !string.IsNullOrEmpty(productDto.LanguagesList)) metas.Add(new ProductMeta { MetaName = "LanguageCode", MetaValue = productDto.LanguagesList });
        productDto.product.ProductMetas = metas;
        #endregion

        #region Category
        if (productDto.CategoriesId != null && productDto.CategoriesId.Count() > 0)
        {
            var categories = await _productCatService.GetByIdAsync(productDto.CategoriesId);
            productDto.product.ProductCats = categories;
        }
        #endregion

        #region Product
        productDto.product.CreationTime = DateTime.Now;
        productDto.product.CommentStatus = (byte)(productDto.CommentStatus == "open" ? 1 : (productDto.CommentStatus == "private" ? 2 : 0));
        byte status = 3;
        if (productDto.Status == "published") status = 1;
        else if (productDto.Status == "edited") status = 2;
        else if (productDto.Status == "private") status = 4;
        else if (productDto.Status == "scheduled") status = 5;
        else status = 3;
        productDto.product.Status = status;
        #endregion

        try
        {
            var product = await _productRepository.CreateAsync(productDto.product);
            return product;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _productRepository.IsExistBySlug(slug);
    }

    public async Task<List<Product>> GetOriginalProducts(string defaultLanguage)
    {
        return await _productRepository.GetOriginalProducts(defaultLanguage);
    }

    public async Task<List<Product>> GetAllAsync(string lang)
    {
        return await _productRepository.GetAllAsync(lang);
    }

    public async Task<Product> UpdateAsync(ProductDto productDto)
    {
        var product = await GetByIdAsync(productDto.product.Id);
        if (product == null) return null;


        #region Product Metas
        var metas = product.ProductMetas;
        product.ProductMetas.Clear();
        #region SKU Meta
        if (metas.Any(x => x.MetaName == "SKU"))
        {
            if (!string.IsNullOrEmpty(productDto.SKU)) metas.Where(x => x.MetaName == "SKU").FirstOrDefault().MetaValue = productDto.SKU;
        }
        else
        {
            if (!string.IsNullOrEmpty(productDto.SKU)) metas.Add(new ProductMeta { MetaName = "SKU", MetaValue = productDto.SKU });
        }

        #endregion

        if (metas.Any(x => x.MetaName == "RegularPrice"))
        {
            metas.Where(x => x.MetaName == "RegularPrice").FirstOrDefault().MetaValue = productDto.RegularPrice.ToString();
        }
        else
        {
            metas.Add(new ProductMeta { MetaName = "RegularPrice", MetaValue = productDto.RegularPrice.ToString() });
        }

        if (metas.Any(x => x.MetaName == "SalePrice"))
        {
            if (productDto.SalePrice != null) metas.Where(x => x.MetaName == "SalePrice").FirstOrDefault().MetaValue = productDto.SalePrice.ToString();
        }
        else
        {
            if (productDto.SalePrice != null) metas.Add(new ProductMeta { MetaName = "SalePrice", MetaValue = productDto.SalePrice.Value.ToString() });
        }

        if (metas.Any(x => x.MetaName == "DiscountExpiration"))
        {
            if (productDto.DiscountExpiration != null) metas.Where(x => x.MetaName == "DiscountExpiration").FirstOrDefault().MetaValue = productDto.DiscountExpiration.ToString();
        }
        else
        {
            if (productDto.DiscountExpiration != null) metas.Add(new ProductMeta { MetaName = "DiscountExpiration", MetaValue = productDto.DiscountExpiration.ToString() });
        }

        if (metas.Any(x => x.MetaName == "Quantity"))
        {
            if (productDto.Quantity != null) metas.Where(x => x.MetaName == "Quantity").FirstOrDefault().MetaValue = productDto.Quantity.ToString();
        }
        else
        {
            if (productDto.Quantity != null) metas.Add(new ProductMeta { MetaName = "Quantity", MetaValue = productDto.Quantity.Value.ToString() });
        }

        if (metas.Any(x => x.MetaName == "ProductType"))
        {
            metas.Where(x => x.MetaName == "ProductType").FirstOrDefault().MetaValue = productDto.ProductType;
        }
        else
        {
            metas.Add(new ProductMeta { MetaName = "ProductType", MetaValue = productDto.ProductType });
        }

        if (metas.Any(x => x.MetaName == "SoldIndividually"))
        {
            metas.Where(x => x.MetaName == "SoldIndividually").FirstOrDefault().MetaValue = productDto.SoldIndividually ? "true" : "false";
        }
        else
        {
            if (productDto.SoldIndividually) metas.Add(new ProductMeta { MetaName = "SoldIndividually", MetaValue = "true" });
            else metas.Add(new ProductMeta { MetaName = "SoldIndividually", MetaValue = "false" });
        }

        if (metas.Any(x => x.MetaName == "Gallery"))
        {
            metas.Where(x => x.MetaName == "Gallery").FirstOrDefault().MetaValue = JsonConvert.SerializeObject(productDto.Gallery);
        }
        else
        {
            if (productDto.Gallery != null && productDto.Gallery.Count() > 0) metas.Add(new ProductMeta { MetaName = "Gallery", MetaValue = JsonConvert.SerializeObject(productDto.Gallery) });
        }

        if (metas.Any(x => x.MetaName == "LanguageCode"))
        {
            metas.Where(x => x.MetaName == "LanguageCode").FirstOrDefault().MetaValue = productDto.LanguagesList;
        }
        else
        {
            if (productDto.LanguagesList != null) metas.Add(new ProductMeta { MetaName = "LanguageCode", MetaValue = productDto.LanguagesList });
        }

        product.ProductMetas = metas;
        #endregion

        #region Category
        product.ProductCats.Clear();
        if (productDto.CategoriesId != null && productDto.CategoriesId.Count() > 0)
        {
            var categories = await _productCatService.GetByIdAsync(productDto.CategoriesId);
            product.ProductCats = categories;
        }
        #endregion

        #region Product
        product.Title = productDto.product.Title;
        product.Slug = productDto.product.Slug;
        product.Description = productDto.product.Description;
        product.Content = productDto.product.Content;
        product.LastModified = DateTime.Now;
        product.CommentStatus = (byte)(productDto.CommentStatus == "open" ? 1 : (productDto.CommentStatus == "private" ? 2 : 0));
        byte status = 3;
        if (productDto.Status == "published") status = 1;
        else if (productDto.Status == "dreditedaft") status = 2;
        else if (productDto.Status == "private") status = 4;
        else if (productDto.Status == "scheduled") status = 5;

        product.Status = status;
        #endregion

        try
        {
            var result = await _productRepository.UpdateAsync(product);
            if (result == null) return null;
            return product;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<Product>> GetList(int count, string? culture)
    {
        return await _productRepository.GetList(count, culture);
    }
}
