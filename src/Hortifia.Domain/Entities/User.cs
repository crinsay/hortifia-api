using Microsoft.AspNetCore.Identity;

namespace Hortifia.Domain.Entities;

public class User : IdentityUser
{
    public string NickName { get; set; } = default!;
}
