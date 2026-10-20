using StoreApi.Entities;

namespace StoreApi.Data;

public static class CustomerData
{
    public static IReadOnlyList<Customer> SeedCustomers { get; } =
    [
        new() { Id = "customer-1", Name = "Alice Johnson", Email = "alice@example.com" },
        new() { Id = "customer-2", Name = "Bob Smith", Email = "bob@example.com" }
    ];
}
