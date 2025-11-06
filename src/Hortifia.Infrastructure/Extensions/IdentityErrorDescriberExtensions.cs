using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

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
