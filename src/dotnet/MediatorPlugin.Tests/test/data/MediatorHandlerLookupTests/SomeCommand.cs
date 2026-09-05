using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ClassLibrary1;

public class SomeCommand : IRequest
{
}

public class SomeCommandHandler : IRequestHandler<SomeCommand>
{
    public Task Handle(SomeCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
