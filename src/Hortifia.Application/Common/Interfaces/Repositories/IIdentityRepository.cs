using Hortifia.Application.Identity.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IIdentityRepository
{
    public Task<UserDataResponse> GetUserDataById(string userId);
}
