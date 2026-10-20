namespace StoreApi.Dtos;

/// <summary>Represents an order returned by the API.</summary>
public sealed record OrderDto(
    Guid Id,
    string CustomerId,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal TotalPrice);
