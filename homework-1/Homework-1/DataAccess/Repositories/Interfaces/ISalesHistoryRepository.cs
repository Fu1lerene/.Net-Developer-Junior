using Models;

namespace DataAccess.Repositories.Interfaces;

public interface ISalesHistoryRepository
{
    public List<SalesHistoryModel> Get(int productId);
    public void GenerateDataToTxt(string path);
    public void Read(string path);
}