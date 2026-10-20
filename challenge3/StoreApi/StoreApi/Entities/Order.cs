namespace StoreApi.Entities;

public sealed class Order
{
    public required Guid Id { get; init; }
    public required string CustomerId { get; init; }
    public Customer? Customer { get; init; }
    public required int ProductId { get; init; }
    public Product? Product { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal TotalPrice { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string IdempotencyKey { get; init; }
}