using System.IO;
using System.Linq;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using NUnit.Framework;
using ReSharper.MediatorPlugin.Services.Libraries;

namespace Octelys.MediatorPlugin.Tests;

/// <summary>
/// Positions a real caret (via the "{caret}" marker in each test's data file) and asserts which
/// handler file the plugin resolves, if any, for the identifier under it. Unlike <see cref="LookupTestBase" />,
/// which locates the request's declaration identifier directly through the PSI, this test goes
/// through the same caret-to-PSI resolution (<see cref="TextControlToPsi" />) that the plugin's
/// own actions use (e.g. GoToHandlerAction.GetSelectedMediatrRequest), so it exercises the literal
/// "caret sits on this token" scenario rather than simulating it by name. <see cref="CaretInNamespace" />
/// and <see cref="CaretOnSendCall" /> place the caret on identifiers that are not a Mediator request
/// declaration and assert that no handler is resolved.
/// </summary>
[TestNetCoreLatest]
[TestReferences("System.Runtime")]
public abstract class GoToHandlerCaretTestsBase : BaseTestWithTextControl
{
    private static readonly string[] TestsExpectingNoHandler =
    {
        nameof(CaretInNamespace),
        nameof(CaretOnSendCall)
    };

    protected override string RelativeTestDataPath => "GoToHandlerCaretTests";

    [Test]
    public void Request() => DoNamedTest("Handler.cs");

    [Test]
    public void RequestCaretAtBeginning() => DoNamedTest("Handler.cs");

    [Test]
    public void CaretInNamespace() => DoNamedTest("Handler.cs");

    [Test]
    public void CaretOnSendCall() => DoNamedTest("Handler.cs");

    protected override void DoTest
    (
        Lifetime lifetime,
        ISolution solution
    )
    {
        solution.GetPsiServices().Files.CommitAllDocuments();

        using (ReadLockCookie.Create())
        {
            ITextControl textControl = OpenTextControl(lifetime, GetCaretPosition());

            IIdentifier requestIdentifier = TextControlToPsi.GetElement<IIdentifier>(solution, textControl);

            Assert.That(requestIdentifier, Is.Not.Null, "No identifier was found at the caret.");

            ITypeElement[] handlers = new LibraryAdaptor().FindHandlers(requestIdentifier).ToArray();

            if (TestsExpectingNoHandler.Contains(TestMethodName))
            {
                Assert.That(handlers, Is.Empty, "Expected no handler to be resolved from this caret position.");

                return;
            }

            string handlerFileName = handlers
                .Single()
                .GetSourceFiles()
                .Select(sourceFile => Path.GetFileName(sourceFile.GetLocation().FullPath))
                .Single();

            Assert.That(handlerFileName, Is.EqualTo("Handler.cs"));
        }
    }
}
