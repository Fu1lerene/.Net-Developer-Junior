using FluentValidation;
using Homework4.ProductService.Api.Application;
using Homework4.ProductService.Api.GrpcServices;
using Homework4.ProductService.Api.Interceptors;
using Homework4.ProductService.Infrastructure;

namespace Homework4.ProductService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc(o =>
        {
            //o.Interceptors.Add<LoggingInterceptor>();
            //o.Interceptors.Add<ValidationInterceptor>();
        }).AddJsonTranscoding();

        builder.Services.AddGrpcSwagger();
        builder.Services.AddSwaggerGen();

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

        builder.Services.AddControllers();
        builder.Services.AddServices();
        builder.Services.AddRepositories();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGrpcService<ProductServiceGrpc>();
        app.MapControllers();

        app.Run();
    }
}