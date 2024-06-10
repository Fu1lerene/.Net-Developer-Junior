using Homework2.ProductService.Domain;

namespace Homework2.ProductService.Api.Extensions;

public static class EnumExtension
{
    public static ProductType GetProductType(this GrpcProductType grpcProductType)
    {
        return grpcProductType switch
        {
            GrpcProductType.Groceries => ProductType.Groceries,
            GrpcProductType.Appliances => ProductType.Appliances,
            GrpcProductType.General => ProductType.General,
            GrpcProductType.Chemicals => ProductType.Chemicals,
            _ => ProductType.Default
        };
    }
    
    public static GrpcProductType GetGrpcProductType(this ProductType productType)
    {
        return productType switch
        {
            ProductType.Groceries => GrpcProductType.Groceries,
            ProductType.Appliances => GrpcProductType.Appliances,
            ProductType.General => GrpcProductType.General,
            ProductType.Chemicals => GrpcProductType.Chemicals,
            _ => GrpcProductType.Default
        };
    }
}