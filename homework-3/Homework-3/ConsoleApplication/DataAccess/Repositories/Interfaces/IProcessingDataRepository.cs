using Models;

namespace Homework_3.DataAccess.Repositories.Interfaces;

public interface IProcessingDataRepository
{
    IAsyncEnumerable<ProcessingDataModel> GetDataAsync(StreamReader reader, CancellationToken cancellationToken);
    Task WriteDataToTxtAsync(StreamWriter writer, OutputDataModel outputDataModel, CancellationToken cancellationToken);
    void GenerateDataToTxt(string path);
}