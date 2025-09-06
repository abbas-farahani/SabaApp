using Core.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Blog;

public class ToolsService : IToolsService
{
	public string FileSizeHelper(long fileSize)
	{
		if (fileSize < 1024)
			return $"{fileSize} B";
		else if (fileSize < 1024 * 1024)
			return $"{fileSize / 1024.0:F2} KB";
		else if (fileSize < 1024 * 1024 * 1024)
			return $"{fileSize / (1024.0 * 1024):F2} MB";
		else
			return $"{fileSize / (1024.0 * 1024 * 1024):F2} GB";
	}
}
