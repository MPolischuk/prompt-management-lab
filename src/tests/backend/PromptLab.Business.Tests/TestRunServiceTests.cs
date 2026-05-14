using FluentAssertions;
using Moq;
using PromptLab.Business.Services;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;

namespace PromptLab.Business.Tests;

public class TestRunServiceTests
{
    private readonly Mock<ITestRunRepository> _repository = new();
    private TestRunService CreateSut() => new(_repository.Object);

    [Fact]
    public async Task GetAllAsync_DelegatesToRepository()
    {
        var list = new List<TestRun>();
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);
        var sut = CreateSut();

        var result = await sut.GetAllAsync(CancellationToken.None);

        result.Should().BeSameAs(list);
    }

    [Fact]
    public async Task GetBySuiteIdAsync_DelegatesToRepository()
    {
        var suiteId = Guid.NewGuid();
        var list = new List<TestRun>();
        _repository.Setup(r => r.GetBySuiteIdAsync(suiteId, It.IsAny<CancellationToken>())).ReturnsAsync(list);
        var sut = CreateSut();

        var result = await sut.GetBySuiteIdAsync(suiteId, CancellationToken.None);

        result.Should().BeSameAs(list);
    }

    [Fact]
    public async Task GetByIdWithResultsAsync_WhenRunMissing_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((TestRun?)null);
        var sut = CreateSut();

        var detail = await sut.GetByIdWithResultsAsync(id, CancellationToken.None);

        detail.Should().BeNull();
        _repository.Verify(r => r.GetResultsByRunIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdWithResultsAsync_WhenRunExists_LoadsResults()
    {
        var id = Guid.NewGuid();
        var run = new TestRun
        {
            Id = id,
            SuiteId = Guid.NewGuid(),
            PromptId = Guid.NewGuid(),
            PromptVersion = 1,
            Model = "m",
            Temperature = 0.1m,
            Status = "Done",
            StartedAt = null,
            CompletedAt = null,
            CreatedAt = DateTime.UtcNow,
            PromptTitle = null,
            SuiteName = null
        };
        var results = new List<TestResult>();
        _repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(run);
        _repository.Setup(r => r.GetResultsByRunIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(results);
        var sut = CreateSut();

        var detail = await sut.GetByIdWithResultsAsync(id, CancellationToken.None);

        detail.Should().NotBeNull();
        detail!.Run.Should().Be(run);
        detail.Results.Should().BeSameAs(results);
    }

    [Fact]
    public async Task CreateAsync_DelegatesToRepository()
    {
        var req = new CreateTestRunRequest
        {
            SuiteId = Guid.NewGuid(),
            PromptId = Guid.NewGuid(),
            PromptVersion = 1,
            Model = "m",
            Temperature = 0.2m,
            Status = "Pending"
        };
        _repository.Setup(r => r.CreateAsync(req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        var sut = CreateSut();

        var result = await sut.CreateAsync(req, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var req = new UpdateTestRunRequest { Status = "Done", StartedAt = DateTime.UtcNow, CompletedAt = DateTime.UtcNow };
        _repository.Setup(r => r.UpdateAsync(id, req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = id });
        var sut = CreateSut();

        var result = await sut.UpdateAsync(id, req, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateResultAsync_DelegatesToRepository()
    {
        var req = new CreateTestResultRequest
        {
            RunId = Guid.NewGuid(),
            CaseId = Guid.NewGuid(),
            ActualOutput = "a",
            Passed = true,
            Score = 1m,
            LatencyMs = 1,
            Error = null
        };
        _repository.Setup(r => r.CreateResultAsync(req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        var sut = CreateSut();

        var result = await sut.CreateResultAsync(req, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}
