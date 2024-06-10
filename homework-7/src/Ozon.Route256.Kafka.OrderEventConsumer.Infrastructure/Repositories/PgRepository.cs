using System.Threading.Tasks;
using System.Transactions;
using Npgsql;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public class PgRepository
{
    protected const int DefaultTimeoutInSeconds = 5;

    protected PgRepository()
    {
    }
    
    protected async Task<NpgsqlConnection> GetConnection(string connectionString)
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException("Transaction was aborted (probably by user cancellation request)");
        }
        
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        
        // Due to in-process migrations
        connection.ReloadTypes();
        
        return connection;
    }
}