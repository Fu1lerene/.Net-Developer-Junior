using DataAccess;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using Domain;
using Domain.Services;
using Domain.Services.Demand;
using Domain.Services.SalesPrediction;

namespace Homework_1;

public class ConsoleApplication
{
    private readonly int _numberProducts;
    private readonly int _numberRecords;
    private static IProductRepository? _productRepository;
    private static ISalesHistoryRepository? _salesHistoryRepository;
    private static ISeasonCoefRepository? _seasonCoefRepository;
    private static ICalculateAds? _calculateAds;
    private static ICalculateSalesPrediction? _calculateSalesPrediction;
    private static ICalculateDemand? _calculateDemand;
    
    public ConsoleApplication(int numberProducts, int numberRecords)
    {
        _numberProducts = numberProducts;
        _numberRecords = numberRecords;
        GenerateData();
    }
    public ConsoleApplication()
    {

    }

    private void GenerateData()
    {
        _productRepository = new ProductRepository(_numberProducts);
        _salesHistoryRepository = new SalesHistoryRepository(_productRepository, _numberRecords);
        _seasonCoefRepository = new SeasonCoefRepository(_productRepository);
        _salesHistoryRepository.GenerateDataToTxt("../../../../DataAccess/Data/SalesHistory.txt");
        _seasonCoefRepository.GenerateDataToTxt("../../../../DataAccess/Data/SeasonCoefs.txt");
    }

    public void Run()
    {
        _salesHistoryRepository = new SalesHistoryRepository();
        _seasonCoefRepository = new SeasonCoefRepository();
        _calculateAds = new CalculateAds(_salesHistoryRepository);
        _calculateSalesPrediction = new CalculateSalesPrediction(_salesHistoryRepository, _seasonCoefRepository);
        _calculateDemand = new CalculateDemand(_salesHistoryRepository, _seasonCoefRepository);
        
        _salesHistoryRepository.Read("../../../../DataAccess/Data/SalesHistory.txt");
        _seasonCoefRepository.Read("../../../../DataAccess/Data/SeasonCoefs.txt");
        
        Input();
    }

    private void Input()
    {
        while (true)
        {
            try
            {
                var input = Console.ReadLine().Split(' ', '|');
                if (input.Length > 3)
                {
                    Console.WriteLine("Некорректный ввод");
                    continue;
                }

                int.TryParse(input[1], out int id);
                int numberDays = 0;

                if (input[0] != "ads")
                {
                    int.TryParse(input[2], out numberDays);
                }

                switch (input[0])
                {
                    case "ads":
                        Console.WriteLine(_calculateAds.Ads(id));
                        break;
                    case "prediction":
                        Console.WriteLine(_calculateSalesPrediction.SalesPrediction(id, numberDays));
                        break;
                    case "demand":
                        Console.WriteLine(_calculateDemand.Demand(id, numberDays));
                        break;
                    default:
                        Console.WriteLine("Некорректный ввод");
                        break;
                }
            }
            catch (ProductNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Некорректный ввод");
            }
        }
    }
}