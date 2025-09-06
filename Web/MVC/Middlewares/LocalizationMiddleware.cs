using System;
using Core.Domain.Contracts.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Threading;

namespace MVC.Middlewares;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _memory;
    public LocalizationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, IMemoryCache memory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _memory = memory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string cookie = string.Empty;

        if (context.Request.Cookies.TryGetValue("Language", out cookie))
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cookie);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie);
        }
        else
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }
        //using (var scope = _scopeFactory.CreateScope())
        //{
        //    var _options = scope.ServiceProvider.GetRequiredService<IOptionService>();
        //}

        await _next(context);
    }
}

