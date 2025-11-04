using System.Security.Claims;

namespace Hortifia.Infrastructure.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal? user)
    {
        return user?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
