namespace HomeworkApp.Dal.Infrastructure.Interfaces;

public interface IDateTimeProvider
{
    DateTimeOffset DateUtcNow();
}