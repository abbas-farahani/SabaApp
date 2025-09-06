using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.User;

public static class GetCurrentUserId
{
    public static string GetUserId(this ClaimsPrincipal? claim)
    {
        var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
        return userId;
    }
}
