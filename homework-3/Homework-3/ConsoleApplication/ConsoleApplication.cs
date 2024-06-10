using Homework_3.Configurations;
using Homework_3.DataAccess.Repositories;
using Homework_3.DataAccess.Repositories.Interfaces;
using Homework_3.Domain.Services.ParallelDemand;
using Microsoft.Extensions.Options;

namespace Homework_3;

public class ConsoleApplication
{
    private readonly IProductRepository _productRepository;
    private readonly ISalesHistoryRepository _salesHistoryRepository;
    private readonly ISeasonCoefRepository _seasonCoefRepository;
    private readonly IProcessingDataRepository _processingDataRepository;
    private readonly IParallelCalculateDemand _parallelCalculateDemand;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly IDynamicParallelCalculateDemand _dynamicCalculateDemand;
    
    private readonly int _numberProducts = 500;
    private readonly int _numberHistoryRecords = 1000;

    private IDisposable ChangerListener;
    public ConsoleApplication(IOptionsMonitor<AppSettings> appSettings)
    {
        _appSettings = appSettings;
        _productRepository = new ProductRepository(_numberProducts);
        _salesHistoryRepository = new SalesHistoryRepository(_productRepository, _numberHistoryRecords);
        _seasonCoefRepository = new SeasonCoefRepository(_productRepository);
        _processingDataRepository = new ProcessingDataRepository(_salesHistoryRepository, _seasonCoefRepository);
        _parallelCalculateDemand = new ParallelCalculateDemand(_processingDataRepository, _appSettings);
        _dynamicCalculateDemand = new DynamicParallelCalculateDemand(_processingDataRepository, _appSettings);
    }

    public async Task RunAsync()
    {
        await _dynamicCalculateDemand.CalculateAndWriteToTxtAsync();
        //await _parallelCalculateDemand.CalculateAndWriteToTxtAsync();
    }

    public void GenerateProcessingData()
    {
        _processingDataRepository.GenerateDataToTxt(_appSettings.CurrentValue.InputPath);
    }

}