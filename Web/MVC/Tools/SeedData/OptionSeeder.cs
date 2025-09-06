using Core.Domain.Contracts.Services;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MVC.Tools.SeedData;

public static class OptionSeeder
{
    private static string _siteName;
    private static string _siteUrl;
    public static async Task SeedData(IOptionService service)
    {
        List<Option> options = new List<Option>();
        var allOptions = await service.GetAllAsync();

        if (!(allOptions.Any(x => x.OptionName == "SiteName"))) options.Add(new Option { OptionName = "SiteName", OptionValue = "" });
        if (!(allOptions.Any(x => x.OptionName == "SiteTitle"))) options.Add(new Option { OptionName = "SiteTitle", OptionValue = "" });

    }
}
