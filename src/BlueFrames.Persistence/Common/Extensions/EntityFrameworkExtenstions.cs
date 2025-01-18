using System.Linq.Expressions;
using System.Transactions;

namespace BlueFrames.Persistence.Common.Extensions;

public static class EntityFrameworkExtensions
{
    public static List<T> ToListWithNoLock<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> expression = null)
    {
        using var scope = CreateTransaction();
        if (expression is not null)
        {
            query = query.Where(expression);
        }
        var result = query.ToList();
        scope.Complete();
        return result;
    }

    public static async Task<List<T>> ToListWithNoLockAsync<T>(
        this IQueryable<T> query,
        CancellationToken cancellationToken = default)
    {
        return await query.ToListWithNoLockAsync(
            null,
            cancellationToken);
    }

    public static async Task<List<T>> ToListWithNoLockAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> expression = null,
        CancellationToken cancellationToken = default)
    {
        using var scope = CreateTransactionAsync();
        if (expression is not null)
        {
            query = query.Where(expression);
        }
        var result = await query.ToListAsync(cancellationToken);
        scope.Complete();
        return result;
    }

    public static T FirstOrDefaultWithNoLock<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> expression = null)
    {
        using var scope = CreateTransaction();
        if (expression is not null)
        {
            query = query.Where(expression);
        }
        var result = query.FirstOrDefault();
        scope.Complete();
        return result;
    }

    public static async Task<T> FirstOrDefaultWithNoLockAsync<T>(
        this IQueryable<T> query,
        CancellationToken cancellationToken = default)
    {
        return await query.FirstOrDefaultWithNoLockAsync(
            null,
            cancellationToken);
    }

    public static async Task<T> FirstOrDefaultWithNoLockAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> expression = null,
        CancellationToken cancellationToken = default)
    {
        using var scope = CreateTransactionAsync();
        if (expression is not null)
        {
            query = query.Where(expression);
        }
        var result = await query.FirstOrDefaultAsync(cancellationToken);
        scope.Complete();
        return result;
    }

    public static int CountWithNoLock<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> expression = null)
    {
        using var scope = CreateTransaction();
        if (expression is not null)
        {
            query = query.Where(expression);
        }
        var toReturn = query.Count();
        scope.Complete();
        return toReturn;
    }

    public static async Task<int> CountWithNoLockAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> expression = null,
        CancellationToken cancellationToken = default)
    {
        using var scope = CreateTransactionAsync();
        if (expression is not null)
        {
            query = query.Where(expression);
        }
        var toReturn = await query.CountAsync(cancellationToken);
        scope.Complete();
        return toReturn;
    }

    private static TransactionScope CreateTransactionAsync()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadUncommitted
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }

    private static TransactionScope CreateTransaction()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadUncommitted
            });
    }
}