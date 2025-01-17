namespace BlueFrames.Application.Interfaces.Common;

public interface IUnitOfWork
{
    void ClearChangeTracker();
    void RevertChanges<T>(T fixture) where T : BlueFrames.Domain.Common.Domain.Entity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}