using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Homework2.ProductService.Api.Interceptors;

public class LoggingInterceptor : Interceptor
{
    private ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            _logger.LogInformation($"Start grpc method:{context.Method}. Request = {request}");
            var response = await continuation(request, context);
            _logger.LogInformation($"End grpc method:{context.Method}. Response = {response}");
            
            return response;
        }
        catch (RpcException exception)
        {
            _logger.LogError($"Во время выполнения {context.Method} произошла ошибка {exception.Message}");
            throw;
        }



    }
}