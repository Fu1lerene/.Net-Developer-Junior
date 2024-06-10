using DataAccess.Repositories.Interfaces;
using Models;

namespace DataAccess.Repositories;

public class SeasonCoefRepository : ISeasonCoefRepository
{
    private static readonly List<SeasonCoefModel> SeasonCoefs = new();

    public SeasonCoefRepository(IProductRepository products)
    {
        GenerateCoef(products);
    }

    public SeasonCoefRepository()
    {
        
    }
    
    private void GenerateCoef(IProductRepository products)
    {
        for (int i = 1; i <= products.NumberProducts; i++)
        {
            for (int month = 1; month <= 12; month++)
            {
                SeasonCoefs.Add(new SeasonCoefModel
                {
                    ProductId = products.Get(i).Id,
                    Month = month,
                    Coef = Random.Shared.Next(0, 30) / 10f
                });
            }
        }
    }

    public List<SeasonCoefModel> Get(int productId)
    {
        return SeasonCoefs.Where(x => x.ProductId == productId).ToList();
    }
    public void GenerateDataToTxt(string path)
    {
        var writer = new StreamWriter(path, false);
        writer.WriteLine("Id | Month | Coef");
        foreach (var coef in SeasonCoefs)
        {
            writer.WriteLine($"{coef.ProductId} | {coef.Month} | {coef.Coef}");
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
                Parsing(line, out var productId, out var month, out float coef);

                SeasonCoefs.Add(new SeasonCoefModel
                {
                    ProductId = productId,
                    Month = month,
                    Coef = coef
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

    private void Parsing(string line, out int productId, out int month, out float coef)
    {
        var lineSplit = line.Split(" | ");
        int.TryParse(lineSplit[0], out productId);
        int.TryParse(lineSplit[1], out month);
        float.TryParse(lineSplit[2], out coef);
    }
    
}