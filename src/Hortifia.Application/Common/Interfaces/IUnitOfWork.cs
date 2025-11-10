using Hortifia.Domain.Common;

namespace Hortifia.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task ExecuteTransactionalAsync(Func<Task> operation);
    Task ExecuteTransactionalAsync(Func<Task<Result>> operation);
    Task SaveChangesAsync();
}
