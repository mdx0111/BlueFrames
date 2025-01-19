using BlueFrames.Application.Interfaces.Repositories.Common;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Application.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> GetByIdAsync(
        CustomerId id,
        CancellationToken cancellationToken);

    Task<List<Customer>> GetAllAsync(
        int limit,
        int offset,
        CancellationToken cancellationToken);
}