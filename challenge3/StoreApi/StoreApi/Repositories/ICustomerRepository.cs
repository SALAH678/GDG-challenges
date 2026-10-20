using StoreApi.Entities;

namespace StoreApi.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string customerId, CancellationToken cancellationToken);
}
