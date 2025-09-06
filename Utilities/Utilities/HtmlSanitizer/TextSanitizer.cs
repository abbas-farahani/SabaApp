using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities.HtmlSanitizer;

public static class TextSanitizer
{
    public static string Sanitize(string text, string v)
    {
        //var str = new HtmlSanitizer().
        return text;
    }

    public static string Sanitize(this string text)
    {
        Regex pattern = new Regex(@"[!@#$%^&*'""/<>;{}~`:]");

        if (!string.IsNullOrEmpty(text) && pattern.IsMatch(text))
            return pattern.Replace(text, "");
        return text;
    }

    public static string SanitizePath(this string text)
    {
        Regex pattern = new Regex(@"[!#$%^&*'""<>;{}~`]");

        if (!string.IsNullOrEmpty(text) && pattern.IsMatch(text))
            return pattern.Replace(text, "");

        return text;
    }
}
