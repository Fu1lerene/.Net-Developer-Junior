using DataAccess.Repositories.Interfaces;
using Models;

namespace DataAccess.Repositories;

public class SalesHistoryRepository : ISalesHistoryRepository
{
    private static List<SalesHistoryModel> _salesHistory = new();
    
    public SalesHistoryRepository(IProductRepository products, int numberRecords)
    { 
        GenerateHistory(products, numberRecords);
    }

    public SalesHistoryRepository()
    {
        
    }

    private void GenerateHistory(IProductRepository products, int numberRecords)
    {
        for (int i = 1; i <= numberRecords; i++)
        {
            _salesHistory.Add(new SalesHistoryModel
            {
                ProductId = products.Get(Random.Shared.Next(1, products.NumberProducts)).Id,
                Date = DateTimeOffset.Now.AddDays(Random.Shared.Next(1, 365) * -1),
                Sales = Random.Shared.Next(0, 20),
                Stock = Random.Shared.Next(20, 40)
            });
        }

        _salesHistory = _salesHistory
            .OrderBy(x => x.ProductId)
            .ThenBy(x => x.Date)
            .ToList();
    }

    public List<SalesHistoryModel> Get(int productId)
    {
        return _salesHistory.Where(x => x.ProductId == productId).ToList();
    }

    public void GenerateDataToTxt(string path)
    {
        var writer = new StreamWriter(path, false);
        writer.WriteLine("Id | Date | Sales | Stock");
        foreach (var history in _salesHistory)
        {
            writer.WriteLine($"{history.ProductId} | {history.Date.ToString("dd/MM/yyyy")} | {history.Sales} | {history.Stock}");
        }
        writer.Close();
    }

    public void Read(string path)
    {
        try
        {
            var reader = new StreamReader(path);
            var line = reader.ReadLine();
            line = reader.ReadLine();
            while (line != null)
            {
                Parsing(line, out var productId, out var date, out var sales, out var stock);

                _salesHistory.Add(new SalesHistoryModel
                {
                    ProductId = productId,
                    Date = date,
                    Sales = sales,
                    Stock = stock
                });
                line = reader.ReadLine();
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void Parsing(string line, out int productId, out DateTimeOffset date, out int sales, out int stock)
    {
        var lineSplit = line.Split(" | ");
        int.TryParse(lineSplit[0], out productId);
        DateTimeOffset.TryParse(lineSplit[1], out date);
        int.TryParse(lineSplit[2], out sales);
        int.TryParse(lineSplit[3], out stock);
    }
}