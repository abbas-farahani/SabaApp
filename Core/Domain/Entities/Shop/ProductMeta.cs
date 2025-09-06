namespace Core.Domain.Entities.Shop;

public class ProductMeta
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string MetaName { get; set; }
    public string MetaValue { get; set; }


    public virtual Product Product { get; set; }
}