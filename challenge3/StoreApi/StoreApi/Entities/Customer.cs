namespace StoreApi.Entities;

public sealed class Customer
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public ICollection<Order> Orders { get; } = [];
}