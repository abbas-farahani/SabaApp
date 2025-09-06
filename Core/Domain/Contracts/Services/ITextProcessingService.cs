using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services;

public interface ITextProcessingService
{
    string GenerateSlug(string text);
    string SanitizeHtml(string text);
}
