using StoreApi.Entities;

namespace StoreApi.Repositories;

public enum ReserveStockStatus
{
    Reserved,
    ProductNotFound,
    InsufficientStock
}

public sealed record ReserveStockResult(ReserveStockStatus Status, Product? Product = null);

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetAvailableAsync(
        CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken);

    Task<ReserveStockResult> TryReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken);

    Task ReleaseStockAsync(int productId, int quantity, CancellationToken cancellationToken);
}