using Microsoft.AspNetCore.Http.HttpResults;
using StoreApi.Dtos;
using StoreApi.Services;

namespace StoreApi.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetProducts)
            .WithName("GetProducts")
            .WithSummary("Browse products")
            .WithDescription("Returns every product and its current available stock.")
            .Produces<IReadOnlyList<ProductDto>>();

        group.MapGet("/available", GetAvailableProducts)
            .WithName("GetAvailableProducts")
            .WithSummary("Get products available in stock")
            .WithDescription("Returns only products with at least one unit currently available.")
            .Produces<IReadOnlyList<ProductDto>>();

        group.MapGet("/search", SearchProductsByName)
            .WithName("SearchProductsByName")
            .WithSummary("Search products by name")
            .WithDescription("Returns products whose names contain the supplied search term, ignoring letter casing.")
            .Produces<IReadOnlyList<ProductDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{productId:int}", GetProduct)
            .WithName("GetProduct")
            .WithSummary("View product stock")
            .WithDescription("Returns one product and its current available stock.")
            .Produces<ProductDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<Ok<IReadOnlyList<ProductDto>>> GetProducts(IProductService service, CancellationToken cancellationToken) =>
        TypedResults.Ok(await service.GetProductsAsync(cancellationToken));

    private static async Task<Ok<IReadOnlyList<ProductDto>>> SearchProductsByName(string name, IProductService service,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await service.SearchProductsByNameAsync(name, cancellationToken));

    private static async Task<Ok<IReadOnlyList<ProductDto>>> GetAvailableProducts(
        IProductService service,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await service.GetAvailableProductsAsync(cancellationToken));

    private static async Task<Results<Ok<ProductDto>, NotFound>> GetProduct(int productId, IProductService service,
        CancellationToken cancellationToken)
    {
        var product = await service.GetProductAsync(productId, cancellationToken);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
    }
}
