using Microsoft.AspNetCore.Identity;

namespace Hortifia.Infrastructure.Extensions
{
    public static class IdentityErrorDescriberExtensions
    {
        public static IdentityError InvalidUserData(this IdentityErrorDescriber _, string errorMessage)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserData),
                Description = errorMessage
            };
        }
    }
}
