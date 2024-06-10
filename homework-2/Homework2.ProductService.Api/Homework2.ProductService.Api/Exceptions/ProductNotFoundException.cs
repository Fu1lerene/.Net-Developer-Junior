namespace Homework2.ProductService.Api.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException()
    {
        
    }

    public ProductNotFoundException(string message):
        base(message)
    {
        
    }
}