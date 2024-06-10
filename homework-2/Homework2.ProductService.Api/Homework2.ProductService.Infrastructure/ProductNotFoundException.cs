namespace Homework2.ProductService.Infrastructure;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException()
    {
        
    }

    public ProductNotFoundException(string message) 
        : base(message)
    {
        
    }
}