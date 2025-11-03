using Hortifia.Domain.Constants;
using System.Security.Claims;

namespace Hortifia.Infrastructure.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal? user)
    {
        return user?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }


    public static string? GetUserNickName(this ClaimsPrincipal? user)
    {
        return user?.FindFirst(c => c.Type == CustomClaimTypes.NickName)?.Value;
    }
}
