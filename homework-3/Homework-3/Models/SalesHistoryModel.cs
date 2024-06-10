namespace Models;

public class SalesHistoryModel
{
    public long ProductId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int Sales { get; init; }
    public int Stock { get; init; }
}