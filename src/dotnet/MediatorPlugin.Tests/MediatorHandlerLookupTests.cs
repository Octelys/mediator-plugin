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
/// Exercises the handler lookup against a real solution: the framework restores MediatR,
/// builds the PSI caches, and the plugin resolves handlers the same way it does in the IDE.
/// </summary>
[TestPackages("MediatR/12.1.0")]
public class MediatorHandlerLookupTests : BaseTestWithSingleProject
{
    private string _requestTypeName;
    private string[] _expectedHandlerNames;

    protected override string RelativeTestDataPath => nameof(MediatorHandlerLookupTests);

    [Test]
    public void FindsHandlerDeclaredInAnotherFile()
    {
        AssertHandlersFor
        (
            "SomeQuery",
            ["SomeQueryHandler"],
            "SomeQuery.cs", "SomeQueryHandler.cs"
        );
    }

    [Test]
    public void FindsHandlerForCommandWithoutResponse()
    {
        AssertHandlersFor
        (
            "SomeCommand",
            ["SomeCommandHandler"],
            "SomeCommand.cs"
        );
    }

    [Test]
    public void FindsEveryHandlerOfANotification()
    {
        AssertHandlersFor
        (
            "SomethingHappened",
            ["AuditSomethingHappenedHandler", "NotifySomethingHappenedHandler"],
            "SomethingHappened.cs"
        );
    }

    [Test]
    public void FindsNoHandlerWhenTheRequestHasNone()
    {
        AssertHandlersFor
        (
            "OrphanQuery",
            [],
            "OrphanQuery.cs"
        );
    }

    [Test]
    public void FindsNoHandlerWhenTheTypeIsNotARequest()
    {
        AssertHandlersFor
        (
            "NotARequest",
            [],
            "NotARequest.cs"
        );
    }

    protected override void DoTest(Lifetime lifetime, ISolution solution)
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

    private void AssertHandlersFor
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
    /// when the caret sits on a request in the editor.
    /// </summary>
    private static IIdentifier FindDeclarationIdentifier
    (
        ISolution solution,
        string typeName
    )
    {
        IIdentifier identifier = EnumerateIdentifiers(solution)
            .FirstOrDefault
            (
                candidate => candidate.Name == typeName && candidate.Parent is IClassLikeDeclaration
            );

        Assert.That(identifier, Is.Not.Null, $"No declaration of '{typeName}' was found in the test solution.");

        return identifier;
    }

    private static IEnumerable<IIdentifier> EnumerateIdentifiers
    (
        ISolution solution
    )
    {
        foreach (IProject project in solution.GetAllProjects())
        foreach (IProjectFile projectFile in project.GetAllProjectFiles(_ => true))
        {
            IPsiSourceFile sourceFile = projectFile.ToSourceFile();

            if (sourceFile is null)
                continue;

            IFile psiFile = sourceFile.GetPrimaryPsiFile();

            if (psiFile is null)
                continue;

            foreach (IIdentifier identifier in psiFile.Descendants<IIdentifier>().ToEnumerable())
                yield return identifier;
        }
    }
}
