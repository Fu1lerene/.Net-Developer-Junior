using DataAccess;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using Domain.Services;
using Domain.Services.Demand;
using Domain.Services.SalesPrediction;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

ISalesHistoryRepository salesHistoryRepository = new SalesHistoryRepository();
ISeasonCoefRepository seasonCoefRepository = new SeasonCoefRepository();

ICalculateAds calculateAds = new CalculateAds(salesHistoryRepository);
ICalculateSalesPrediction calculateSalesPrediction = new CalculateSalesPrediction(salesHistoryRepository, seasonCoefRepository);
ICalculateDemand calculateDemand = new CalculateDemand(salesHistoryRepository, seasonCoefRepository);

string salesHistoryPath = builder.Configuration.GetValue<string>("RepositoryData:SalesHistoryData")!;
string seasonCoefPath = builder.Configuration.GetValue<string>("RepositoryData:MonthCoefficientData")!;

salesHistoryRepository.Read(salesHistoryPath);
seasonCoefRepository.Read(seasonCoefPath);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/homework-1/Ads{productId}", (int id) =>
{
    return calculateAds.Ads(id);
});

app.MapGet("/homework-1/SalesPrediction/{productId},{numberDays}", (int id, int numberDays) =>
{
    return calculateSalesPrediction.SalesPrediction(id, numberDays);
});

app.MapGet("/homework-1/Demand/{productId},{numberDays}", (int id, int numberDays) =>
{
    return calculateDemand.Demand(id, numberDays);
});


app.Run();