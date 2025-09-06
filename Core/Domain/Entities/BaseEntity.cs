using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? LastModified { get; set; }
}
