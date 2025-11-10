using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IPostsRepository
{
    Task<int> CreateAsync(Post post);
}
