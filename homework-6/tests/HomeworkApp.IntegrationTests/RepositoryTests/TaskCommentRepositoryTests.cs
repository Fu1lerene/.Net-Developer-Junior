using FluentAssertions;
using HomeworkApp.Dal.Infrastructure.Interfaces;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskCommentRepositoryTests
{
    private readonly ITaskCommentRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TaskCommentRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskCommentRepository;
        _dateTimeProvider = fixture.DateTimeProvider;
    }
    
    [Fact]
    public async Task Add_Comment_Success()
    {
        // Arrange
        var comment = TaskCommentEntityV1Faker.Generate().First();
        
        // Act
        var result = await _repository.Add(comment, default);

        // Asserts
        result.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task Add_Comments_Success()
    {
        // Arrange
        var count = 10;
        var comment = TaskCommentEntityV1Faker.Generate(10);
        
        // Act
        var results = await _repository.AddMany(comment, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }
    
    [Fact]
    public async Task UpdateMessage_AtTime_Success()
    {
        // Arrange
        var taskId = 2;
        var expectedMessage = "test";
        var expectedTime = _dateTimeProvider.DateUtcNow();
        
        var comment = TaskCommentEntityV1Faker
            .Generate();

        var expectedComment = comment.First()
            .WithMessage(expectedMessage)
            .WithModifiedAt(expectedTime)
            .WithTaskId(taskId);
        
        await _repository.Add(expectedComment, default);
        
        // Act
        await _repository.Update(expectedComment, default);

        // Asserts
        var result = await _repository.Get(new TaskCommentGetModel
        {
            IncludeDeleted = true,
            TaskId = taskId
        }, default);

        var actualComment = result.Single();
        actualComment.ModifiedAt.Should()
            .BeCloseTo(_dateTimeProvider.DateUtcNow(), TimeSpan.FromMilliseconds(100));
        
        actualComment.Should().BeEquivalentTo(
            expectedComment, opt => opt
                .Excluding(x => x.Id)
                .Excluding(x => x.ModifiedAt));
    }
    
    [Fact]
    public async Task GetComments_WithDeletedComments_Success()
    {
        // Arrange
        var count = 5;
        var comments = TaskCommentEntityV1Faker.Generate(count);
        var expectedTaskId = 1L;

        var expectedComments = comments.Select(x => x.WithTaskId(expectedTaskId)).ToArray();
        
        await _repository.AddMany(expectedComments, default);
        
        // Act
        var results = await _repository.Get(new TaskCommentGetModel
        {
            TaskId = expectedTaskId,
            IncludeDeleted = true
        }, default);

        // Asserts
        results.Should().HaveCount(count);
        
        results.Should().AllSatisfy(x => x.DeletedAt.Should().NotBeNull());
        results.Should().BeEquivalentTo(
            expectedComments, opt => opt
                .Excluding(x => x.Id));
    }
    
    [Fact]
    public async Task GetComments_WithoutDeletedComments_Success()
    {
        // Arrange
        var count = 5;
        var comments = TaskCommentEntityV1Faker.Generate(count);
        var expectedTaskId = 1L;
        
        var expectedComments = comments.Select(x => x.WithTaskId(expectedTaskId)).ToArray();
        expectedComments[0] = expectedComments[0].WithDeletedAt(null);
        
        await _repository.AddMany(expectedComments, default);
        
        // Act
        var results = await _repository.Get(new TaskCommentGetModel
        {
            TaskId = expectedTaskId,
            IncludeDeleted = false
        }, default);

        // Asserts
        results.Should().HaveCount(1);
        
        results.Should().AllSatisfy(x => x.DeletedAt.Should().BeNull());
    }
    
    [Fact]
    public async Task Set_DeletedMessage_Success()
    {
        // Arrange
        var expectedTime = _dateTimeProvider.DateUtcNow();
        var comment = TaskCommentEntityV1Faker
            .Generate()
            .First()
            .WithDeletedAt(expectedTime);
        
        var commentId = await _repository.Add(comment, default);
        
        // Act
        await _repository.SetDeleted(commentId, default);

        // Asserts
        var results = await _repository.Get(new TaskCommentGetModel
        {
            IncludeDeleted = true,
            TaskId = comment.TaskId
        }, default);

        results.First().DeletedAt.Should()
            .BeCloseTo(_dateTimeProvider.DateUtcNow(), TimeSpan.FromMilliseconds(100));
    }
}