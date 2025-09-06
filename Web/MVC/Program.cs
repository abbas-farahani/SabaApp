using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infra.Persistence.Context;
using Utilities.Email;
using MVC.Tools.Converter;
using Core.Domain.Contracts.Services;
using Core.Services;
using Core.Domain.Contracts.Repositories;
using Infra.Persistence.Repositories;
using MVC.Tools.Identity;
using Core.Domain.Entities;
using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Contracts.Repositories.Identity;
using Core.Services.Identity;
using Infra.Persistence.Repositories.Identity;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.CookiePolicy;
using System.Globalization;
using Core.Services.Blog;
using Core.Domain.Contracts.Services.Identity.Security;
using Core.Services.Identity.Security;
using MVC.Tools.SeedData;
using MVC.Areas.Dashboard.Hubs;
using MVC.Areas.Dashboard.Services;
using Utilities.HttpContextHelper;
using Core.Domain.Contracts.AppServices;
using Core.AppServices;
using Core.Domain.Contracts.Services.Shop;
using Core.Services.Shop;
using Core.Domain.Contracts.Repositories.Shop;
using Infra.Persistence.Repositories.Shop;
using MVC.Middlewares;
using Microsoft.Extensions.Options;
using MVC.Areas.Admin.Mapping;
using Core.Domain.Mapping;

var builder = WebApplication.CreateBuilder(args);
#region Services

#region DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SabaaConnection")));
#endregion

#region General Services
builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = false;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 3;

})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<PersianIdentityErrors>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    options.SignIn.RequireConfirmedAccount = false;
    options.Tokens.AuthenticatorIssuer = "Haani";
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "HaaniDev";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToLogin = (context) =>
        {
            context.HttpContext.Response.Redirect("/Login");
            return Task.CompletedTask;
        }
    };
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/AccessDenied";
    options.LoginPath = "/Login";
    options.LogoutPath = "/LogOut";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(10);
});

// Add services to the container.
builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization()
    .AddViewOptions(options =>
{
    options.ClientModelValidatorProviders.Clear();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDataProtection().SetApplicationName("Haani");

//builder.Services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication=false; });
builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 2147483647); // 2GB
builder.Services.AddSignalR();

#endregion

#region WebHost Service
//builder.WebHost.ConfigureKestrel(options =>
//{
//	options.ListenAnyIP(5000); // HTTP
//	options.ListenAnyIP(5001, listenOptions =>
//	{
//		listenOptions.UseHttps(); // HTTPS
//	});
//});
#endregion


#region Custom Repositories
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

#region Blog (CMS)
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAttachmentMetaRepository, AttachmentMetaRepository>();
builder.Services.AddScoped<IOptionRepository, OptionRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentMetaRepository, CommentMetaRepository>();
builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<IPageMetaRepository, PageMetaRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostMetaRepository, PostMetaRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
#endregion

#region Identity
builder.Services.AddScoped<Core.Domain.Contracts.Repositories.Identity.IUserRepository, Infra.Persistence.Repositories.Identity.UserRepository>();
builder.Services.AddScoped<IUserClaimRepository, UserClaimRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

#endregion

#region Shop
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCatRepository, ProductCatRepository>();
builder.Services.AddScoped<IProductMetaRepository, ProductMetaRepository>();
#endregion

#endregion

#region Custom Services
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
builder.Services.AddScoped<IToolsService, ToolsService>();
builder.Services.AddScoped<ITextProcessingService, TextProcessingService>();
//builder.Services.AddSingleton<ITradingSignalService, TradingSignalService>();
//builder.Services.AddHostedService(provider => provider.GetRequiredService<ITradingSignalService>() as TradingSignalService);


#region App Services
//builder.Services.AddScoped<IOrderAppService, OrderAppService>();
//builder.Services.AddScoped<IShopAppService, ShopAppService>();
#endregion

#region Blog (CMS)
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IAttachmentMetaService, AttachmentMetaService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentMetaService, CommentMetaService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IPageMetaService, PageMetaService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostMetaService, PostMetaService>();
builder.Services.AddScoped<IOptionService, OptionService>();
#endregion

#region Identity
builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsPrincipalFactory>();
builder.Services.AddScoped<Core.Domain.Contracts.Services.Identity.IUserService, Core.Services.Identity.UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
#endregion

#region Shop
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductCatService, ProductCatService>();
builder.Services.AddScoped<IProductMetaService, ProductMetaService>();
#endregion


#region MVC(UI) Services 
//builder.Services.AddHostedService<TradingSignalService>();
#endregion

#endregion

#region AutoMapper
builder.Services.AddAutoMapper(config =>
{
    #region Application Layer
    config.AddProfile<PostDtoMapping>(); // dto ↔ entity
    config.AddProfile<CommentDtoMapping>(); // dto ↔ entity
    config.AddProfile<PageDtoMapping>(); // dto ↔ entity
    
    #endregion

    #region Presentation Layer
    config.AddProfile<PostViewModelMapping>(); // viewModel ↔ dto
    config.AddProfile<PageViewModelMapping>(); // viewModel ↔ dto
    #endregion

});
#endregion

#region Tools 
builder.Services.AddScoped<IEmailManagement, EmailManagement>();
builder.Services.AddScoped<IViewToStringService, ViewToStringService>();

#endregion

#endregion


var app = builder.Build();
#region Application
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}
app.UseHttpsRedirection();
app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});
CultureInfo.DefaultThreadCurrentCulture = PersianDateExtensionMethods.GetPersianCulture(); // Change default culture for datetime

var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
HttpContextHelper.Configure(httpContextAccessor);


#region Statics
app.UseStaticFiles();
//var dashboardPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "Areas", "Dashboard", "wwwroot"));
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(dashboardPath),
//    RequestPath = "/DashboardStatic"
//});
// var adminPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "Areas", "Admin", "wwwroot"));
// app.UseStaticFiles(new StaticFileOptions
// {
// FileProvider = new PhysicalFileProvider(adminPath),
// RequestPath = "/AdminStatic"
// });

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // جلوگیری از دسترسی به فایلهای سیستمی
        if (ctx.File.PhysicalPath.Contains("System32"))
        {
            ctx.Context.Response.StatusCode = 403;
            ctx.Context.Response.ContentLength = 0;
            ctx.Context.Response.Body = Stream.Null;
        }
    }
});
#endregion

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

#region Multilingual
app.UseRequestLocalization();
app.UseMiddleware<LocalizationMiddleware>();
#endregion


#region Routes
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "dashboard",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<CryptoHub>("/crypto");
#endregion

#region Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    var optionManager = services.GetRequiredService<IOptionService>();
    var permissionManager = services.GetRequiredService<IPermissionService>();
    await DataSeeder.SeedUser(userManager, roleManager);
    await DataSeeder.SeedPermissions(permissionManager);

    await OptionSeeder.SeedData(optionManager);
}
#endregion

app.Run();
#endregion

