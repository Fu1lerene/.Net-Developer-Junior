using HomeworkApp.Dal.Infrastructure.Interfaces;

namespace HomeworkApp.Dal.Infrastructure;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset DateUtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}