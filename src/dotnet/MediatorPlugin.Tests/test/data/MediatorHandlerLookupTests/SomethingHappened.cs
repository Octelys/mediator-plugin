using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ClassLibrary1;

public class SomethingHappened : INotification
{
}

public class AuditSomethingHappenedHandler : INotificationHandler<SomethingHappened>
{
    public Task Handle(SomethingHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class NotifySomethingHappenedHandler : INotificationHandler<SomethingHappened>
{
    public Task Handle(SomethingHappened notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
