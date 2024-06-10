using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Infrastructure;
using HomeworkApp.Dal.Infrastructure.Interfaces;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;

namespace HomeworkApp.Dal.Repositories;

public class TaskCommentRepository : PgRepository, ITaskCommentRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;
    
    public TaskCommentRepository(
        IOptions<DalOptions> dalSettings, IDateTimeProvider dateTimeProvider) : base(dalSettings.Value)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<long> Add(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
insert into task_comments (task_id, author_user_id, message, at, modified_at, deleted_at)  
values (@TaskId, @AuthorUserId, @Message, @At, @ModifiedAt, @DeletedAt)
returning id;
";

        await using var connection = await GetConnection();
        var id = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    model.TaskId,
                    model.AuthorUserId,
                    model.Message,
                    model.At,
                    model.ModifiedAt,
                    model.DeletedAt
                },
                cancellationToken: token));
        
        return id
            .ToArray()
            .First();
    }
    
    public async Task<long[]> AddMany(TaskCommentEntityV1[] model, CancellationToken token)
    {
        const string sqlQuery = @"
insert into task_comments (task_id, author_user_id, message, at, modified_at, deleted_at)  
select task_id,author_user_id, message, at, modified_at, deleted_at
  from UNNEST(@Model)
returning id;
";

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Model = model
                },
                cancellationToken: token));

        return ids
            .ToArray();
    }

    public async Task Update(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments set message = @Message
                       , modified_at = @ModifiedAt;
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    model.Message,
                    ModifiedAt = _dateTimeProvider.DateUtcNow()
                },
                cancellationToken: token,
                commandTimeout: DefaultTimeoutInSeconds));
    }

    public async Task SetDeleted(long taskCommentId, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments set deleted_at = @DeletedAt
 where id = @TaskCommentId;
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskCommentId = taskCommentId,
                    DeletedAt = _dateTimeProvider.DateUtcNow()
                },
                cancellationToken: token,
                commandTimeout: DefaultTimeoutInSeconds));
    }

    public async Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token)
    {
        var baseSql = @"
select id
     , task_id
     , author_user_id
     , message
     , at
     , modified_at
     , deleted_at
  from task_comments
";
        
        var conditions = new List<string>();
        var @params = new DynamicParameters();
        
        conditions.Add($"task_id = @TaskId");
        @params.Add($"TaskId", model.TaskId);
        
        if (!model.IncludeDeleted)
        {
            conditions.Add($"deleted_at is null");
        }
        
        var cmd = new CommandDefinition(
            baseSql + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskCommentEntityV1>(cmd))
            .OrderByDescending(tc => tc.At)
            .ToArray();
    }
}