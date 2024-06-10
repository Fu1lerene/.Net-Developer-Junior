namespace Homework_3.Configurations;

public class AppSettings
{
    public const string Position = "AppSettings";
    public int MaxDegreeOfParallelism { get; init; }
    public string? InputPath { get; init; }
    public string? OutputPath { get; init; }
}