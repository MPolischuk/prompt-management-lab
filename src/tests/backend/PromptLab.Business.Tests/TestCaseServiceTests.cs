using FluentAssertions;
using Moq;
using PromptLab.Business.Services;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestCases;

namespace PromptLab.Business.Tests;

public class TestCaseServiceTests
{
    private readonly Mock<ITestCaseRepository> _repository = new();
    private TestCaseService CreateSut() => new(_repository.Object);

    [Fact]
    public async Task GetBySuiteIdAsync_DelegatesToRepository()
    {
        var suiteId = Guid.NewGuid();
        var cases = new List<TestCase>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SuiteId = suiteId,
                Name = "c1",
                InputVariables = "{}",
                ExpectedOutput = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        _repository.Setup(r => r.GetBySuiteIdAsync(suiteId, It.IsAny<CancellationToken>())).ReturnsAsync(cases);
        var sut = CreateSut();

        var result = await sut.GetBySuiteIdAsync(suiteId, CancellationToken.None);

        result.Should().BeEquivalentTo(cases, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task CreateAsync_DelegatesToRepository()
    {
        var req = new CreateTestCaseRequest { SuiteId = Guid.NewGuid(), Name = "n", InputVariables = "{}", ExpectedOutput = "e" };
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
        var req = new UpdateTestCaseRequest { Name = "n", InputVariables = "{}", ExpectedOutput = null };
        _repository.Setup(r => r.UpdateAsync(id, req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = id });
        var sut = CreateSut();

        var result = await sut.UpdateAsync(id, req, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        var sut = CreateSut();

        var result = await sut.DeleteAsync(id, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}
