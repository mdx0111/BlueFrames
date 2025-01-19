using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Customers;
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
        throw new NotImplementedException();
    }

    public Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Customer>> GetAllAsync(int limit, int offset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}