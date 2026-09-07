using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace Octelys.MediatorPlugin.Tests;

[TestNetCoreLatest]
[TestPackages("Mediator.Abstractions/3.0.1")]
[TestReferences("System.Runtime")]
public class MediatorRequestsTests : LookupTestBase
{
    [Test]
    public void FindHandlers_RequestImplementsMultipleRequestInterfaces_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "NewMediatorRequest",
            ["NewMediatorHandler"],
            "Mediator/Requests/NewMediatorRequest.cs", "Mediator/Handlers/NewMediatorHandler.cs", "Entities/Dto.cs"
        );
    }
}
