using Homework_3.Configurations;
using Homework_3.DataAccess.Repositories.Interfaces;
using Homework_3.Domain.CustomTaskSchedulers;
using Microsoft.Extensions.Options;
using Models;

namespace Homework_3.Domain.Services.ParallelDemand;

public class ParallelCalculateDemand : IParallelCalculateDemand
{
    private readonly IProcessingDataRepository _processingDataRepository;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private CancellationTokenSource _cancellationTokenSource;
    private int _counterCalculatedRecords;
    private CancellationToken _cancellationToken;
    private ParallelOptions _parallelOptions;
    private IDisposable _disposable;
    private CustomTaskScheduler _taskScheduler;
    
    private readonly HashSet<Task> _runningTasks;
    private volatile int _parallelismFactor;

    public ParallelCalculateDemand(
        IProcessingDataRepository processingDataRepository, 
        IOptionsMonitor<AppSettings> appSettings)
    {
        _processingDataRepository = processingDataRepository;
        _appSettings = appSettings;

        _parallelismFactor = appSettings.CurrentValue.MaxDegreeOfParallelism;
        _runningTasks = new HashSet<Task>(_parallelismFactor);
    }

    public async Task CalculateAndWriteToTxtAsync()
    {
        SetCancellationToken();
         _taskScheduler = new(_appSettings, _cancellationToken);
        SetParallelOptions();
        
        Console.WriteLine($"Нажмите любую кнопку, чтобы начать рассчет.{Environment.NewLine}" +
                          $"Для отмены нажмите Ctrl + c");
        Console.ReadKey();
        
        await using var writer = new StreamWriter(_appSettings.CurrentValue.OutputPath);
        await writer.WriteLineAsync("ProductId | Demand");
        var reader = new StreamReader(_appSettings.CurrentValue.InputPath);

        var processingData = _processingDataRepository.GetDataAsync(reader, _parallelOptions.CancellationToken);
        
        Console.WriteLine("Рассчет начался");
        
        try
        {
            await Parallel.ForEachAsync(processingData, _parallelOptions, async (data, ct) =>
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
            _taskScheduler.Dispose();
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
        _parallelOptions = new ParallelOptions
        {
            CancellationToken = _cancellationToken,
            MaxDegreeOfParallelism = _appSettings.CurrentValue.MaxDegreeOfParallelism,
            TaskScheduler = _taskScheduler
        };
        
        _appSettings.OnChange(OnConfigChanged);
    }
    
    private void OnConfigChanged(AppSettings appSettings, string? _)
    {
        _parallelOptions = new ParallelOptions
        {
            CancellationToken = _cancellationToken,
            MaxDegreeOfParallelism = _appSettings.CurrentValue.MaxDegreeOfParallelism,
            TaskScheduler = _taskScheduler
        };

        Interlocked.Exchange(ref _parallelismFactor, appSettings.MaxDegreeOfParallelism);
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