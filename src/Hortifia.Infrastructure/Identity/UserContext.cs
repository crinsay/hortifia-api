using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Types;
using Hortifia.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Hortifia.Infrastructure.Identity;

internal class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public ClaimsPrincipal? ClaimsPrincipalUser
    {
        get => httpContextAccessor?.HttpContext?.User;
    }

    public CurrentUser GetCurrentUser()
    {
        var user = ClaimsPrincipalUser
            ?? throw new InvalidOperationException("User context is not present");

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return CreateCurrentUser(
                id: null);
        }

        var userId = user.GetUserId();

        return CreateCurrentUser(
            id: userId);
    }
    private static CurrentUser CreateCurrentUser(string? id)
    {
        return new CurrentUser
        {
            Id = id
        };
    }

}
