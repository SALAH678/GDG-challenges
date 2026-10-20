using StoreApi.Entities;

namespace StoreApi.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);

    Task<Order?> FindByIdempotencyKeyAsync(
        string customerId,
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<(Order Order, bool Created)> AddIfAbsentAsync(Order order, CancellationToken cancellationToken);
}