namespace Homework_3.Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public override string Message { get; }

    public ProductNotFoundException(long productId)
    {
        Message = $"{productId} ID not found";
    }
}