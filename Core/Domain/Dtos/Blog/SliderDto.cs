using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.Dtos.Blog;

public class SliderDto
{
    public int Id { get; set; } = 0;
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public DateTime? LastModified { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public List<SlideDto> Slides { get; set; } = new List<SlideDto> { };
}
