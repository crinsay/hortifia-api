using Hortifia.Domain.Interfaces;
using Hortifia.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Hortifia.Infrastructure.Authorization.Requirements.MustBeOwner;

internal class MustBeOwnerRequirementHandler : AuthorizationHandler<MustBeOwnerRequirement, IOwnedResource>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeOwnerRequirement requirement, IOwnedResource resource)
    {
        var currentUserId = context.User.GetUserId();

        if (currentUserId == resource.OwnerId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
