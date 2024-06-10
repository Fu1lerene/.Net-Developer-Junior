using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Homework4.ProductService.Api.Application;
using Homework4.ProductService.Api.Exceptions;
using Homework4.ProductService.Api.Extensions;
using Homework4.ProductService.Domain;

namespace Homework4.ProductService.Api.GrpcServices;

public class ProductServiceGrpc : ProductService.ProductServiceBase
{
    private readonly IProductService _productService;

    public ProductServiceGrpc(IProductService productService)
    {
        _productService = productService;
    }

    public override Task<SetPriceResponse> SetPrice(SetPriceRequest request, ServerCallContext context)
    {
        var product = _productService.GetProduct(request.ProductId);
        
        if (product == null)
        {
            throw new RpcProductNotFoundException(request.ProductId);
        }
        
        _productService.SetPrice(new SetPriceModel
        {
            Price = request.Price,
            ProductId = request.ProductId
        });
        
        return Task.FromResult(new SetPriceResponse
        {
            Response = "Цена товара обновлена"
        });
    }

    public override Task<AddProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
    {
        var product = new ProductModelDto
        (
            request.Name,
            request.Price,
            request.Weight,
            request.ProductType.GetProductType(),
            request.DateCreation.ToDateTimeOffset(),
            request.WarehouseId
        );
        
        var productId = _productService.AddProduct(product);
        return Task.FromResult(new AddProductResponse
        {
            Response = $"Товар с {productId} ID успешно добавлен"
        });
    }
    
    
    public override Task<ListProductResponse> GetAllProducts(Empty request, ServerCallContext context)
    {
        var products = _productService.GetAllProducts();
        return Task.FromResult(new ListProductResponse
        {
            Products =
            {
                products.Select(p => new GrpcProductModel
                {
                    Name = p.Name,
                    DateCreation = Timestamp.FromDateTimeOffset(p.DateCreation),
                    Price = p.Price,
                    ProductId = p.ProductId,
                    ProductType = p.Type.GetGrpcProductType(),
                    WarehouseId = p.WarehouseId,
                    Weight = p.Weight
                })
            }
        });
    }

    public override Task<ListProductResponse> GetFilteredList(GetFilteredListRequest request, ServerCallContext context)
    {
        var products = _productService.GetFilteredList(new ProductFilter
        {
            StartDate = request.StartDate.ToDateTimeOffset(),
            EndDate = request.EndDate.ToDateTimeOffset(),
            Type = request.ProductType.GetProductType(),
            WarehouseId = request.WarehouseId,
            Page = request.Page,
            PageSize = request.PageSize
        });
        
        return Task.FromResult(new ListProductResponse
        {
            Products =
            {
                products.Select(p => new GrpcProductModel
                {
                    Name = p.Name,
                    DateCreation = Timestamp.FromDateTimeOffset(p.DateCreation),
                    Price = p.Price,
                    ProductId = p.ProductId,
                    ProductType = p.Type.GetGrpcProductType(),
                    WarehouseId = p.WarehouseId,
                    Weight = p.Weight
                })
            }
        });
    }

    public override Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var product = _productService.GetProduct(request.ProductId);

        if (product == null)
        {
            throw new RpcProductNotFoundException(request.ProductId);
        }
        
        return Task.FromResult(new GetProductResponse
        {
            Product =
                new GrpcProductModel {
                    ProductId = product.ProductId,
                    ProductType = product.Type.GetGrpcProductType(),
                    WarehouseId = product.WarehouseId,
                    Weight = product.Weight,
                    DateCreation = Timestamp.FromDateTimeOffset(product.DateCreation),
                    Price = product.Price,
                    Name = product.Name
                }
        });
    }
}