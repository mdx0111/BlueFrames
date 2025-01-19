using BlueFrames.Application.Interfaces.Repositories.Common;
using BlueFrames.Domain.Products;

namespace BlueFrames.Application.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<List<Product>> GetAllAsync(
        int limit,
        int offset,
        CancellationToken cancellationToken);
}