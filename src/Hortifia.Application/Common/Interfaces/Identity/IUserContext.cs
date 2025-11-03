using Hortifia.Application.Common.Types;
using System.Security.Claims;

namespace Hortifia.Application.Common.Interfaces.Identity;

public interface IUserContext
{
    ClaimsPrincipal? ClaimsPrincipalUser { get; }
    CurrentUser GetCurrentUser();
}
