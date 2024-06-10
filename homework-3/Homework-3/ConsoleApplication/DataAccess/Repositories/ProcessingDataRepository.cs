using System.Runtime.CompilerServices;
using Homework_3.DataAccess.Repositories.Interfaces;
using Homework_3.Domain.Services.SalesPrediction;
using Models;

namespace Homework_3.DataAccess.Repositories;

public class ProcessingDataRepository : IProcessingDataRepository
{
    private readonly Dictionary<long, ProcessingDataModel> _generatedProcessingDataRepository;
    private readonly ICalculateSalesPrediction _calculateSalesPrediction;
    private readonly ISalesHistoryRepository _salesHistoryRepository;
    private readonly int _numberDaysForPrediction = 30;
    private readonly Semaphore _writeSemaphore;
    private int _counterWrittenLines;
    private int _counterReadLines;
        
    public ProcessingDataRepository(
        ISalesHistoryRepository salesHistoryRepository,
        ISeasonCoefRepository seasonCoefRepository)
    {
        _salesHistoryRepository = salesHistoryRepository;
        _generatedProcessingDataRepository = new();
        _calculateSalesPrediction = new CalculateSalesPrediction(salesHistoryRepository, seasonCoefRepository);
        
        _writeSemaphore = new (1, 1);
    }

    private void GenerateData()
    {
        var unicId = _salesHistoryRepository
            .GetAll()
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();
        
        for (int i = 0; i < unicId.Count; i++)
        {
            var productId = unicId[i];
            var salesHistoryList = _salesHistoryRepository.Get(productId);
            var salesPrediction = _calculateSalesPrediction.SalesPrediction(productId, _numberDaysForPrediction);
            var stock = salesHistoryList.OrderBy(x => x.Date).Last().Stock;
            
            _generatedProcessingDataRepository.Add(productId, new ProcessingDataModel
            {
                ProductId = productId,
                Prediction = salesPrediction,
                Stock = stock
            });
        }
    }
    
    public void GenerateDataToTxt(string path)
    {
        GenerateData();
        
        var writer = new StreamWriter(path, false);
        writer.WriteLine("Id | Prediction | Stock");
        foreach (var processingData in _generatedProcessingDataRepository)
        {
            writer.WriteLine($"{processingData.Key} | {processingData.Value.Prediction} | {processingData.Value.Stock}");
        }
        writer.Close();
    }

    public async IAsyncEnumerable<ProcessingDataModel> GetDataAsync(
        StreamReader reader,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await reader.ReadLineAsync(cancellationToken);
        var line = await reader.ReadLineAsync(cancellationToken);
        
        while (line is not null)
        {
            var data = line.Split(" | ");
            Int64.TryParse(data[0], out long productId);
            Int32.TryParse(data[1], out int prediction);
            Int32.TryParse(data[2], out int stock);
            

            yield return new ProcessingDataModel
            {
                Prediction = prediction,
                ProductId = productId,
                Stock = stock
            };

            Console.WriteLine($"Прочитано {++_counterReadLines} строк");
            line = await reader.ReadLineAsync(cancellationToken);
        }
    }

    public async Task WriteDataToTxtAsync(StreamWriter writer,
        OutputDataModel outputDataModel,
        CancellationToken cancellationToken)
    {
        _writeSemaphore.WaitOne();
        await writer.WriteLineAsync($"{outputDataModel.ProductId} | {outputDataModel.Demand}");
        Console.WriteLine($"Записано {++_counterWrittenLines} результатов");
        await writer.FlushAsync(cancellationToken);
        _writeSemaphore.Release();
    }
    
}