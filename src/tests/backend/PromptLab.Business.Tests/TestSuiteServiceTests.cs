using FluentAssertions;
using Moq;
using PromptLab.Business.Services;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestCases;
using PromptLab.Entities.TestSuites;

namespace PromptLab.Business.Tests;

public class TestSuiteServiceTests
{
    private readonly Mock<ITestSuiteRepository> _suites = new();
    private readonly Mock<ITestCaseRepository> _cases = new();

    private TestSuiteService CreateSut() => new(_suites.Object, _cases.Object);

    [Fact]
    public async Task GetByPromptIdAsync_DelegatesToRepository()
    {
        var promptId = Guid.NewGuid();
        var list = new List<TestSuite>();
        _suites.Setup(s => s.GetByPromptIdAsync(promptId, It.IsAny<CancellationToken>())).ReturnsAsync(list);
        var sut = CreateSut();

        var result = await sut.GetByPromptIdAsync(promptId, CancellationToken.None);

        result.Should().BeSameAs(list);
    }

    [Fact]
    public async Task GetByIdWithCasesAsync_WhenSuiteMissing_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _suites.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((TestSuite?)null);
        var sut = CreateSut();

        var detail = await sut.GetByIdWithCasesAsync(id, CancellationToken.None);

        detail.Should().BeNull();
        _cases.Verify(c => c.GetBySuiteIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdWithCasesAsync_WhenSuiteExists_LoadsCases()
    {
        var id = Guid.NewGuid();
        var suite = new TestSuite
        {
            Id = id,
            PromptId = Guid.NewGuid(),
            Name = "S",
            Description = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var caseList = new List<TestCase>();
        _suites.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(suite);
        _cases.Setup(c => c.GetBySuiteIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(caseList);
        var sut = CreateSut();

        var detail = await sut.GetByIdWithCasesAsync(id, CancellationToken.None);

        detail.Should().NotBeNull();
        detail!.Suite.Should().Be(suite);
        detail.Cases.Should().BeSameAs(caseList);
    }

    [Fact]
    public async Task CreateAsync_DelegatesToRepository()
    {
        var req = new CreateTestSuiteRequest { PromptId = Guid.NewGuid(), Name = "n", Description = "d" };
        _suites.Setup(s => s.CreateAsync(req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        var sut = CreateSut();

        var result = await sut.CreateAsync(req, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var req = new UpdateTestSuiteRequest { Name = "n", Description = null };
        _suites.Setup(s => s.UpdateAsync(id, req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = id });
        var sut = CreateSut();

        var result = await sut.UpdateAsync(id, req, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        _suites.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        var sut = CreateSut();

        var result = await sut.DeleteAsync(id, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}
