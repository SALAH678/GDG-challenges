namespace StoreApi.Entities;

public sealed class Product
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required int Stock { get; set; }
    public ICollection<Order> Orders { get; } = [];
}