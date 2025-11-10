using Hortifia.Application.Common.Interfaces;
using Hortifia.Domain.Common;

namespace Hortifia.Infrastructure.Persistence;

internal class UnitOfWork(HortifiaDbContext dbContext /*ILogger<UnitOfWork> logger*/) : IUnitOfWork
{
    public async Task ExecuteTransactionalAsync(Func<Task> operation)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await operation();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            throw new Exception("An unexpected error occured while saving changes.", ex);
        }
    }

    public async Task ExecuteTransactionalAsync(Func<Task<Result>> operation)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await operation();
            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            throw new Exception("An unexpected error occured while saving changes.", ex);
        }
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
