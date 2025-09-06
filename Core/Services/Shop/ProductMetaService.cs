using Core.Domain.Entities.Shop;
using Core.Domain.Contracts.Services.Shop;
using Core.Domain.Contracts.Repositories.Shop;
using Core.Domain.Entities;

namespace Core.Services.Shop;

public class ProductMetaService : BaseService<ProductMeta>, IProductMetaService
{
    private readonly IProductMetaRepository _productMetaRepository;

    public ProductMetaService(IProductMetaRepository productMetaRepository) : base(productMetaRepository)
    {
        _productMetaRepository = productMetaRepository;
    }

    public async Task<ProductMeta> GetByIdAsync(int productId, string metaName)
    {
        return await _productMetaRepository.GetByIdAsync(productId, metaName);
    }

    public async Task<bool> AddTranslationMeta(Product currentProduct)
    {
        try
        {
            var dictionary = new Dictionary<string, int>();
            var currentLanguageCode = currentProduct.ProductMetas.First(x => x.MetaName == "LanguageCode").MetaValue;
            dictionary.Add(currentLanguageCode, currentProduct.Id);

            var newMeta = new ProductMeta
            {
                ProductId = currentProduct.Id,
                MetaName = "Translation",
                MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary)
            };
            var result = await AddAsync(newMeta);
            if (result == null) return false;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateTranslationMeta(Product currentProduct, Product parentProduct)
    {
        try
        {
            var dictionary = new Dictionary<string, int>();
            //var parentProduct = await _productService.GetByIdAsync(parentProductId);
            var meta = parentProduct.ProductMetas.FirstOrDefault(x => x.MetaName == "Translation");
            var parentLanguageCode = parentProduct.ProductMetas.First(x => x.MetaName == "LanguageCode").MetaValue;
            var currentLanguageCode = currentProduct.ProductMetas.First(x => x.MetaName == "LanguageCode").MetaValue;
            if (meta != null)
            {
                dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(meta.MetaValue);
                dictionary.Add(currentLanguageCode, currentProduct.Id);
            }
            else
            {
                dictionary.Add(parentLanguageCode, parentProduct.Id);
                dictionary.Add(currentLanguageCode, currentProduct.Id);
            }

            var ids = dictionary.Values.ToList();
            foreach (var id in ids)
            {
                var translationMeta = await GetByIdAsync(id, "Translation");
                if (translationMeta != null)
                {
                    translationMeta.MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
                    var result = await UpdateAsync(translationMeta);
                    if (result == null) return false;
                }
                else
                {
                    var newMeta = new ProductMeta
                    {
                        ProductId = id,
                        MetaName = "Translation",
                        MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary)
                    };
                    var result = await AddAsync(newMeta);
                    if (result == null) return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
