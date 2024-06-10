namespace HomeworkApp.Dal.Extensions;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

public static class TaskStatusesExtensions
{
    public static int[] ToIntArray(this TaskStatus[] taskStatuses)
    {
        var arrayTaskStatuses = new int[taskStatuses.Length];
        for (var i = 0; i < taskStatuses.Length; i++)
        {
            arrayTaskStatuses[i] = (int)taskStatuses[i];
            Console.WriteLine(arrayTaskStatuses[i]);
        }

        return arrayTaskStatuses;
    }
    
    public static TaskStatus ToTaskStatus(this int integer)
    {
        return integer switch
        {
            1 => TaskStatus.Draft,
            2 => TaskStatus.ToDo,
            3 => TaskStatus.InProgress,
            4 => TaskStatus.Done,
            5 => TaskStatus.Canceled,
            _ => default
        };
    }
}