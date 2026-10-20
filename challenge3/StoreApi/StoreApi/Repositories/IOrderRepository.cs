using StoreApi.Entities;

namespace StoreApi.Repositories;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);

    Task<Order?> FindByIdempotencyKeyAsync(
        string customerId,
        string idempotencyKey,
        CancellationToken cancellationToken);

    /// <summary>
    /// Atomically stores the order unless one already exists for the same
    /// (customer, idempotency key). Returns the stored order and whether it was newly created.
    /// </summary>
    Task<(Order Order, bool Created)> AddIfAbsentAsync(Order order, CancellationToken cancellationToken);
}