using StoreApi.Data;
using StoreApi.Entities;

namespace StoreApi.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<int, Product> _products =
        ProductData.SeedProducts.ToDictionary(product => product.Id, Clone);

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Product> products;
        lock (_syncRoot)
        {
            products = _products.Values.OrderBy(product => product.Id).Select(Clone).ToList();
        }

        return Task.FromResult(products);
    }

    public Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Product? product;
        lock (_syncRoot)
        {
            product = _products.TryGetValue(productId, out var stored) ? Clone(stored) : null;
        }

        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<Product>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Product> products;
        lock (_syncRoot)
        {
            products = _products.Values
                .Where(product => product.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(product => product.Id)
                .Select(Clone)
                .ToList();
        }

        return Task.FromResult(products);
    }

    public Task<IReadOnlyList<Product>> GetAvailableAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Product> products;
        lock (_syncRoot)
        {
            products = _products.Values
                .Where(product => product.Stock > 0)
                .OrderBy(product => product.Id)
                .Select(Clone)
                .ToList();
        }

        return Task.FromResult(products);
    }

    public Task<ReserveStockResult> TryReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            if (!_products.TryGetValue(productId, out var stored))
            {
                return Task.FromResult(new ReserveStockResult(ReserveStockStatus.ProductNotFound));
            }

            if (stored.Stock < quantity)
            {
                return Task.FromResult(new ReserveStockResult(ReserveStockStatus.InsufficientStock));
            }

            stored.Stock -= quantity;
            return Task.FromResult(new ReserveStockResult(ReserveStockStatus.Reserved, Clone(stored)));
        }
    }

    public Task ReleaseStockAsync(int productId, int quantity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            if (_products.TryGetValue(productId, out var stored))
            {
                stored.Stock += quantity;
            }
        }

        return Task.CompletedTask;
    }

    private static Product Clone(Product product) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
}