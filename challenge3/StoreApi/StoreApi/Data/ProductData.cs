using StoreApi.Entities;

namespace StoreApi.Data;

public static class ProductData
{
    public static IReadOnlyList<Product> SeedProducts { get; } =
    [
        new() { Id = 1, Name = "Wireless Mouse", Price = 29.99m, Stock = 5 },
        new() { Id = 2, Name = "Mechanical Keyboard", Price = 89.99m, Stock = 12 },
        new() { Id = 3, Name = "USB-C Hub", Price = 39.99m, Stock = 0 }
    ];
}
