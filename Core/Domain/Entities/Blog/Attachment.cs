using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class Attachment : BaseEntity
{
	public string UserId { get; set; }

	[StringLength(200)]
	public string Title { get; set; }

	[StringLength(200)]
	public string FileName { get; set; }

	[StringLength(8)]
	public string FileExtension { get; set; }

	public long FileSize { get; set; }

	[StringLength(200)]
	public string Slug { get; set; }
	public string Path { get; set; }


	[StringLength(1000)]
	public string? Description { get; set; }
	public byte Status { get; set; } = 1; //0:Delete - 1:Publish - 2:Edit - 3:Draft - 4:Private(show content by password)



	public virtual User User { get; set; }
	public virtual ICollection<AttachmentMeta> AttachmentMeta { get; set; }
}
