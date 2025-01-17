using BlueFrames.Application.Interfaces.Common;
using BlueFrames.Domain.Common.Domain;
using BlueFrames.Persistence.Common.Extensions;
using BlueFrames.Persistence.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlueFrames.Persistence.Common.Database;

internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _appDbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(
        AppDbContext appDbContext,
        IMediator mediator,
        ILogger<UnitOfWork> logger)
    {
        _appDbContext = appDbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        int result;

        try
        {
            result = await _appDbContext.SaveChangesAsync(cancellationToken);
            await _mediator.DispatchDomainEvents(_appDbContext);
        }
        catch (DbUpdateException ex)
        {
            foreach (var entry in ex.Entries)
            {
                entry.State = EntityState.Detached;
            }

            _logger.LogError(ex, "An error occurred while executing your request");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing your request");
            throw;
        }

        return result;
    }

    public void RevertChanges<T>(T entity)
        where T : Entity
    {
        var isDetached = _appDbContext.Entry(entity).State == EntityState.Detached;
        if (isDetached)
        {
            return;
        }

        _appDbContext.Entry(entity).State = EntityState.Unchanged;
    }

    public void ClearChangeTracker()
    {
        _appDbContext.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }
}
