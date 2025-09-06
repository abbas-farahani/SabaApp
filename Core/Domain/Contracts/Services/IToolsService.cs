using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services;

public interface IToolsService
{
    string FileSizeHelper(long fileSize);
}
