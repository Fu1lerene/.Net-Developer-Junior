namespace Homework2.ProductService.Domain;

public class ProductFilter
{
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
    public ProductType Type { get; init; }
    public long WarehouseId { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}