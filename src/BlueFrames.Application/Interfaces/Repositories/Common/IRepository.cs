namespace BlueFrames.Application.Interfaces.Repositories.Common;

public interface IRepository<in T>
{
    void AddOrUpdate(T entity);
}
