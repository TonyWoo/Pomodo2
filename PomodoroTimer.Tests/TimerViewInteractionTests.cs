using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class TimerViewInteractionTests
{
    private static readonly string RepoRoot = FindRepoRoot();

    [Fact]
    public void TaskDescriptionInputsBindEnterToAddTaskCommand()
    {
        XNamespace avalonia = "https://github.com/avaloniaui";
        var document = XDocument.Load(GetRepoPath("PomodoroTimer", "Views", "TimerView.axaml"));
        var taskInputs = document
            .Descendants(avalonia + "TextBox")
            .Where(element => string.Equals((string?)element.Attribute("Text"), "{Binding Topic}", StringComparison.Ordinal))
            .ToList();

        Assert.Equal(2, taskInputs.Count);

        foreach (var taskInput in taskInputs)
        {
            var keyBindings = taskInput
                .Element(avalonia + "TextBox.KeyBindings")
                ?.Elements(avalonia + "KeyBinding")
                .ToList();

            Assert.NotNull(keyBindings);
            Assert.Contains(keyBindings, binding =>
                string.Equals((string?)binding.Attribute("Gesture"), "Enter", StringComparison.Ordinal)
                && string.Equals((string?)binding.Attribute("Command"), "{Binding AddTaskCommand}", StringComparison.Ordinal));
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
