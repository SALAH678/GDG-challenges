using StoreApi.Dtos;

namespace StoreApi.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductDto>> SearchProductsByNameAsync(string name, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductDto>> GetAvailableProductsAsync( CancellationToken cancellationToken);
    Task<ProductDto?> GetProductAsync(int productId, CancellationToken cancellationToken);
}
