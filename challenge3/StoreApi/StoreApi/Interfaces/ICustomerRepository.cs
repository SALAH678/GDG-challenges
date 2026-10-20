using StoreApi.Entities;

namespace StoreApi.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string customerId, CancellationToken cancellationToken);
}
