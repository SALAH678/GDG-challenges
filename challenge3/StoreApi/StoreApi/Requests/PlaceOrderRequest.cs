using System.ComponentModel.DataAnnotations;

namespace StoreApi.Requests;

/// <summary>Payload used to purchase product stock.</summary>
public sealed record PlaceOrderRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public required string CustomerId { get; init; }

    [Range(1, int.MaxValue)]
    public required int ProductId { get; init; }

    [Range(1, int.MaxValue)]
    public required int Quantity { get; init; }

    [Required, StringLength(200, MinimumLength = 1)]
    public required string IdempotencyKey { get; init; }
}
