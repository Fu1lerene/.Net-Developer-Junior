namespace Homework2.ProductService.Domain;

public class ProductModel
{
    public long ProductId { get; }
    public string Name { get; set; }
    public double Price { get; set; }
    public double Weight { get; set; }
    public ProductType Type { get; set; }
    public DateTimeOffset DateCreation { get; init; }
    public long WarehouseId { get; init; }

    private static long _id = 1;

    public ProductModel()
    {
        ProductId = _id++;
    }
}