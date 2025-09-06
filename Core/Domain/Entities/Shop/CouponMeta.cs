using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities.Shop;

public class CouponMeta
{
    [Key]
    public int Id { get; set; }

    public int CouponId { get; set; }
    public string MetaName { get; set; }
    public string MetaValue { get; set; }


    public virtual Coupon Coupon { get; set; }
}
