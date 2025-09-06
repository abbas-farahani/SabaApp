using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MVC.Areas.Admin.Helpers.Attributes;

public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionAttribute(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Claims.Any(c => c.Type == "IsAdmin")) // Defined in CustomClaimsPrincipalFactory Class at Core.Services.Identity.Security 
            if (!user.Claims.Any(c => c.Type == "Permission" && c.Value == _permission)) // Defined in RoleService
                context.Result = new ForbidResult();
    }
}
