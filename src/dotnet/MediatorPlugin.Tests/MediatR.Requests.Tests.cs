using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Octelys.MediatorPlugin.Tests.Infrastructure;

namespace Octelys.MediatorPlugin.Tests;

[TestNetCoreLatest]
[MediatRTestPackages]
[TestReferences("System.Runtime")]
public class MediatRRequestsTests : LookupTestBase
{
    [Test]
    public void FindHandlers_HandlerDeclaredInSameNamespace_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "Domain.GetEntityRequest",
            ["GetEntityHandler"],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_RequestAndHandlerAreInACustomNamespace_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "Domain.OtherNamespace.GetEntityRequest",
            ["GetEntityHandler"],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_RequestImplementsCustomBaseRequestInterface_EveryHandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "CustomClassRequest",
            ["CustomClassHandler", "DuplicateHandler"],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_RequestIsARecord_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "CustomRecordRequest",
            ["CustomRecordHandler"],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_HandlerIsNestedInsideOuterClasses_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "InnerClassRequest",
            ["InnerClassHandler"],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_HandlerInheritsFromAbstractBaseClass_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "OtherRequest",
            ["BaseRequestHandler"],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_RequestHasNoHandler_NoHandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "NewRequest",
            [],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_RequestIsAReadonlyRecord_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "ReadonlyRecordStructRequest",
            ["ReadonlyRecordStructHandler"],
            TestSolutionFiles.MediatR
        );
    }

    [Test]
    public void FindHandlers_HandlerImplementsSharedGenericHandlerInterface_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "XCommandRequest",
            ["XCommandHandler"],
            TestSolutionFiles.MediatR
        );
    }
}