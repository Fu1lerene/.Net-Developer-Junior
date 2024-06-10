using Models;

namespace DataAccess.Repositories.Interfaces;

public interface ISeasonCoefRepository
{
    public List<SeasonCoefModel> Get(int productId);
    public void GenerateDataToTxt(string path);
    public void Read(string path);
}