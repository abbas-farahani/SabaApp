using Core.Domain.Contracts.Services;
using Ganss.Xss;
using Slugify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.SlugifyPersian;

namespace Core.Services;

public class TextProcessingService : ITextProcessingService
{
    //private readonly SlugHelperForNonAsciiLanguages _slugHelper;
    private readonly SlugifyPersian _slugHelper;
    private readonly HtmlSanitizer _sanitizer;
    public TextProcessingService()
    {
        _slugHelper = new SlugifyPersian();
        _sanitizer = new HtmlSanitizer();
    }
    public string GenerateSlug(string text)
    {
        try
        {
            return _slugHelper.GenerateSlug(text);
        }
        catch (Exception er)
        {
            return text;
        }
    }

    public string SanitizeHtml(string text)
    {
        return _sanitizer.Sanitize(text);
    }
}
