using Grpc.Core;
using Grpc.Core.Interceptors;
using HomeworkApp.Bll.Services.Interfaces;

namespace HomeworkApp.Interceptors;

public class RateLimiterInterceptor : Interceptor
{
    private readonly IRateLimiterService _rateLimiter;

    public RateLimiterInterceptor(IRateLimiterService rateLimiter)
    {
        _rateLimiter = rateLimiter;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await _rateLimiter.ThrowIfTooManyRequests(
                context.RequestHeaders.GetValue("X-R256-USER-IP"), default);
        }
        catch (InvalidOperationException e)
        {
            var status = new Status(StatusCode.ResourceExhausted, e.Message);
            throw new RpcException(status);
        }
        
        return await continuation(request, context);
    }
}