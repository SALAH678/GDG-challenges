namespace StoreApi.Exceptions;

public sealed class CustomerNotFoundException(string customerId)
    : Exception($"Customer with ID '{customerId}' was not found.")
{
    public string CustomerId { get; } = customerId;
}

public sealed class ProductNotFoundException(int productId)
    : Exception($"Product with ID '{productId}' was not found.")
{
    public int ProductId { get; } = productId;
}

public sealed class ProductSoldOutException(int productId)
    : Exception($"Product with ID '{productId}' does not have enough stock.")
{
    public int ProductId { get; } = productId;
}

public sealed class IdempotencyConflictException(string idempotencyKey)
    : Exception($"Idempotency key '{idempotencyKey}' was already used for a different request.")
{
    public string IdempotencyKey { get; } = idempotencyKey;
}
