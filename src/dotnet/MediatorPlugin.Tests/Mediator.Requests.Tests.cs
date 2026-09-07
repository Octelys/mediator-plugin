using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Octelys.MediatorPlugin.Tests.Infrastructure;

namespace Octelys.MediatorPlugin.Tests;

[TestNetCoreLatest]
[TestReferences("System.Runtime")]
public abstract class MediatorRequestsTestsBase : LookupTestBase
{
    [Test]
    public void FindHandlers_RequestImplementsMultipleRequestInterfaces_HandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "NewMediatorRequest",
            ["NewMediatorHandler"],
            TestSolutionFiles.Mediator
        );
    }
}
