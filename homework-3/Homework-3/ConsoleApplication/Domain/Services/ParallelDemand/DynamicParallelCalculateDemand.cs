using Homework_3.Configurations;
using Homework_3.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Models;

namespace Homework_3.Domain.Services.ParallelDemand;

public class DynamicParallelCalculateDemand : IDynamicParallelCalculateDemand
{
    private readonly IProcessingDataRepository _processingDataRepository;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private CancellationTokenSource _cancellationTokenSource;
    private int _counterCalculatedRecords;
    private CancellationToken _cancellationToken;
    private DynamicParallelOptions _dynamicParallelOptions;
    
    public DynamicParallelCalculateDemand(IProcessingDataRepository processingDataRepository, IOptionsMonitor<AppSettings> appSettings)
    {
        _processingDataRepository = processingDataRepository;
        _appSettings = appSettings;
    }

    public async Task CalculateAndWriteToTxtAsync()
    {
        SetCancellationToken();
        SetParallelOptions();
        
        Console.WriteLine($"Нажмите любую кнопку, чтобы начать рассчет.{Environment.NewLine}" +
                          $"Для отмены нажмите Ctrl + c");
        Console.ReadKey();
        
        await using var writer = new StreamWriter(_appSettings.CurrentValue.OutputPath);
        await writer.WriteLineAsync("ProductId | Demand");
        var reader = new StreamReader(_appSettings.CurrentValue.InputPath);
        
        var processingData = _processingDataRepository.GetDataAsync(reader, _dynamicParallelOptions.CancellationToken);
        Console.WriteLine("Рассчет начался");
        
        try
        {
            await MyParallel.DynamicParallelForEachAsync(processingData, _dynamicParallelOptions, async (data, ct) =>
            {
                var outputData = await CalculateDemandAsync(data, ct);
                await _processingDataRepository.WriteDataToTxtAsync(writer, outputData, ct);
            });
            Console.WriteLine("Рассчеты закончены");
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _cancellationTokenSource.Dispose();
        }
    }
    
    private async Task<OutputDataModel> CalculateDemandAsync(ProcessingDataModel processingData, CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        var demand = processingData.Prediction - processingData.Stock;
        
        Console.WriteLine($"{++_counterCalculatedRecords} товаров посчитано");
        
        return new OutputDataModel
        {
            ProductId = processingData.ProductId,
            Demand = demand
        };
    }

    private void SetParallelOptions()
    {
        _dynamicParallelOptions = new DynamicParallelOptions(1000)
        {
            DegreeOfParallelism = _appSettings.CurrentValue.MaxDegreeOfParallelism,
            CancellationToken = _cancellationToken
        };

        _appSettings.OnChange((a, b) =>
        {
            _dynamicParallelOptions.DegreeOfParallelism = a.MaxDegreeOfParallelism;
        });
    }
    
    private void SetCancellationToken()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            _cancellationTokenSource.Cancel();
            eventArgs.Cancel = true;
        };
    }
}