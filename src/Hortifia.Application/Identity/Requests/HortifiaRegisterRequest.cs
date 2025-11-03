using System;
using System.Collections.Generic;
using System.Text;

namespace Hortifia.Application.Identity.Requests;

/// <summary>
/// The request type for the "/register" endpoint added by <see cref="IdentityApiEndpointRouteBuilderExtensions.MapHortifiaIdentityApi"/>.
/// </summary>
public sealed class HortifiaRegisterRequest
{
    // Properties from original Identity RegisterRequest class (the original class is sealed, that's why we cannot inherit from it):
    public required string Email { get; init; }
    public required string Password { get; init; }


    // Additional properties that HortifiaAPI requires:
    public required string Nickname { get; init; }
    public required double Latitude { get; init; }
    public required double Longtitude { get; init; }
}
