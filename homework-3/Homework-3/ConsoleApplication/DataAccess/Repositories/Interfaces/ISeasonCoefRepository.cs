using Models;

namespace Homework_3.DataAccess.Repositories.Interfaces;

public interface ISeasonCoefRepository
{
    public List<SeasonCoefModel> Get(long productId);
}