using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hortifia.Infrastructure.Extensions
{
    public static class IdentityErrorDescriberExtensions
    {
        public static IdentityError InvalidNickname(this IdentityErrorDescriber _, string? nickname)
        {
            return new IdentityError
            {
                Code = nameof(InvalidNickname),
                Description = $"Nickname '{nickname}' is invalid - make sure it is not empty."
            };
        }

        public static IdentityError InvalidCoordinates(this IdentityErrorDescriber _, string errorMessage)
        {
            return new IdentityError
            {
                Code = nameof(InvalidCoordinates),
                Description = $"Coordinates are invalid - {errorMessage}"
            };
        }
    }
}
