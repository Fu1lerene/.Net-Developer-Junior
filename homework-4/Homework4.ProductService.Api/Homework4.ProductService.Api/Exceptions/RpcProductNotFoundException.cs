using Grpc.Core;

namespace Homework4.ProductService.Api.Exceptions;

public class RpcProductNotFoundException : RpcException
{
    public RpcProductNotFoundException(long productId)
        : base(new Status(StatusCode.NotFound, $"{productId} ID not found"))
    {
    }
}