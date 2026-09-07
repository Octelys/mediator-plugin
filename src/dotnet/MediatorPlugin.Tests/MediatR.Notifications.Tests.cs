using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Octelys.MediatorPlugin.Tests.Infrastructure;

namespace Octelys.MediatorPlugin.Tests;

[TestNetCoreLatest]
[MediatRTestPackages]
[TestReferences("System.Runtime")]
public class MediatRNotificationsTests : LookupTestBase
{
    [Test]
    public void FindHandlers_NotificationHasMultipleHandlers_EveryHandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "CustomNotification",
            ["FirstCustomNotificationHandler", "SecondCustomNotificationHandler"],
            "MediatR/Notifications/CustomNotification.cs", "MediatR/Handlers/FirstCustomNotificationHandler.cs", "MediatR/Handlers/SecondCustomNotificationHandler.cs"
        );
    }
}
