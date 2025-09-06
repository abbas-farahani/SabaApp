using Core.Domain.Contracts.Services.Identity;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MVC.Tools.SeedData;

public static class DataSeeder
{
    public static async Task SeedUser(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        // بررسی وجود نقش‌های پیش‌فرض
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = "Admin",
            });
        }

        if (!await roleManager.RoleExistsAsync("Subscriber"))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = "Subscriber",
            });
        }

        // بررسی وجود کاربر admin
        if (await userManager.FindByNameAsync("admin") == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public static async Task SeedPermissions(IPermissionService permissionManager)
    {
        List<string> permissions = new List<string>()
        {
            "Post.Create","Post.Read","Post.Update","Post.Delete",
            "Post.Others.Create","Post.Others.Read","Post.Others.Update","Post.Others.Delete",

            "Category.Create","Category.Read","Category.Update","Category.Delete",
            "Category.Others.Create","Category.Others.Read","Category.Others.Update","Category.Others.Delete",

            "Page.Create","Page.Read","Page.Update","Page.Delete",
            "Page.Others.Create","Page.Others.Read","Page.Others.Update","Page.Others.Delete",

            "Comment.Create","Comment.Read","Comment.Update","Comment.Delete",
            "Comment.Others.Create","Comment.Others.Read","Comment.Others.Update","Comment.Others.Delete",

            "Media.Create","Media.Read","Media.Update","Media.Delete",
            "Media.Others.Create","Media.Others.Read","Media.Others.Update","Media.Others.Delete",

            "Option.Create", "Option.Read", "Option.Update", "Option.Delete",
            "Role.Create", "Role.Read", "Role.Update", "Role.Delete",
            "Admin.Read",
        };

        List<Permission> newItems = new List<Permission>();
        permissions.Select(x =>
        {
            newItems.Add(new Permission
            {
                Name = x,
                Description = $"{x.Replace('.', ' ')} Description",
                CreationTime = DateTime.Now,
                LastModified = null,
            });
            return x;
        }).ToList();

        if (newItems.Count > 0) await permissionManager.AddAsync(newItems);
    }
}
