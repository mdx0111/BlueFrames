using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Persistence.DataContext;

namespace BlueFrames.Persistence.Customers;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _appDbContext;

    public CustomerRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void Add(Customer entity)
    {
        _appDbContext.Customers.Add(entity);
    }

    public void Update(Customer entity)
    {
        _appDbContext.Customers.Update(entity);
    }

    public async Task<Customer> GetByIdAsync(CustomerId id, CancellationToken cancellationToken)
    {
        return await _appDbContext
            .Customers
            .FindAsync([id], cancellationToken);
    }

    public async Task<List<Customer>> GetAllAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        return await _appDbContext
            .Customers
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}