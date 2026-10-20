using Microsoft.AspNetCore.Http.HttpResults;
using StoreApi.Dtos;
using StoreApi.Requests;
using StoreApi.Services;

namespace StoreApi.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/orders").WithTags("Orders");

        group.MapPost("/", PlaceOrder)
            .WithName("PlaceOrder")
            .WithSummary("Place an order")
            .WithDescription("Atomically reserves stock and creates an order. Reusing an idempotency key safely returns the original order.")
            .Produces<OrderDto>(StatusCodes.Status201Created)
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", GetOrders)
            .WithName("GetOrders")
            .WithSummary("View customer orders")
            .WithDescription("Returns all orders belonging to the requested customer.")
            .Produces<IReadOnlyList<OrderDto>>();

        return endpoints;
    }

    private static async Task<Results<Created<OrderDto>, Ok<OrderDto>>> PlaceOrder(PlaceOrderRequest request, IOrderService service,
        CancellationToken cancellationToken)
    {
        var (order, alreadyExists) = await service.PlaceOrderAsync(request, cancellationToken);
        return alreadyExists
            ? TypedResults.Ok(order)
            : TypedResults.Created($"/api/orders/{order.Id}", order);
    }

    private static async Task<Ok<IReadOnlyList<OrderDto>>> GetOrders(string customerId, IOrderService service, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await service.GetOrdersAsync(customerId, cancellationToken));
    }
}
