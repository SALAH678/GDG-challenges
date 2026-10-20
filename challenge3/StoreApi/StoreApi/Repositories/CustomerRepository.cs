using StoreApi.Data;
using StoreApi.Entities;

namespace StoreApi.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly Dictionary<string, Customer> _customers =
        CustomerData.SeedCustomers.ToDictionary(customer => customer.Id);

    public Task<Customer?> GetByIdAsync(string customerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }
}