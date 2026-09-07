using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Octelys.MediatorPlugin.Tests.Infrastructure;

/// <summary>
/// Enumerates every file under a library's folder in the shared TestSolution test data (plus the
/// common Entities folder), so lookup tests can load the whole library instead of curating a file
/// subset per test.
/// </summary>
public static class TestSolutionFiles
{
    private static readonly string TestSolutionPath = ResolveTestSolutionPath();

    public static string[] MediatR { get; } = LibraryFiles("MediatR");

    public static string[] Mediator { get; } = LibraryFiles("Mediator");

    private static string[] LibraryFiles
    (
        string libraryFolderName
    )
    {
        return new[] { libraryFolderName, "Entities" }
            .SelectMany(EnumerateRelativeFiles)
            .OrderBy(relativePath => relativePath)
            .ToArray();
    }

    private static IEnumerable<string> EnumerateRelativeFiles
    (
        string folderName
    )
    {
        string folderPath = Path.Combine(TestSolutionPath, folderName);

        return Directory
            .EnumerateFiles(folderPath, "*.cs", SearchOption.AllDirectories)
            .Select(filePath => filePath.Substring(TestSolutionPath.Length + 1).Replace(Path.DirectorySeparatorChar, '/'));
    }

    private static string ResolveTestSolutionPath
    (
        [CallerFilePath] string sourceFilePath = ""
    )
    {
        string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(sourceFilePath));

        return Path.Combine(projectPath!, "test", "data", "TestSolution");
    }
}
