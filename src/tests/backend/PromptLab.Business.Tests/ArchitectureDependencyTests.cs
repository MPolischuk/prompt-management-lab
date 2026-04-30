using System.Xml.Linq;
using FluentAssertions;

namespace PromptLab.Business.Tests;

public class ArchitectureDependencyTests
{
    [Fact]
    public void PromptLabBusiness_ShouldNotReferencePromptLabDataProject()
    {
        var projectPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..", "..",
                "PromptLab.Service", "PromptLab.Business", "PromptLab.Business.csproj"));

        File.Exists(projectPath).Should().BeTrue($"Expected project file at {projectPath}");
        var doc = XDocument.Load(projectPath);
        var projectReferences = doc
            .Descendants("ProjectReference")
            .Select(x => x.Attribute("Include")?.Value ?? string.Empty)
            .ToArray();

        projectReferences.Should().OnlyContain(reference =>
            !reference.Contains("PromptLab.Data", StringComparison.OrdinalIgnoreCase));
    }
}
