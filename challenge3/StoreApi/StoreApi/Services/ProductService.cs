using StoreApi.Dtos;
using StoreApi.Entities;
using StoreApi.Interfaces;

namespace StoreApi.Services;

public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapProduct).ToList();
    }

    public async Task<IReadOnlyList<ProductDto>> SearchProductsByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "The product name search term is required.",
                nameof(name));
        }

        var products = await productRepository.GetByNameAsync(
            name.Trim(),
            cancellationToken);
        return products.Select(MapProduct).ToList();
    }

    public async Task<IReadOnlyList<ProductDto>> GetAvailableProductsAsync(
        CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAvailableAsync(cancellationToken);
        return products.Select(MapProduct).ToList();
    }

    public async Task<ProductDto?> GetProductAsync(int productId,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(
            productId,
            cancellationToken);
        return product is null ? null : MapProduct(product);
    }

    private static ProductDto MapProduct(Product product) =>
        new(product.Id, product.Name, product.Price, product.Stock);
}
