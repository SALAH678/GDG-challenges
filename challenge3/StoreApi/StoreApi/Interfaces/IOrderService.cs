using StoreApi.Dtos;
using StoreApi.Requests;

namespace StoreApi.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetOrdersAsync(
        string customerId,
        CancellationToken cancellationToken);
    Task<(OrderDto Order, bool AlreadyExists)> PlaceOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken);
}
