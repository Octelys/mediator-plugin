using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Octelys.MediatorPlugin.Tests.Infrastructure;

namespace Octelys.MediatorPlugin.Tests;

[TestNetCoreLatest]
[MediatorTestPackages]
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
