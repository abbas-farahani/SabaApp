using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities.Shop;

public class ReviewMeta
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public string MetaName { get; set; }
    public string MetaValue { get; set; }


    public virtual Review Review { get; set; }
}
