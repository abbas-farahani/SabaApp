using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Utilities.StaticServiceCaller;

public static class StaticServiceCaller
{
    private static IServiceProvider _serviceProvider;
    private static IServiceCollection _serviceCollection;

    public static void Initialize(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public static T GetService<T>()
    {
        if (_serviceProvider == null)
            return (dynamic)null;
        return _serviceProvider.GetRequiredService<T>();
    }

    public static void RegisterService<TService, TImplementaion>() where TService : class where TImplementaion : class, TService
    {
        if (_serviceCollection == null)
            throw new InvalidOperationException("Service is not Initialized");
        _serviceCollection.AddScoped<TService, TImplementaion>();
        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    public static HttpContext HttpContext => GetService<IHttpContextAccessor>()?.HttpContext;
}
