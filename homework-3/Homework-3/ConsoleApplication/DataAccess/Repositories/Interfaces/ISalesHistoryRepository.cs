using Models;

namespace Homework_3.DataAccess.Repositories.Interfaces;

public interface ISalesHistoryRepository
{
    List<SalesHistoryModel> Get(long productId);
    List<SalesHistoryModel> GetAll();
}