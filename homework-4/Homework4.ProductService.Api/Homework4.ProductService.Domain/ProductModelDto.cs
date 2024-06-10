namespace Homework4.ProductService.Domain;

public record ProductModelDto
(
    string Name,
    double Price,
    double Weight,
    ProductType Type,
    DateTimeOffset DateCreation,
    long WarehouseId
);