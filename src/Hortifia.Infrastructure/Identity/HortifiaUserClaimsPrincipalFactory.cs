using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Hortifia.Infrastructure.Identity;

public class HortifiaUserClaimsPrincipalFactory(UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<User, IdentityRole>(userManager, roleManager, options)
{
    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var claimsIdentity = await GenerateClaimsAsync(user);

        var preferredNotificationTime = new Claim(HortifiaClaimTypes.PreferredNotificationTime, user.PreferredNotificationTime.ToString());

        claimsIdentity.AddClaim(preferredNotificationTime);

        return new ClaimsPrincipal(claimsIdentity);
    }
}
