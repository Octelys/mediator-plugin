using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Octelys.MediatorPlugin.Tests.Infrastructure;

namespace Octelys.MediatorPlugin.Tests;

[TestNetCoreLatest]
[MediatorTestPackages]
[TestReferences("System.Runtime")]
public class MediatorNotificationsTests : LookupTestBase
{
    [Test]
    public void FindHandlers_NotificationHasMultipleHandlers_EveryHandlerReturned()
    {
        //  Act & Assert.
        AssertHandlersFor
        (
            "NewMediatorNotification",
            ["NewMediatorNotificationHandler", "MediatorNotificationHandler"],
            "Mediator/Notifications/NewMediatorNotification.cs", "Mediator/Handlers/NewMediatorNotificationHandler.cs", "Mediator/Handlers/OtherMediatorNotificationHandler.cs", "Entities/Dto.cs"
        );
    }
}
