using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class CommentMeta
{
    public int Id { get; set; }
    public int CommentId { get; set; }
    public string MetaName { get; set; }
    public string MetaValue { get; set; }


    public virtual Comment Comment { get; set; }
}
