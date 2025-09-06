using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class PageMeta
{
    public int Id { get; set; }
    public int PageId { get; set; }
    public string MetaName { get; set; }
    public string MetaValue { get; set; }


    public virtual Page Page { get; set; }
}
