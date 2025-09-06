using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class Option
{
    [Key]
    public int Id { get; set; }
    public string OptionName { get; set; }
    public string OptionValue { get; set; }
}
