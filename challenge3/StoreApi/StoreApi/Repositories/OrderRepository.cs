using StoreApi.Entities;
using StoreApi.Interfaces;

namespace StoreApi.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<(string CustomerId, string Key), Order> _byIdempotencyKey = [];
    private readonly Dictionary<string, List<Order>> _byCustomer = [];

    public Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Order> orders;
        lock (_syncRoot)
        {
            orders = _byCustomer.TryGetValue(customerId, out var list)
                ? list.OrderByDescending(order => order.CreatedAt).ToList()
                : [];
        }

        return Task.FromResult(orders);
    }

    public Task<Order?> FindByIdempotencyKeyAsync(string customerId, string idempotencyKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            _byIdempotencyKey.TryGetValue((customerId, idempotencyKey), out var order);
            return Task.FromResult(order);
        }
    }

    public Task<(Order Order, bool Created)> AddIfAbsentAsync(Order order, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_syncRoot)
        {
            var key = (order.CustomerId, order.IdempotencyKey);

            if (_byIdempotencyKey.TryGetValue(key, out var existing))
            {
                return Task.FromResult<(Order Order, bool Created)>((existing, false));
            }

            _byIdempotencyKey.Add(key, order);

            if (!_byCustomer.TryGetValue(order.CustomerId, out var customerOrders))
            {
                customerOrders = [];
                _byCustomer[order.CustomerId] = customerOrders;
            }

            customerOrders.Add(order);
            return Task.FromResult<(Order Order, bool Created)>((order, true));
        }
    }
}