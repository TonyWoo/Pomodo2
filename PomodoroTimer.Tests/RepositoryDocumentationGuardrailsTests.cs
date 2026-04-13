using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class RepositoryDocumentationGuardrailsTests
{
    private static readonly string RepoRoot = FindRepoRoot();

    [Fact]
    public void AgentsGuideRemainsCompactAndAnchoredToRealSources()
    {
        var agentsPath = GetRepoPath("AGENTS.md");
        Assert.True(File.Exists(agentsPath), "Expected AGENTS.md to exist at the repo root.");

        var lines = File.ReadAllLines(agentsPath);
        Assert.True(lines.Length <= 100, $"AGENTS.md should stay close to a map, not an encyclopedia. Current line count: {lines.Length}.");

        var agentsText = File.ReadAllText(agentsPath);

        foreach (var requiredSnippet in new[]
        {
            "[README.md](README.md)",
            "[README.zh-CN.md](README.zh-CN.md)",
            "[WORKFLOW.md](WORKFLOW.md)",
            "[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)",
            "[docs/exec-plans/README.md](docs/exec-plans/README.md)",
            "PomodoroTimer/App.axaml",
            "PomodoroTimer/Views/MainView.axaml",
            "PomodoroTimer/Views/MainWindow.axaml",
            "PomodoroTimer.Desktop/Program.cs",
            "PomodoroTimer.Android/MainActivity.cs",
            "PomodoroTimer.iOS/AppDelegate.cs",
            "PomodoroTimer/ViewModels/MainWindowViewModel.cs",
            "PomodoroTimer/Models/PomodoroTimerState.cs",
            "PomodoroTimer/Localization/",
            "PomodoroTimer.CrossPlatform.slnx",
            "PomodoroTimer.Tests/RepositoryDocumentationGuardrailsTests.cs",
            ".github/workflows/dotnet-desktop.yml",
            "dotnet build PomodoroTimer.sln",
            "dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj"
        })
        {
            Assert.Contains(requiredSnippet, agentsText, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void RequiredHarnessDocumentationFilesExist()
    {
        foreach (var relativePath in new[]
        {
            "README.md",
            "README.zh-CN.md",
            "WORKFLOW.md",
            "AGENTS.md",
            "PomodoroTimer.CrossPlatform.slnx",
            "PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj",
            "PomodoroTimer.Android/PomodoroTimer.Android.csproj",
            "PomodoroTimer.iOS/PomodoroTimer.iOS.csproj",
            "docs/ARCHITECTURE.md",
            "docs/exec-plans/README.md",
            "docs/exec-plans/active/README.md",
            "docs/exec-plans/completed/README.md",
            "docs/exec-plans/tech-debt-tracker.md",
            "docs/exec-plans/active/AI-9-harness-engineering-agents-map.md"
        })
        {
            Assert.True(File.Exists(GetRepoPath(relativePath)), $"Expected required documentation file '{relativePath}' to exist.");
        }
    }

    [Fact]
    public void SupportingDocsDescribeTheExpectedStructure()
    {
        var architectureText = File.ReadAllText(GetRepoPath("docs", "ARCHITECTURE.md"));

        foreach (var requiredSnippet in new[]
        {
            "PomodoroTimer/",
            "PomodoroTimer.Desktop/",
            "PomodoroTimer.Android/",
            "PomodoroTimer.iOS/",
            "PomodoroTimer.Tests/",
            "PomodoroTimer/Views/MainView.axaml",
            "PomodoroTimer/ViewModels/MainWindowViewModel.cs",
            "PomodoroTimer/Models/PomodoroTimerState.cs",
            "dotnet build PomodoroTimer.sln",
            "dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj",
            "[exec-plans/tech-debt-tracker.md](exec-plans/tech-debt-tracker.md)"
        })
        {
            Assert.Contains(requiredSnippet, architectureText, StringComparison.Ordinal);
        }

        var readmeText = File.ReadAllText(GetRepoPath("README.md"));

        foreach (var requiredSnippet in new[]
        {
            "PomodoroTimer.CrossPlatform.slnx",
            "PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj",
            "PomodoroTimer.Android/PomodoroTimer.Android.csproj",
            "PomodoroTimer.iOS/PomodoroTimer.iOS.csproj"
        })
        {
            Assert.Contains(requiredSnippet, readmeText, StringComparison.Ordinal);
        }

        var plansReadmeText = File.ReadAllText(GetRepoPath("docs", "exec-plans", "README.md"));

        foreach (var requiredSnippet in new[]
        {
            "[active/README.md](active/README.md)",
            "[completed/README.md](completed/README.md)",
            "[tech-debt-tracker.md](tech-debt-tracker.md)",
            "`AI-9-harness-engineering-guide.md`",
            "Move the file to `completed/` when the work is merged or otherwise finished."
        })
        {
            Assert.Contains(requiredSnippet, plansReadmeText, StringComparison.Ordinal);
        }

        var solutionText = File.ReadAllText(GetRepoPath("PomodoroTimer.sln"));
        Assert.Contains("PomodoroTimer.Desktop\\PomodoroTimer.Desktop.csproj", solutionText, StringComparison.Ordinal);

        var crossPlatformSolutionText = File.ReadAllText(GetRepoPath("PomodoroTimer.CrossPlatform.slnx"));
        Assert.Contains("PomodoroTimer.Android/PomodoroTimer.Android.csproj", crossPlatformSolutionText, StringComparison.Ordinal);
        Assert.Contains("PomodoroTimer.iOS/PomodoroTimer.iOS.csproj", crossPlatformSolutionText, StringComparison.Ordinal);
        Assert.Contains("PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj", crossPlatformSolutionText, StringComparison.Ordinal);

        var activePlanText = File.ReadAllText(GetRepoPath("docs", "exec-plans", "active", "AI-9-harness-engineering-agents-map.md"));

        foreach (var requiredSnippet in new[]
        {
            "## Scope",
            "## Assumptions",
            "## Steps",
            "## Validation",
            "## Progress Log",
            "without modifying the existing GitHub Actions",
            "workflow."
        })
        {
            Assert.Contains(requiredSnippet, activePlanText, StringComparison.Ordinal);
        }
    }

    private static string GetRepoPath(params string[] relativeSegments)
    {
        var segments = new List<string> { RepoRoot };
        segments.AddRange(relativeSegments);
        return Path.Combine(segments.ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null && !File.Exists(Path.Combine(current.FullName, "PomodoroTimer.sln")))
        {
            current = current.Parent;
        }

        return current?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate the repository root from the test output directory.");
    }
}
