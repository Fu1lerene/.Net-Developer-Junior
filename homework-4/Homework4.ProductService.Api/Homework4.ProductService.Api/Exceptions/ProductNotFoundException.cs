using System;

namespace Homework4.ProductService.Api.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(long productId)
        :base(message: $"Товар с {productId} ID не найден")
    {
        
    }
}