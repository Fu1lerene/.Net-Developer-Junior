using FluentValidation;
using Homework2.ProductService.Api.Application;
using Homework2.ProductService.Api.GrpcServices;
using Homework2.ProductService.Api.Interceptors;
using Homework2.ProductService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<LoggingInterceptor>();
    o.Interceptors.Add<ValidationInterceptor>();
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