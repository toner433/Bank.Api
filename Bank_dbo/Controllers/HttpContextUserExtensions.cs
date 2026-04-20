using System;
using System.Security.Claims;

namespace Bank.API.Controllers
{
    internal static class HttpContextUserExtensions
    {
        public static Guid? GetCurrentUserId(this ClaimsPrincipal user)
        {
            var v = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(v, out var g) ? g : null;
        }
    }
}
