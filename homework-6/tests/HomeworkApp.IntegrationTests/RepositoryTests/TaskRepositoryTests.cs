using FluentAssertions;
using HomeworkApp.Dal.Extensions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);
        
        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }
    
    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);
        
        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }
    
    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();
        
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);
        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);
        
        // Act
        await _repository.Assign(assign, default);
        
        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        results.Should().HaveCount(1);
        var task = results.Single();
        
        expectedTask = expectedTask with {Status = assign.Status};
        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task Get_SubTasksInStatus_Success()
    {
        // Arrange
        var count = 4;

        var subTasks = TaskEntityV1Faker.Generate(count);
        subTasks[0] = subTasks[0].WithStatus(TaskStatus.ToDo);
        subTasks[1] = subTasks[1].WithStatus(TaskStatus.InProgress);
        subTasks[2] = subTasks[2].WithStatus(TaskStatus.Draft);
        subTasks[3] = subTasks[3].WithStatus(TaskStatus.Done);

        subTasks[0] = subTasks[0].WithId(1);
        subTasks[0] = subTasks[0].WithParentTaskId(null);
        
        for (int i = 1; i < count; i++)
        {
            subTasks[i] = subTasks[i].WithId(i + 1);
            subTasks[i] = subTasks[i].WithParentTaskId(subTasks[i - 1].Id);
        }
        var taskIds = await _repository.Add(subTasks, default);

        var parentTaskId = taskIds[0];
        
        var expectedSubTasks = new[]
        {
            new SubTaskModel
            {
                TaskId = taskIds[1],
                Title = subTasks[1].Title,
                Status = subTasks[1].Status.ToTaskStatus(),
                ParentTaskIds = new[] { taskIds[0] }
            },
            new SubTaskModel
            {
                TaskId = subTasks[3].Id,
                Title = subTasks[3].Title,
                Status = subTasks[3].Status.ToTaskStatus(),
                ParentTaskIds = new[] { taskIds[2], taskIds[1], taskIds[0]}
            }
        };

        // Act
        var results = await _repository.GetSubTasksInStatus(parentTaskId, 
            new[]
            {
                TaskStatus.InProgress,
                TaskStatus.Done
            },
            default);
        
        // Asserts
        results.Should().HaveCount(2);
        results.Should().BeEquivalentTo(expectedSubTasks);
    }
}
