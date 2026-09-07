using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace Octelys.MediatorPlugin.Tests;

[TestNetCoreLatest]
[TestPackages("MediatR/12.5.0", "MediatR/13.1.0", "MediatR/14.2.0")]
[TestReferences("System.Runtime")]
public class MediatRRequestsTests : LookupTestBase
{
    [Test]
    public void FindHandlers_HandlerDeclaredInSameNamespace_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "GetEntityRequest",
            ["GetEntityHandler"],
            "MediatR/Requests/GetEntityRequest.cs", "MediatR/Handlers/GetEntityHandler.cs", "Entities/GetEntityResponse.cs"
        );
    }

    [Test]
    public void FindHandlers_RequestAndHandlerAreInACustomNamespace_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "GetEntityRequest",
            ["GetEntityHandler"],
            "MediatR/Requests/OtherNamespace/GetEntityRequest.cs", "MediatR/Handlers/OtherNamespace/GetEntityHandler.cs", "Entities/OtherNamespace/GetEntityResponse.cs"
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
            "MediatR/Requests/ICustomRequest.cs", "MediatR/Requests/CustomClassRequest.cs", "MediatR/Handlers/CustomClassHandler.cs", "MediatR/Handlers/DuplicateHandler.cs"
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
            "MediatR/Requests/ICustomRequest.cs", "MediatR/Requests/CustomRecordRequest.cs", "MediatR/Handlers/CustomRecordHandler.cs"
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
            "MediatR/Requests/InnerClassRequest.cs", "MediatR/Handlers/OuterClassHandler.cs"
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
            "MediatR/Requests/ICustomRequest.cs", "MediatR/Requests/OtherRequest.cs", "MediatR/Requests/NewRequest.cs", "MediatR/Handlers/BaseRequestHandler.cs"
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
            "MediatR/Requests/NewRequest.cs"
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
            "MediatR/Requests/ReadonlyRecordStructRequest.cs", "MediatR/Handlers/ReadonlyRecordStructHandler.cs", "Entities/ReadonlyRecordStructResponse.cs"
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
            "MediatR/Requests/XCommandRequest.cs", "MediatR/Handlers/XCommandHandler.cs", "MediatR/Handlers/ISharedCommandHandler.cs", "Entities/Dto.cs"
        );
    }
}