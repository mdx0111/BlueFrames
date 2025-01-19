namespace BlueFrames.Application.Interfaces.Repositories.Common;

public interface IRepository<in T>
{
    void Add(T entity);
    void Update(T entity);
}
