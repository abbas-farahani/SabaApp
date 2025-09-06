using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class AttachmentMeta
{
    public int Id { get; set; }
    public int AttachmentId { get; set; }
    public string MetaName { get; set; }
    public string MetaValue { get; set; }

    public virtual Attachment Attachment { get; set; }
}



// MetaName:     FileTitle (SEO)
// MetaName:     FileAlt (SEO)

