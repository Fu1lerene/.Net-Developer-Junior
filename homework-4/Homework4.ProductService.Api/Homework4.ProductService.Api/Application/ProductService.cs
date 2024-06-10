using System;
using System.Collections.Generic;
using Homework4.ProductService.Api.Exceptions;
using Homework4.ProductService.Domain;
using Homework4.ProductService.Infrastructure;

namespace Homework4.ProductService.Api.Application;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public long AddProduct(ProductModelDto productDto)
    {
        var product = new ProductModel
        {
            Name = productDto.Name,
            DateCreation = productDto.DateCreation,
            Price = productDto.Price,
            Type = productDto.Type,
            WarehouseId = productDto.WarehouseId,
            Weight = productDto.Weight
        };

        _productRepository.AddProduct(product);
        return product.ProductId;
    }

    public IEnumerable<ProductModel> GetAllProducts()
    {
        return _productRepository.GetAllProducts();
    }

    public void SetPrice(SetPriceModel setPriceModel)
    {
        try
        {
            _productRepository.SetPrice(setPriceModel);
        }
        catch (Exception e)
        {
            throw new ProductNotFoundException(setPriceModel.ProductId);
        }
    }

    public ProductModel? GetProduct(long productId)
    {
        try
        {
            return _productRepository.GetProduct(productId);
        }
        catch (Exception e)
        {
            throw new ProductNotFoundException(productId);
        }
    }

    public IEnumerable<ProductModel> GetFilteredList(ProductFilter productFilter)
    {
        return _productRepository.GetFilteredList(productFilter);
    }
}