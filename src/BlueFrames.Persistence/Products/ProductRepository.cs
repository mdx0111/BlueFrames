using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;
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
        _appDbContext.Products.Update(entity);
    }

    public async Task<Product> GetByIdAsync(ProductId id, CancellationToken cancellationToken)
    {
        return await _appDbContext
            .Products
            .FindAsync([id], cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        return await _appDbContext
            .Products
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}