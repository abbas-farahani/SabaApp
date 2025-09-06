using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Utilities.SlugifyPersian;

public class SlugifyPersian
{
    public string GenerateSlug(string phrase)
    {
        string str = RemoveDiacritics(phrase).ToLower();
        str = Regex.Replace(str, @"\s+", "-"); // Replace spaces with hyphens
        str = Regex.Replace(str, @"[^\w\-]+", ""); // Remove all non-word characters
        str = Regex.Replace(str, @"\-\-+", "-"); // Replace multiple hyphens with a single hyphen
        str = str.Trim('-'); // Trim hyphens from the start and end of the string
        if (str.Length > 60)
            return str.Substring(0, 60);
        return str;
    }

    private string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}

