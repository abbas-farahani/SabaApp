using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Languages;

public static class LanguagesUtil
{
    private static List<Tuple<string, string, string, string, string, string>> LanguagesTuple()
    {
        return new List<Tuple<string, string, string, string, string, string>>
        {
            new Tuple<string, string, string, string, string, string>("Persian", "فارسی","fa-IR", "Iran", "Rial", "/media/flags/iran.svg"),
            new Tuple<string, string, string, string, string, string>("Arabic", "عربی","ar-SA", "Saudi Arabia", "USD", "/media/flags/saudi-arabia.svg"),
            new Tuple<string, string, string, string, string, string>("English", "انگلیسی","en-US", "United States", "USD", "/media/flags/united-states.svg"),
            new Tuple<string, string, string, string, string, string>("Russian", "روسی","ru-RU", "Russian Federation", "USD", "/media/flags/russia.svg"),
            new Tuple<string, string, string, string, string, string>("Chinese", "چینی (ساده شده)","zh-CN", "China", "USD", "/media/flags/china.svg"),
            new Tuple<string, string, string, string, string, string>("French", "فرانسوی","fr-FR", "France", "FRF", "/media/flags/france.svg"),
        };
    }


    public static Tuple<string, string, string, string, string, string> GetLanguage(string lang)
    {
        return LanguagesTuple().Where(x => x.Item3 == lang).FirstOrDefault();
    }

    public static List<Tuple<string, string, string, string, string, string>> GetLanguages(List<string> langs)
    {
        var list = LanguagesTuple();
        return list.Where(x => langs.Contains(x.Item3)).ToList();
    }

    public static List<Tuple<string, string, string, string, string, string>> GetLanguages()
    {
        var list = LanguagesTuple();
        return list.ToList();
    }
}
