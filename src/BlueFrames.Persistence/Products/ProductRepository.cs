using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Products;

namespace BlueFrames.Persistence.Products;

public class ProductRepository : IProductRepository
{
    public void Add(Product entity)
    {
        throw new NotImplementedException();
    }

    public void Update(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetAllAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}