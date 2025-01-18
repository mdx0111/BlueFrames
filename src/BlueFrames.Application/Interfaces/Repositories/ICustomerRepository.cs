using BlueFrames.Application.Interfaces.Repositories.Common;
using BlueFrames.Domain.Customers;

namespace BlueFrames.Application.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}