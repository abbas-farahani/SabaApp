using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class PostMeta
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string MetaName { get; set; }
    public string MetaValue { get; set; }


    public virtual Post Post { get; set; }
}
