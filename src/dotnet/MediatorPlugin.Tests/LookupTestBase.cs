using System.Collections.Generic;
using System.Linq;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReSharper.MediatorPlugin.Services.Libraries;

namespace Octelys.MediatorPlugin.Tests;

/// <summary>
/// Shared plumbing for tests that load a small test solution and assert which handlers the
/// plugin resolves for a given request or notification type. Test data lives under
/// test/data/TestSolution, organized per library (MediatR, Mediator) and kind (Requests,
/// Notifications, Handlers, Entities).
/// </summary>
public abstract class LookupTestBase : BaseTestWithSingleProject
{
    private string _requestTypeName;
    private string[] _expectedHandlerNames;

    protected override string RelativeTestDataPath => "TestSolution";

    /// <summary>
    /// Runs inside the loaded test solution: the framework calls this once the project of the
    /// current test has been restored and its PSI caches are ready.
    /// </summary>
    protected override void DoTest
    (
        Lifetime lifetime,
        ISolution solution
    )
    {
        solution.GetPsiServices().Files.CommitAllDocuments();

        using (ReadLockCookie.Create())
        {
            IIdentifier requestIdentifier = FindDeclarationIdentifier(solution, _requestTypeName);

            IEnumerable<ITypeElement> handlers = new LibraryAdaptor().FindHandlers(requestIdentifier);

            string[] handlerNames = handlers
                .Select(handler => handler.ShortName)
                .OrderBy(name => name)
                .ToArray();

            Assert.That(handlerNames, Is.EqualTo(_expectedHandlerNames.OrderBy(name => name).ToArray()));
        }
    }

    /// <summary>
    /// Loads the given files as a test solution and asserts that the request or notification
    /// resolves to exactly the expected handlers; the lookup itself runs in <see cref="DoTest" />.
    /// </summary>
    protected void AssertHandlersFor
    (
        string requestTypeName,
        string[] expectedHandlerNames,
        params string[] fileNames
    )
    {
        _requestTypeName = requestTypeName;
        _expectedHandlerNames = expectedHandlerNames;

        DoTestSolution(fileNames);
    }

    /// <summary>
    /// Returns the name token of the type declaration, which is the node the plugin receives
    /// when the caret sits on a request in the editor. <paramref name="typeName" /> may be a
    /// short name ("GetEntityRequest") when it is unique in the loaded files, or a fully
    /// qualified name ("Domain.OtherNamespace.GetEntityRequest") to disambiguate types that
    /// share a short name across namespaces.
    /// </summary>
    private static IIdentifier FindDeclarationIdentifier
    (
        ISolution solution,
        string typeName
    )
    {
        bool isQualified = typeName.Contains('.');

        IIdentifier[] candidates = EnumerateIdentifiers(solution)
            .Where(candidate => candidate.Parent is IClassLikeDeclaration)
            .Where(candidate => isQualified ? GetFullName(candidate) == typeName : candidate.Name == typeName)
            .ToArray();

        Assert.That
        (
            candidates,
            Has.Length.EqualTo(1),
            candidates.Length == 0
                ? $"No declaration of '{typeName}' was found in the test solution."
                : $"Multiple declarations of '{typeName}' were found: {string.Join(", ", candidates.Select(GetFullName))}. " +
                  "Qualify the name with its namespace to disambiguate."
        );

        return candidates[0];
    }

    private static string GetFullName
    (
        IIdentifier identifier
    )
    {
        IClassLikeDeclaration declaration = (IClassLikeDeclaration)identifier.Parent;

        return ((ITypeElement)declaration.DeclaredElement).GetClrName().FullName;
    }

    private static IEnumerable<IIdentifier> EnumerateIdentifiers
    (
        ISolution solution
    )
    {
        foreach (IProject project in solution.GetAllProjects())
        {
            foreach (IProjectFile projectFile in project.GetAllProjectFiles(_ => true))
            {
                IPsiSourceFile sourceFile = projectFile.ToSourceFile();

                if (sourceFile is null)
                {
                    continue;
                }

                IFile psiFile = sourceFile.GetPrimaryPsiFile();

                if (psiFile is null)
                {
                    continue;
                }

                foreach (IIdentifier identifier in psiFile.Descendants<IIdentifier>().ToEnumerable())
                {
                    yield return identifier;
                }
            }
        }
    }
}
