using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Products;
using BlueFrames.Persistence.DataContext;

namespace BlueFrames.Persistence.Products;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _appDbContext;

    public ProductRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public void Add(Product entity)
    {
        _appDbContext.Products.Add(entity);
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