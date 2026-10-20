namespace StoreApi.Dtos;

public sealed record ProductDto(
    int Id,
    string Name,
    decimal Price,
    int AvailableStock);
