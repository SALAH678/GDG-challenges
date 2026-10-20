using StoreApi.Dtos;
using StoreApi.Entities;
using StoreApi.Exceptions;
using StoreApi.Interfaces;
using StoreApi.Requests;

namespace StoreApi.Services;

public sealed class OrderService(
    ICustomerRepository customerRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    TimeProvider timeProvider) : IOrderService
{
    private readonly SemaphoreSlim _orderLock = new(1, 1);

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string customerId, CancellationToken cancellationToken)
    {
        if (await customerRepository.GetByIdAsync(customerId, cancellationToken) is null)
        {
            throw new CustomerNotFoundException(customerId);
        }

        var orders = await orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return orders.Select(MapOrder).ToList();
    }

    public async Task<(OrderDto Order, bool AlreadyExists)> PlaceOrderAsync(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        await _orderLock.WaitAsync(cancellationToken);
        try
        {
            return await PlaceOrderCoreAsync(request, cancellationToken);
        }
        finally
        {
            _orderLock.Release();
        }
    }

    private async Task<(OrderDto Order, bool AlreadyExists)> PlaceOrderCoreAsync(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        var existing = await orderRepository.FindByIdempotencyKeyAsync(request.CustomerId, request.IdempotencyKey, cancellationToken);

        if (existing is not null)
        {
            return (ReplayExisting(existing, request), true);
        }

        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new CustomerNotFoundException(request.CustomerId);

        var reservation = await productRepository.TryReserveStockAsync(request.ProductId, request.Quantity, cancellationToken);

        var product = reservation.Status switch
        {
            ReserveStockStatus.Reserved => reservation.Product!,
            ReserveStockStatus.ProductNotFound =>
                throw new ProductNotFoundException(request.ProductId),
            _ => throw new ProductSoldOutException(request.ProductId)
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            ProductId = product.Id,
            Product = product,
            ProductName = product.Name,
            Quantity = request.Quantity,
            TotalPrice = product.Price * request.Quantity,
            CreatedAt = timeProvider.GetUtcNow(),
            IdempotencyKey = request.IdempotencyKey
        };

        customer.Orders.Add(order);
        product.Orders.Add(order);

        Order stored;
        bool created;
        try
        {
            (stored, created) = await orderRepository.AddIfAbsentAsync(order, CancellationToken.None);
        }
        catch
        {
            await productRepository.ReleaseStockAsync(product.Id, request.Quantity, CancellationToken.None);
            throw;
        }

        if (created)
        {
            return (MapOrder(stored), false);
        }

        await productRepository.ReleaseStockAsync(product.Id, request.Quantity, CancellationToken.None);
        return (ReplayExisting(stored, request), true);
    }

    private static OrderDto ReplayExisting(Order existing, PlaceOrderRequest request)
    {
        if (existing.ProductId != request.ProductId ||
            existing.Quantity != request.Quantity)
        {
            throw new IdempotencyConflictException(request.IdempotencyKey);
        }

        return MapOrder(existing);
    }

    private static OrderDto MapOrder(Order order) =>
        new (
            order.Id,
            order.CustomerId,
            order.ProductId,
            order.ProductName,
            order.Quantity,
            order.TotalPrice
        );
}
